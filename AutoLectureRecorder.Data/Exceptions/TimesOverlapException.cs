namespace AutoLectureRecorder.Data.Exceptions;

public class TimesOverlapException : Exception
{
    public TimesOverlapException()
    {
    }

    public TimesOverlapException(string message)
        : base(message)
    {
    }

    public TimesOverlapException(string message, Exception inner)
        : base(message, inner)
    {
    }
}