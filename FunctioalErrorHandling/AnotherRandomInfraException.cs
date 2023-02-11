// See https://aka.ms/new-console-template for more information
using System.Runtime.Serialization;

[Serializable]
internal class AnotherRandomException : Exception
{
    public AnotherRandomException()
    {
    }

    public AnotherRandomException(string? message) : base(message)
    {
    }

    public AnotherRandomException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected AnotherRandomException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}