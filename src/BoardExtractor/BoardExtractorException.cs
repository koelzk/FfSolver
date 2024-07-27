namespace BoardExtractor;

public class BoardExtractorException : Exception
{
    public BoardExtractorException()
    {
        // Nothing to do here.
    }

    public BoardExtractorException(string? message) : base(message)
    {
        // Nothing to do here.
    }

    public BoardExtractorException(string? message, Exception? innerException) : base(message, innerException)
    {
        // Nothing to do here.
    }
}