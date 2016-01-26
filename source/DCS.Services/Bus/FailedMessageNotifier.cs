using DCS.Contracts;
using DCS.Contracts.Entities;
using DCS.Core;
using DCS.ServerRuntime.Entities;
using Newtonsoft.Json;
using Rebus;

namespace DCS.Services.Bus
{
    public class FailedMessageNotifier : IHandleMessages<FailedMessage>
    {
        private readonly IBus _bus;
        private readonly EntitiesRoot _entities;

        public FailedMessageNotifier(IBus bus, EntitiesRoot entities)
        {
            _bus = bus;
            _entities = entities;
        }

        public void Handle(FailedMessage message)
        {
            UserEntity user = null;
            object subject = message.Message;
            if (subject.IfType<IUserAware>(u => user = _entities.Users.Get(u))
                || subject.IfType<IUsernameAware>(u => user = _entities.Users.Get(u.Username)))
            {
                _bus.Publish(new NotifyUser
                {
                    UserId = user.Id,
                    Subject = string.Format("Failed to process stage"),
                    Body = string.Format(JsonConvert.SerializeObject(subject))
                });
            }
        }
    }
}