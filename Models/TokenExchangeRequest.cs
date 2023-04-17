namespace OpenID.Models;

public class TokenExchangeRequest
{
    public string Code { get; set; } = null!;

    public string ClientId { get; set; } = null!;

    public string ClientSecret { get; set; } = null!;

    public string RedirectUri { get; set; } = null!;

    public string GrantType { get; set; } = null!;

    public static FormUrlEncodedContent ToFormUrlEncodedContent(TokenExchangeRequest request)
    {
        var keyValuePairs = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("code", request.Code),
            new KeyValuePair<string, string>("client_id", request.ClientId),
            new KeyValuePair<string, string>("client_secret", request.ClientSecret),
            new KeyValuePair<string, string>("redirect_uri", request.RedirectUri),
            new KeyValuePair<string, string>("grant_type", request.GrantType),
        };

        return new FormUrlEncodedContent(keyValuePairs);
    }
}