namespace SonGun.Models;

public class AuthSettings
{
    public const string SectionName = "AuthSettings";

    public string AdminUsername { get; set; } = string.Empty;
    public string AdminPassword { get; set; } = string.Empty;
    public string EditorUsername { get; set; } = string.Empty;
    public string EditorPassword { get; set; } = string.Empty;
}
