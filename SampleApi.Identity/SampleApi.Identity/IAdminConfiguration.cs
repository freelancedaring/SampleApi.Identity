namespace SampleApi.Identity
{
    public interface IAdminConfiguration
    {
        string IdentityAdminBaseUrl { get; }
        string ClientId { get; }
        string OidcApiName { get; set; }
        string OidcSwaggerUIClientId { get; set; }
        string IdentityServerBaseUrl { get; set; }

    }
}