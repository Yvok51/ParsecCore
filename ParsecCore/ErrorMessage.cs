namespace ParsecCore
{
    internal struct GenericMessage
    {
        public GenericMessage(string message)
        {
            Message = message;
        }

        public string Message { get; set; }

        public override string ToString()
        {
            return Message;
        }
    }

    internal  struct ExpectedMessage
    {
        public ExpectedMessage(string expected)
        {
            Expected = expected;
        }

        public override string ToString()
        {
            return $"Expected {Expected}";
        }

        public string Expected { get; set; }
    }

    internal struct EncounteredMessage
    {
        public EncounteredMessage(string encoutered)
        {
            Encoutered = encoutered;
        }

        public override string ToString()
        {
            return $"Unexpected {Encoutered} encountered";
        }

        public string Encoutered { get; set; }
    }
}
