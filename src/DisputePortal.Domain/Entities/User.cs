namespace DisputePortal.Domain.Entities;

public class User
{
    private User() { }

    public User(string email, string displayName, Role role)
    {
        Id = Guid.NewGuid();
        Email = email;
        DisplayName = displayName;
        Role = role;
    }

    public Guid Id { get; private set; }
    public string Email { get; private set; } = null!;
    public string DisplayName { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public Role Role { get; private set; }
    public void SetPasswordHash(string hash) => PasswordHash = hash;
}
