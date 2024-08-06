using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SudokuVS.Server.Exceptions;
using SudokuVS.Server.Infrastructure.Database;
using SudokuVS.Server.Infrastructure.Database.Models;

namespace SudokuVS.Server.Infrastructure.Authentication;

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

    /// <summary>
    ///     Create a new api key for the given user and return a valid token for the key.
    /// </summary>
    public async Task<string> CreateNewApiKeyAsync(AppUser user)
    {
        ApiKeyEntity key = new(user);
        await _context.ApiKeys.AddAsync(key);
        await _context.SaveChangesAsync();
        return GenerateKeyToken(key);
    }

    /// <summary>
    ///     Get the API keys of the user.
    /// </summary>
    public async Task<IReadOnlyList<string>> GetApiKeysAsync(AppUser user)
    {
        List<ApiKeyEntity> keys = await _context.ApiKeys.Where(k => k.User == user).AsNoTracking().ToListAsync();
        return keys.Select(GenerateKeyToken).ToList();
    }

    /// <summary>
    ///     Revoke the given api key of the given user
    /// </summary>
    public async Task RevokeApiKeyAsync(AppUser user, string apiKey)
    {
        ApiKeyEntity key = await GetApiKeyAsync(apiKey) ?? throw new BadRequestException("Bad api key");
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

    string GenerateKeyToken(ApiKeyEntity keyEntity)
    {
        Claim[] claims = [new Claim(ApiKeySchemeOptions.KeyIdClaimType, keyEntity.Id.ToString())];
        JwtSecurityToken token = new(claims: claims, signingCredentials: _credentials);
        return _tokenHandler.WriteToken(token);
    }

    async Task<ApiKeyEntity?> GetApiKeyAsync(string tokenStr)
    {
        JwtSecurityToken token = _tokenHandler.ReadJwtToken(tokenStr);
        string? keyIdStr = token.Claims.FirstOrDefault(c => c.Type == ApiKeySchemeOptions.KeyIdClaimType)?.Value;
        if (keyIdStr == null || !Guid.TryParse(keyIdStr, out Guid keyId))
        {
            return null;
        }

        return await _context.ApiKeys.SingleOrDefaultAsync(k => k.Id == keyId);
    }
}
