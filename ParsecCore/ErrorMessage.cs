namespace ParsecCore
{
    public abstract class ErrorMessage
    {
    }

    public sealed class StringErrorMessage : ErrorMessage
    {
        public StringErrorMessage(string message)
        {
            Message = message;
        }

        public string Message { get; set; }

        public override string ToString()
        {
            return Message;
        }
    }

    public sealed class ExpectEncouterErrorMessage : ErrorMessage
    {
        public ExpectEncouterErrorMessage(string expected, string encoutered)
        {
            Expected = expected;
            Encoutered = encoutered;
        }

        public override string ToString()
        {
            return $"Unexpected {Encoutered} encountered, {Expected} expected";
        }

        public string Expected { get; set; }
        public string Encoutered { get; set; }
    }
}
