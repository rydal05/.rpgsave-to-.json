using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using LZStringCSharp;

namespace ConsoleApplication
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var path = "../../../input/file1.rpgsave"; // todo: file selection prompt instead of this
            Console.WriteLine("Path located: " + File.Exists(path)); //returns true
            if (!File.Exists(path))
            {
                Console.WriteLine("Could not find file :(");
                return;
            }
            var utf8 = new UTF8Encoding(false); //object for handling utf8 encoding

            await using var inputFile = File.Open(path, new FileStreamOptions //File I/O accessor with settings
            {
                Mode = FileMode.Open,
                Access = FileAccess.Read,
                Share = FileShare.Read,
                Options = FileOptions.Asynchronous | FileOptions.SequentialScan
            });

            using var reader = new StreamReader(inputFile, utf8);

            var inputAsString = await reader.ReadToEndAsync().ConfigureAwait(false);

            string output, outputPath; 

            outputPath = path + ".json"; // todo: file save selection prompt instead of this
            output = LZString.DecompressFromBase64(inputAsString);
            using var json = JsonDocument.Parse(output);
            output = JsonSerializer.Serialize(json, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            });

            await using var outputOpen = File.Open(outputPath, new FileStreamOptions
            {
                Mode = FileMode.Create,
                Access = FileAccess.Write,
                Share = FileShare.Read,
                Options = FileOptions.Asynchronous | FileOptions.SequentialScan
            });

            await using var writer = new StreamWriter(outputOpen, utf8); 
            await writer.WriteAsync(output).ConfigureAwait(false); // todo: reverse operation (.json to .rpgsave)
            //memory cleanup
            await writer.FlushAsync().ConfigureAwait(false);
            await outputOpen.FlushAsync().ConfigureAwait(false);

            return;
        }
    }
}
