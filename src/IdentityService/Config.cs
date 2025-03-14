﻿using Duende.IdentityServer.Models;

namespace IdentityService;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("bookStoreMicroserviceApp", "Book store microservice app full access")
        };

    public static IEnumerable<Client> Clients(IConfiguration config) =>
        new Client[]
        {
            new Client
            {
                ClientId = "postman",
                ClientName = "Postman",
                AllowedScopes = ["openid", "profile", "bookStoreMicroserviceApp"],
                ClientSecrets = [new Secret("NotASecret".Sha256())],
                AllowedGrantTypes = [GrantType.ResourceOwnerPassword]
            },
            new Client
            {
                ClientId = "nextApp",
                ClientName = "nextApp",
                AllowedScopes = ["openid", "profile", "bookStoreMicroserviceApp"],
                ClientSecrets = [new Secret("SurelyNotASecret".Sha256())],
                AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                RequirePkce = false,
                RedirectUris = [$"{config["ClientApp"]}/api/auth/callback/id-server"],
                AllowOfflineAccess = true,
                AccessTokenLifetime = 3600*24*30,
                AlwaysIncludeUserClaimsInIdToken = true
            }
        };
}
