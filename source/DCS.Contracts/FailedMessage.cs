namespace DCS.Contracts
{
    /// <summary>
    ///     For application-level notification of message failure (as opposed
    ///     to bus-level).
    /// </summary>
    public class FailedMessage<TMessage> : FailedMessage
    {
        public TMessage MessageTyped
        {
            get { return (TMessage) Message; }
        }
    }

    public class FailedMessage
    {
        public object Message { get; set; }

        public static FailedMessage<T> Create<T>(T message)
        {
            return new FailedMessage<T> {Message = message};
        }
    }
}