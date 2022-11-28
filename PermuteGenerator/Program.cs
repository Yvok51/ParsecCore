namespace PermuteGenerator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = args[0];
            string number = args[1];

            if (int.TryParse(number, out int numberOfParsers))
            {
                // TODO: print error
            }

            string permuteSource = PermuteSourceGenerator.GeneratePermuteSourceDocument(PermuteSourceGenerator.PREMADE_PERMUTE_FUNCTIONS, numberOfParsers);
            string partialSource = PermuteSourceGenerator.GeneratePartialSourceDocument(PermuteSourceGenerator.PREMADE_PARTIAL_FUNCTIONS, numberOfParsers);


        }
    }
}