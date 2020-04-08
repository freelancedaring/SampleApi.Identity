namespace SampleApi.Identity
{
    public class AdminConfiguration : IAdminConfiguration
    {

        public string IdentityServerBaseUrl { get; set; } = AuthorizationConsts.IdentityServerBaseUrl;
        public string OidcApiName { get; set; } = AuthorizationConsts.OidcApiName;
        public string OidcSwaggerUIClientId { get; set; } = AuthorizationConsts.OidcSwaggerUIClientId;
    }
}