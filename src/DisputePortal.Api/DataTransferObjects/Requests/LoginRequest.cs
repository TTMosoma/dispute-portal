namespace DisputePortal.Api.DataTransferObjects.Requests;

/// <summary>
/// Entity required to validate a user's credentials
/// </summary>
public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
