using IdentityServer4.Models;

namespace Server
{
    public class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource
                (
                    "roles",
                    "Your role(s)",
                    new List<string> {"role"}
                ) ,
                new IdentityResource
                (
                    "claims", 
                    "Your claims",
                    new List<string> {"user-id"}
                )
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new[] { new ApiScope("EComAPI.read"), new ApiScope("EComAPI.write"), };
        public static IEnumerable<ApiResource> ApiResources =>
            new[]
            {
                new ApiResource("EComAPI")
                {
                    Scopes = new List<string> { "EComAPI.read", "EComAPI.write" },
                    ApiSecrets = new List<Secret> { new Secret("ScopeSecret".Sha256()) },
                    UserClaims = new List<string> { "role", "user-id" }
                }
            };

        public static IEnumerable<Client> Clients =>
            new[]
            {
                // m2m client credentials flow client
                new Client
                {
                    ClientId = "m2m.client",
                    ClientName = "Client Credentials Client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("ClientSecret1".Sha256()) },
                    AllowedScopes = { "EComAPI.read", "EComAPI.write" }
                },

                new Client()
                {
                    ClientId = "interactive",
                    AllowedGrantTypes = GrantTypes.Code,
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    RedirectUris = { "https://localhost:4200/signin-oidc" },
                    FrontChannelLogoutUri = "https://localhost:4200/signout-oidc",
                    PostLogoutRedirectUris = { "https://localhost:4200/signout-callback-oidc" },
                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "EComAPI.read", "roles", "claims", "offline_access" },
                    RequirePkce = true,
                    RequireConsent = true,
                    AllowPlainTextPkce = false,
                    RequireClientSecret = false,
                    AllowedCorsOrigins={ "https://localhost:4200", "http://localhost:8001"}
                }

            };
    }
}
