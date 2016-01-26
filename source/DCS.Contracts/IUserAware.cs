using System;

namespace DCS.Contracts
{
    public interface IUserAware
    {
        Guid UserId { get; set; }
    }

    public interface IUsernameAware
    {
        string Username { get; set; }
    }
}