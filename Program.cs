using System.CommandLine;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;

internal class Program
{
    private static int Main(string[] args)
    {
        //add feature, so the data is read from stdin, and not from file directly
        bool isPiped = Console.IsInputRedirected;
        
        StringBuilder input = new();
        string s;
        while ((s = Console.ReadLine()) != null)
        {
            input.Append(s);
        }

        var rootCommand = new RootCommand("Get details about file");

        var fileBytesOption = new Option<bool>("-c", "Get the number of bytes in the file");
        var fileLinesOption = new Option<bool>("-l", "Get the number of lines in the file");
        var fileWordsOption = new Option<bool>("-w", "Get the number of words in the file");
        var fileCharactersOption = new Option<bool>("-m", "Get the number of characters in the file");

        var fileNameArgument = new Argument<string>("file", "The file Name");

        fileNameArgument.AddValidator(symbolResult => {
            if(symbolResult.Tokens.Count == 0){
                symbolResult.ErrorMessage = "File name is required";
            }

            if(!File.Exists(symbolResult.Tokens.Single().Value)){
                symbolResult.ErrorMessage = "File does not exist";
            }
        });

        rootCommand.AddOption(fileBytesOption);
        rootCommand.AddOption(fileLinesOption);
        rootCommand.AddOption(fileWordsOption);
        rootCommand.AddOption(fileCharactersOption);

        rootCommand.AddArgument(fileNameArgument);

        rootCommand.SetHandler((bool fileLinesOptionValue, bool fileBytesOptionValue, bool fileWordsOptionValue, bool fileCharactersOptionValue, string fileName) =>
        {
            bool noOptionSelected = !fileLinesOptionValue && !fileBytesOptionValue && !fileWordsOptionValue && !fileCharactersOptionValue;
            if(noOptionSelected){
                var fileBytesCount = File.ReadAllBytes(fileName).Count();
                var fileLinesCount = File.ReadAllText(fileName).Count(character => character == '\n');
                string whitespaceCharacters = @"\s+";
                var fileWordsCount = Regex.Split(File.ReadAllText(fileName), whitespaceCharacters).Count();

                Console.WriteLine($"{fileLinesCount} {fileWordsCount} {fileBytesCount} {fileName}");
                return;
            }
            
            if(fileBytesOptionValue){
                var fileBytesCount = File.ReadAllBytes(fileName).Count();
                Console.WriteLine($"{fileBytesCount} {fileName}");
            }

            if(fileLinesOptionValue){
                // print new line counts, might give incorrect if directly using c# inbuilt function
                // as last line may not have new line, so 'wrongly' included in inbuilt function
                var fileLinesCount = File.ReadAllText(fileName).Count(character => character == '\n');
                Console.WriteLine($"{fileLinesCount} {fileName}");
            }
            
            if(fileWordsOptionValue){
                // split using all whitespace characters
                string whitespaceCharacters = @"\s+";
                var fileWordsCount = Regex.Split(File.ReadAllText(fileName), whitespaceCharacters).Count();
                Console.WriteLine($"{fileWordsCount} {fileName}");
            }
            
            if(fileCharactersOptionValue){
                var fileCharactersCount = File.ReadAllText(fileName).Count();
                Console.WriteLine($"{fileCharactersCount} {fileName}");
            }

        }, fileLinesOption, fileBytesOption, fileWordsOption, fileCharactersOption, fileNameArgument);

        return rootCommand.Invoke(args);
    }
}