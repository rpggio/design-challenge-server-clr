using System;
using System.CodeDom.Compiler;
using System.Linq;
using System.Reflection;
using Args;
using DCS.Contracts;
using DCS.Core;
using Newtonsoft.Json;

namespace DCS.Console.Commands
{
    public abstract class MessageCommandBase : IConsoleCommand
    {
        private object _message;
        private Type _messageType;

        public abstract void Execute(object meessage);

        public bool Execute()
        {
            if (_message == null)
            {
                throw new Exception("No message has been parsed.");
            }
            System.Console.WriteLine(JsonConvert.SerializeObject(_message));
            Execute(_message);
            return true;
        }

        public void PrintHelp(IndentedTextWriter writer)
        {
            writer.WriteLine("{0} <typeName> [ /messageProperty <value> ]*",
                GetType().Name.ToLower().Replace("command", ""));

            if (_messageType != null)
            {
                writer.WriteLine("properties:");
                writer.Indent++;
                foreach (var prop in _messageType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (prop.PropertyType.IsPrimitive
                        || prop.PropertyType.IsValueType
                        || prop.PropertyType == typeof (string)
                        || prop.PropertyType == typeof (Guid)
                        || prop.PropertyType == typeof (Guid?)
                        || prop.PropertyType == typeof (DateTime)
                        || prop.PropertyType == typeof (DateTime?))
                    {
                        writer.WriteLine("/{0} ({1})", prop.Name, prop.PropertyType.Name);
                    }
                }
            }
        }

        public bool TryParse(string[] args, out string message)
        {
            message = null;

            if (args.Length == 0)
            {
                message = "Message type must be first argument";
                return false;
            }

            var messageTypeArgs = Configuration.Configure<MessageTypeArgs>().CreateAndBind(args);
            if (messageTypeArgs.MessageType.IsEmpty())
            {
                message = "Could not read message type";
                return false;
            }

            _messageType = typeof (CreateUser).Assembly.GetTypes()
                .SingleOrDefault(t => t.Name.EqualsIgnoreCase(messageTypeArgs.MessageType));

            if (_messageType == null)
            {
                throw new Exception("Could not find type {0}".FormatFrom(messageTypeArgs.MessageType));
            }

            var bindToMethod = typeof (Configuration).GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Single(m => m.Name == "Configure" && !m.GetParameters().Any());
            var definition = bindToMethod.MakeGenericMethod(_messageType)
                .Invoke(null, null);
            _message = Activator.CreateInstance(_messageType);
            var bindMethod = definition.GetType().GetMethod("BindModel");
            var messageArgs = args.Skip(1).ToArray();

            try
            {
                bindMethod.Invoke(
                    definition,
                    new[] {_message, messageArgs});
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error binding {0}:", _messageType.Name);
                System.Console.WriteLine(ex.Summary());
                return false;
            }

            //var validationResults = (ICollection<ValidationResult>) new List<ValidationResult>();
            //if (!ValidationUtil.TryValidate(busMessage, out validationResults))
            //{
            //    System.Console.WriteLine("Bus message is invalid");
            //    validationResults.Each(System.Console.Error.WriteLine);
            //    return false;
            //}

            return true;
        }

        private class MessageTypeArgs
        {
            [ArgsMemberSwitch(0)]
            public string MessageType { get; set; }
        }
    }
}