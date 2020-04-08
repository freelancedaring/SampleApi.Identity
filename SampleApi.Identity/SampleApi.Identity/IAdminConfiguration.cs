namespace SampleApi.Identity
{
    public interface IAdminConfiguration
    {
        string OidcApiName { get; set; }
        string OidcSwaggerUIClientId { get; set; }
        string IdentityServerBaseUrl { get; set; }

    }
}