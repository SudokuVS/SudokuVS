﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.Net.Http.Headers;

namespace SudokuVS.Server.Infrastructure.Authentication;

public class ApiKeySchemeOptions : AuthenticationSchemeOptions
{
    public const string Scheme = "ApiKeyScheme";
    public const string AuthenticationType = "ApiKey";
    public const string KeyIdClaimType = "api-key-id";

    public string HeaderName => HeaderNames.Authorization;
}
