using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SudokuVS.Server.Exceptions;
using SudokuVS.Server.Infrastructure.Database;
using SudokuVS.Server.Infrastructure.Database.Models;

namespace SudokuVS.Server.Infrastructure.Authentication.ApiKey;

public class ApiKeyService
{
    readonly JwtSecurityTokenHandler _tokenHandler;
    readonly SigningCredentials _credentials;
    readonly AppDbContext _context;

    public ApiKeyService(IOptions<ApiKeyOptions> options, AppDbContext context)
    {
        _tokenHandler = new JwtSecurityTokenHandler();
        _context = context;

        string? credSecret = options.Value.Secret;
        if (string.IsNullOrWhiteSpace(credSecret))
        {
            throw new InternalErrorException("Signing credentials not provided, cannot generate a token for the api key.");
        }

        byte[] credSecretBytes = Encoding.UTF8.GetBytes(credSecret);
        byte[] secret = SHA512.HashData(credSecretBytes);

        SymmetricSecurityKey key = new(secret);
        _credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
    }

    IIncludableQueryable<ApiKeyEntity, AppUser> AllApiKeys => _context.ApiKeys.Include(k => k.User);

    /// <summary>
    ///     Create a new api key for the given user and return a valid token for the key.
    /// </summary>
    public async Task<ApiKey> CreateNewApiKeyAsync(AppUser user, string? name = null)
    {
        ApiKeyEntity key = new(user, name);
        await _context.ApiKeys.AddAsync(key);
        await _context.SaveChangesAsync();
        return ToApiKey(key);
    }

    /// <summary>
    ///     Get the API keys of the user.
    /// </summary>
    public async Task<IReadOnlyList<ApiKey>> GetApiKeysAsync(AppUser user)
    {
        List<ApiKeyEntity> keys = await AllApiKeys.Where(k => k.User == user).AsNoTracking().ToListAsync();
        return keys.Select(ToApiKey).ToList();
    }

    /// <summary>
    ///     Revoke the given api key of the given user
    /// </summary>
    public async Task RevokeApiKeyAsync(AppUser user, string token)
    {
        ApiKeyEntity key = await GetApiKeyAsync(token) ?? throw new BadRequestException("Bad api key");
        if (key.User != user)
        {
            throw new BadRequestException("Bad api key");
        }

        _context.ApiKeys.Remove(key);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    ///     Revoke the given api key of the given user
    /// </summary>
    public async Task RevokeAllApiKeysAsync(AppUser user)
    {
        IQueryable<ApiKeyEntity> keys = _context.ApiKeys.Where(k => k.User == user);
        _context.ApiKeys.RemoveRange(keys);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    ///     Create a new api key for the given user and return a valid token for the key.
    /// </summary>
    public async Task<AppUser?> FindUserAsync(string token) => (await GetApiKeyAsync(token))?.User;

    /// <summary>
    ///     Return true if the provided token is valid: it exists, and it is associated with a user.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<bool> ValidateTokenAsync(string token)
    {
        Guid? keyId = GetApiKeyIdFromToken(token);
        if (!keyId.HasValue)
        {
            return false;
        }

        return await AllApiKeys.AsNoTracking().AnyAsync(k => k.Id == keyId.Value && k.User != null);
    }

    string GenerateKeyToken(ApiKeyEntity keyEntity)
    {
        List<Claim> claims =
        [
            new Claim(ApiKeyConstants.KeyIdClaimType, keyEntity.Id.ToString())
        ];

        if (!string.IsNullOrWhiteSpace(keyEntity.Name))
        {
            claims.Add(new Claim(ApiKeyConstants.KeyNameClaimType, keyEntity.Name));
        }

        JwtSecurityToken token = new(claims: claims, signingCredentials: _credentials);
        return _tokenHandler.WriteToken(token);
    }

    async Task<ApiKeyEntity?> GetApiKeyAsync(string token)
    {
        Guid? keyId = GetApiKeyIdFromToken(token);
        if (!keyId.HasValue)
        {
            return null;
        }

        return await AllApiKeys.SingleOrDefaultAsync(k => k.Id == keyId.Value);
    }

    Guid? GetApiKeyIdFromToken(string tokenStr)
    {
        if (!_tokenHandler.CanReadToken(tokenStr))
        {
            return null;
        }

        JwtSecurityToken token = _tokenHandler.ReadJwtToken(tokenStr);
        string? keyIdStr = token.Claims.FirstOrDefault(c => c.Type == ApiKeyConstants.KeyIdClaimType)?.Value;
        if (keyIdStr == null || !Guid.TryParse(keyIdStr, out Guid keyId))
        {
            return null;
        }

        return keyId;
    }

    ApiKey ToApiKey(ApiKeyEntity key) => new(key.Id, key.CreationDate, key.Name, GenerateKeyToken(key));
}
