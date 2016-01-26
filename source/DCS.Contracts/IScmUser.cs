namespace DCS.Contracts
{
    public interface IScmUser
    {
        string Username { get; }
        string Email { get; }
        string Password { get; }
    }
}