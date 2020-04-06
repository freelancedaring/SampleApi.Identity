namespace SampleApi.Identity
{
    public class AdminConfiguration : IAdminConfiguration
    {
        public string IdentityAdminBaseUrl { get; set; } = "http://localhost:9000";

        public string IdentityServerBaseUrl { get; set; } = "http://localhost:5000";

        public string ClientId { get; set; } = AuthenticationConsts.OidcClientId;

        public string OidcApiName { get; set; } = AuthorizationConsts.OidcApiName;
        public string OidcSwaggerUIClientId { get; set; } = AuthorizationConsts.OidcSwaggerUIClientId;
    }
}