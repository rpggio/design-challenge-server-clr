using System;

namespace DCS.Contracts
{
    public class NotifyUser : IUserAware
    {
        public Guid UserId { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}