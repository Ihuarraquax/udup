using System.Text.Json;
using Udup.Core;

namespace Udup.CLI;

internal class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("  udup <solution-path>");
            return;
        }

        var solutionPath = args[0];
        var udupFilePath = args[1];
        var gatherer = new Gatherer(solutionPath);
        await using StreamWriter outputFile = new StreamWriter(Path.Combine(udupFilePath, "Udup.json"));
        await outputFile.WriteAsync(JsonSerializer.Serialize(await gatherer.Gather()));
    }
}