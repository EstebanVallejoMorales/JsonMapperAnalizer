using System.Text.Json;
using System.Text.RegularExpressions;

namespace JsonMapperAnalizer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string jsonAPath = Path.Combine("JsonFiles", "JsonA", "PnrRetrieve_ADX.json");
            string jsonBPath = Path.Combine("JsonFiles", "JsonB", "PnrRetrieve_ATG.json");

            // 🔹 Leer el contenido de los archivos
            string jsonA = File.ReadAllText(jsonAPath);
            string jsonB = File.ReadAllText(jsonBPath);

            // Comparación exacta
            var(matches, notMatched)  = JsonComparer.FindMatches(jsonA, jsonB, "exact");
            PrintResults("Exacta - matches", matches);

            Console.WriteLine("\n--- Not Matched ---");
            foreach (var nm in notMatched)
                Console.WriteLine(nm);

            /*
            // Comparación ignorando mayúsculas
            var ignoreCaseMatches = JsonComparer.FindMatches(jsonA, jsonB, "ignorecase");
            PrintResults("IgnoreCase", ignoreCaseMatches);

            // Comparación contains
            var containsMatches = JsonComparer.FindMatches(jsonA, jsonB, "contains");
            PrintResults("Contains", containsMatches);

            // Comparación numérica tolerante
            string jsonC = @"{ ""Valor"": 100 }";
            string jsonD = @"{ ""ValorX"": 101 }";

            var numericMatches = JsonComparer.FindMatches(jsonC, jsonD, "numerictolerance");
            PrintResults("NumericTolerance", numericMatches);*/
        }

        static void PrintResults(string title, Dictionary<string, string> matches)
        {
            Console.WriteLine($"--- {title} ---");
            foreach (var match in matches)
            {
                Console.WriteLine($"{match.Key} -> {match.Value}");
            }
            Console.WriteLine();
        }
    }
}
