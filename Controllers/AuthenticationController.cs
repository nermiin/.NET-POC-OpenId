using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using OpenID.Models;
using System.Security.Cryptography;

namespace OpenID.Controllers;

public class AuthenticationController : Controller
{
    private string authorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
    private string clientId = "Your_client_id";
    private string clientSecret = "Your_client_secret";
    private string redirectUri = "https://localhost:5164/callback";
    private string scope = "openid profile email phone address";
    private string responseType = "code";
    private string tokenEndpoint = "https://oauth2.googleapis.com/token";

    const int tokenLength = 32;
    private string state = GenerateStateToken(tokenLength);
    

    [HttpGet("~/login")]
    public string Login()
    {
        // Redirect to OpenID Connect server
        string authorizationUrl = authorizationEndpoint
                                  + "?response_type="
                                  + responseType
                                  + "&client_id="
                                  + clientId
                                  + "&redirect_uri="
                                  + Uri.EscapeDataString(redirectUri)
                                  + "&scope="
                                  + Uri.EscapeDataString(scope)
                                  + "&state="
                                  + Uri.EscapeDataString(state);

        return authorizationUrl;
    }

    [HttpGet("~/callback")]
    public async Task<IActionResult> Callback(string code, string state)
    {
        if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(state))
        {
            return BadRequest("Code or state parameter is missing.");
        }

        // Exchange authorization code for access token

        var tokenRequestContent = new TokenExchangeRequest
        {
            Code = code,
            ClientId = clientId,
            ClientSecret = clientSecret,
            RedirectUri = redirectUri,
            GrantType = "authorization_code",
        };

        var client = new HttpClient();

        var requestBody = TokenExchangeRequest.ToFormUrlEncodedContent(tokenRequestContent);

        var tokenResponse = await client.PostAsync(tokenEndpoint, requestBody);

        if (!tokenResponse.IsSuccessStatusCode)
        {
            return BadRequest("Unable to exchange authorization code for access token.");
        }

        var tokenResponseString = await tokenResponse.Content.ReadAsStringAsync();
        var tokenResponseContent = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenResponse>(tokenResponseString);

        var jwtInfo = DecodeIdToken(tokenResponseContent.id_token);

        // Process the token response here
        return Ok(jwtInfo);
    }

    public static string DecodeIdToken(string idToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(idToken);

        // Extract claims
        var email = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;
        var name = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "name")?.Value;
        var pictureUrl = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "picture")?.Value;
        var locale = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "locale")?.Value;

        // Do something with the user information
        return $"User information:\nEmail: {email}\nName: {name}\nPicture URL: {pictureUrl}\nLocale: {locale}";
    }

    // Generate a 32-character cryptographically secure random string
    static string GenerateStateToken(int tokenLength)
    {

        byte[] randomBytes = new byte[tokenLength];

        var rng = RandomNumberGenerator.Create();

        rng.GetBytes(randomBytes);

        return Convert.ToBase64String(randomBytes);
    }
}