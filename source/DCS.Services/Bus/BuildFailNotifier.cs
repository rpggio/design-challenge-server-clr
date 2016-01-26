using DCS.Contracts;
using DCS.Core;
using Rebus;

namespace DCS.Services.Bus
{
    public class BuildFailNotifier : IHandleMessages<UserBuildComplete>
    {
        private readonly IBus _bus;

        public BuildFailNotifier(IBus bus)
        {
            _bus = bus;
        }

        public void Handle(UserBuildComplete message)
        {
            if (message.Passed)
            {
                return;
            }

            _bus.Publish(new NotifyUser
            {
                UserId = message.UserId,
                Subject = string.Format("{0}: Build failed for {1}",
                    message.ChallengeName,
                    message.Repository),
                Body = message.Output.Left(300)
            });
        }
    }
}