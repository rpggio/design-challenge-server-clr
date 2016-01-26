using Newtonsoft.Json;
using Rebus;

namespace DCS.Console.Commands
{
    public class PublishCommand : MessageCommandBase
    {
        private readonly IBus _bus;

        public PublishCommand(IBus bus)
        {
            _bus = bus;
        }

        public override void Execute(object message)
        {
            System.Console.WriteLine("Publishing message: {0}", JsonConvert.SerializeObject(message));
            _bus.Publish(message);
        }
    }
}