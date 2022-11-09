using System.Runtime.Serialization;

namespace Domain.Exceptions;

[Serializable]
internal class TimesOverlapException : Exception
{
    public TimesOverlapException()
    {
    }

    public TimesOverlapException(string? message) : base(message)
    {
    }

    public TimesOverlapException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected TimesOverlapException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}