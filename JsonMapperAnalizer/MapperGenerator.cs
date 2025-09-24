using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JsonMapperAnalizer;
public class MapperGenerator
{
    static void CompareJsonElements(JsonElement elementA, JsonElement elementB, string pathA, Dictionary<string, string> mappings)
    {
        switch (elementA.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var propertyA in elementA.EnumerateObject())
                {
                    string newPathA = string.IsNullOrEmpty(pathA) ? propertyA.Name : $"{pathA}.{propertyA.Name}";
                    FindMatch(propertyA.Value, elementB, newPathA, mappings);
                }
                break;

            case JsonValueKind.Array:
                for (int i = 0; i < elementA.GetArrayLength(); i++)
                {
                    string newPathA = $"{pathA}[]";
                    FindMatch(elementA[i], elementB, newPathA, mappings);
                }
                break;
        }
    }

    static void FindMatch(JsonElement elementA, JsonElement elementB, string pathA, Dictionary<string, string> mappings)
    {
        // Si ya encontramos un mapeo para esta propiedad, no seguimos
        if (mappings.ContainsKey(pathA))
            return;

        switch (elementB.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var propertyB in elementB.EnumerateObject())
                {
                    if (elementA.ValueKind == JsonValueKind.String || elementA.ValueKind == JsonValueKind.Number || elementA.ValueKind == JsonValueKind.True || elementA.ValueKind == JsonValueKind.False)
                    {
                        if (elementA.ToString() == propertyB.Value.ToString())
                        {
                            mappings[pathA] = propertyB.Name;
                            return; // 🚀 Una vez mapeado, dejamos de buscar
                        }
                    }
                    else
                    {
                        FindMatch(elementA, propertyB.Value, $"{propertyB.Name}", mappings);
                        if (mappings.ContainsKey(pathA)) return;
                    }
                }
                break;

            case JsonValueKind.Array:
                foreach (var itemB in elementB.EnumerateArray())
                {
                    FindMatch(elementA, itemB, pathA, mappings);
                    if (mappings.ContainsKey(pathA)) return; // 🚀 Detenemos cuando ya se encontró
                }
                break;
        }

        // Si elementA es objeto o array, seguimos recorriendo en A
        if (elementA.ValueKind == JsonValueKind.Object || elementA.ValueKind == JsonValueKind.Array)
        {
            CompareJsonElements(elementA, elementB, pathA, mappings);
        }
    }
}
