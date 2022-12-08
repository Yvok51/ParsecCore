namespace PermuteGenerator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1 && (args[0] == "--help" || args[0] == "-h"))
            {
                PrintHelp(Console.Out);
                return;
            }

            if (args.Length != 2)
            {
                Console.WriteLine("Two arguments expected");
                PrintHelp(Console.Out);
                return;
            }

            string path = args[0];
            string number = args[1];

            if (int.TryParse(number, out int numberOfParsers) || numberOfParsers <= 0)
            {
                Console.WriteLine("The NUMBER argument must be a positive integer");
                PrintHelp(Console.Out);
                return;
            }

            string permuteSource = PermuteSourceGenerator.GeneratePermuteSourceDocument(PermuteSourceGenerator.PREMADE_PERMUTE_FUNCTIONS, numberOfParsers);
            string partialSource = PermuteSourceGenerator.GeneratePartialSourceDocument(PermuteSourceGenerator.PREMADE_PARTIAL_FUNCTIONS, numberOfParsers);

            const string permuteFilename = "'Permute.g.cs";
            const string partialFilename = "'Partial.g.cs";

            if (string.IsNullOrEmpty(permuteSource) && string.IsNullOrEmpty(partialSource))
            {
                Console.WriteLine("No source generated");
                return;
            }

            try 
            {
                using (var permuteWriter = new StreamWriter(Path.Combine(path, permuteFilename))) 
                using (var partialWriter = new StreamWriter(Path.Combine(path, partialFilename)))
                {
                    permuteWriter.WriteLine(permuteSource);
                    partialWriter.WriteLine(partialSource);
                }
            }
            catch (Exception ex) when (ex is IOException)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        static void PrintHelp(TextWriter text)
        {
            text.WriteLine(@"
Usage: PermuteGenerator PATH NUMBER
Generate additional Permute functions for permutation parser in ParsecCore.

Two files will be generated 'Permute.g.cs' and 'Partial.g.cs'. The file
'Permute.g.cs' will contain the generated Permute functions and 'Partial.g.cs'
will contain helper function needed in the implementation of Permute.

The PATH argument specifies the directory the files will be generated in.
The directory must already exist.

The NUMBER argument is a positive integer specifies up to how many parsers 
the Permute methods should support. If the NUMBER is less than the number
of already supported parsers in the Permute method, then no files are 
generated.
");

        }
    }
}