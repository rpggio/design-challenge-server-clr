using System;
using System.Linq;
using System.Reflection;
using DCS.Contracts.Entities;
using DCS.Core;
using log4net;
using Nancy;
using Nancy.ModelBinding;
using Newtonsoft.Json;
using Rebus;

namespace DCS.WebServices.Api
{
    public class MessagesEndpoint : NancyModule
    {
        private readonly ILog _log;
        private readonly IBus _bus;

        public MessagesEndpoint(IBus bus, ILog log)
            : base("/messages")
        {
            _bus = bus;
            _log = log;

            Post["/"] = parameters =>
            {
                string typeName = Request.Query.type;
                if (typeName == null)
                {
                    throw new Exception("type required");
                }

                var type =
                    typeof (MessagesEndpoint).Assembly.GetTypes()
                        .Concat(typeof (IEntity<>).Assembly.GetTypes())
                        .FirstOrDefault(t => t.Name.EqualsIgnoreCase(typeName));
                if (type == null)
                {
                    throw new Exception("Could not find type {0}".FormatFrom(typeName));
                }

                var message = Activator.CreateInstance(type);

                if (message == null)
                {
                    throw new Exception("could not deserialize message");
                }

                var bindToMethod = typeof (ModuleExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .Single(m => m.Name == "BindTo" && m.GetParameters().Count() == 2);
                bindToMethod.MakeGenericMethod(type).Invoke(this, new[] {this, message});

                _log.InfoFormat("Publishing inbound message: ({0}) {1}", type.Name, JsonConvert.SerializeObject(message));
                _bus.Publish(message);
                return message;
            };
        }
    }
}