namespace OpenID.Models;

public class CallbackRequest
{
    public string state { get; set; } = null!;
    public string code { get; set; } = null!;
    public string scope { get; set; } = null!;
    public string authuser { get; set; } = null!;
    public string hd { get; set; } = null!;
    public string prompt { get; set; } = null!;
}