using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JsonMapperAnalizer;
//public static class JsonComparer
//{
//    public static Dictionary<string, string> FindMatches(
//        JsonElement objA,
//        JsonElement objB,
//        Func<string, string, bool> comparer = null)
//    {
//        var matches = new Dictionary<string, string>();
//        comparer ??= (a, b) => a == b; // default: exact match

//        ExploreProperties(objA, "A", objB, "B", matches, comparer);

//        return matches;
//    }

//    public static Dictionary<string, string> FindMatches(
//        string jsonA,
//        string jsonB,
//        string comparisonType = "exact")
//    {
//        var docA = JsonDocument.Parse(jsonA);
//        var docB = JsonDocument.Parse(jsonB);

//        var comparer = GetComparer(comparisonType);

//        return JsonComparer.FindMatches(docA.RootElement, docB.RootElement, comparer);
//    }

//    private static Func<string, string, bool> GetComparer(string type)
//    {
//        return type.ToLower() switch
//        {
//            "ignorecase" => (a, b) => a.Equals(b, StringComparison.OrdinalIgnoreCase),
//            "contains" => (a, b) => b.Contains(a, StringComparison.OrdinalIgnoreCase),
//            "numerictolerance" => (a, b) =>
//            {
//                if (decimal.TryParse(a, out var da) && decimal.TryParse(b, out var db))
//                    return Math.Abs(da - db) <= 1; // tolerancia de ±1
//                return a == b;
//            }
//            ,
//            _ => (a, b) => a == b // exact
//        };
//    }

//    private static void ExploreProperties(
//    JsonElement elemA,
//    string pathA,
//    JsonElement elemB,
//    string pathB,
//    Dictionary<string, string> matches,
//    Func<string, string, bool> comparer)
//    {
//        switch (elemA.ValueKind)
//        {
//            case JsonValueKind.Object:
//                foreach (var propA in elemA.EnumerateObject())
//                {
//                    var newPathA = $"{pathA}.{propA.Name}";
//                    ExploreProperties(propA.Value, newPathA, elemB, pathB, matches, comparer);
//                }
//                break;

//            case JsonValueKind.Array:
//                int index = 0;
//                foreach (var itemA in elemA.EnumerateArray())
//                {
//                    var newPathA = $"{pathA}[{index}]";
//                    ExploreProperties(itemA, newPathA, elemB, pathB, matches, comparer);
//                    index++;
//                }
//                break;

//            default: // 🔹 valor simple
//                ExploreAndMatch(elemA, pathA, elemB, pathB, matches, comparer);
//                break;
//        }
//    }

//    private static void ExploreAndMatch(
//        JsonElement valueA,
//        string pathA,
//        JsonElement elemB,
//        string pathB,
//        Dictionary<string, string> matches,
//        Func<string, string, bool> comparer)
//    {
//        switch (elemB.ValueKind)
//        {
//            case JsonValueKind.Object:
//                foreach (var propB in elemB.EnumerateObject())
//                {
//                    var newPathB = $"{pathB}.{propB.Name}";
//                    ExploreAndMatch(valueA, pathA, propB.Value, newPathB, matches, comparer);
//                }
//                break;

//            case JsonValueKind.Array:
//                int index = 0;
//                foreach (var itemB in elemB.EnumerateArray())
//                {
//                    var newPathB = $"{pathB}[{index}]";
//                    ExploreAndMatch(valueA, pathA, itemB, newPathB, matches, comparer);
//                    index++;
//                }
//                break;

//            default: // valor simple
//                if (IsComparable(valueA) && IsComparable(elemB))
//                {
//                    var strA = valueA.ToString();
//                    var strB = elemB.ToString();
//                    if (comparer(strA, strB))
//                    {
//                        matches[pathA] = pathB;
//                    }
//                }
//                break;
//        }
//    }

//    private static bool IsComparable(JsonElement elem)
//    {
//        return elem.ValueKind == JsonValueKind.String
//            || elem.ValueKind == JsonValueKind.Number
//            || elem.ValueKind == JsonValueKind.True
//            || elem.ValueKind == JsonValueKind.False;
//    }
//}

public static class JsonComparer
{
    public static (Dictionary<string, string> Matches, List<string> NotMatched) FindMatches(
        JsonElement objA,
        JsonElement objB,
        Func<string, string, bool> comparer = null)
    {
        var matches = new Dictionary<string, string>();
        var allPropertiesA = new List<string>();
        comparer ??= (a, b) => a == b; // default: exact match

        ExploreProperties(objA, "A", objB, "B", matches, comparer, allPropertiesA);

        // 🔹 Diferencia: propiedades de A que no tienen match en B
        var notMatched = allPropertiesA
            .Where(prop => !matches.ContainsKey(prop))
            .ToList();

        return (matches, notMatched);
    }

    public static (Dictionary<string, string> Matches, List<string> NotMatched) FindMatches(
        string jsonA,
        string jsonB,
        string comparisonType = "exact")
    {
        var docA = JsonDocument.Parse(jsonA);
        var docB = JsonDocument.Parse(jsonB);

        var comparer = GetComparer(comparisonType);

        return JsonComparer.FindMatches(docA.RootElement, docB.RootElement, comparer);
    }

    private static Func<string, string, bool> GetComparer(string type)
    {
        return type.ToLower() switch
        {
            "ignorecase" => (a, b) => a.Equals(b, StringComparison.OrdinalIgnoreCase),
            "contains" => (a, b) => b.Contains(a, StringComparison.OrdinalIgnoreCase),
            "numerictolerance" => (a, b) =>
            {
                if (decimal.TryParse(a, out var da) && decimal.TryParse(b, out var db))
                    return Math.Abs(da - db) <= 1; // tolerancia de ±1
                return a == b;
            }
            ,
            _ => (a, b) => a == b // exact
        };
    }

    private static void ExploreProperties(
        JsonElement elemA,
        string pathA,
        JsonElement elemB,
        string pathB,
        Dictionary<string, string> matches,
        Func<string, string, bool> comparer,
        List<string> allPropertiesA)
    {
        switch (elemA.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var propA in elemA.EnumerateObject())
                {
                    var newPathA = $"{pathA}.{propA.Name}";
                    ExploreProperties(propA.Value, newPathA, elemB, pathB, matches, comparer, allPropertiesA);
                }
                break;

            case JsonValueKind.Array:
                int index = 0;
                foreach (var itemA in elemA.EnumerateArray())
                {
                    var newPathA = $"{pathA}[{index}]";
                    ExploreProperties(itemA, newPathA, elemB, pathB, matches, comparer, allPropertiesA);
                    index++;
                }
                break;

            default: // valor simple
                allPropertiesA.Add(pathA); // 🔹 registramos que esta propiedad de A fue examinada
                ExploreAndMatch(elemA, pathA, elemB, pathB, matches, comparer);
                break;
        }
    }

    private static void ExploreAndMatch(
        JsonElement valueA,
        string pathA,
        JsonElement elemB,
        string pathB,
        Dictionary<string, string> matches,
        Func<string, string, bool> comparer)
    {
        switch (elemB.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var propB in elemB.EnumerateObject())
                {
                    var newPathB = $"{pathB}.{propB.Name}";
                    ExploreAndMatch(valueA, pathA, propB.Value, newPathB, matches, comparer);
                }
                break;

            case JsonValueKind.Array:
                int index = 0;
                foreach (var itemB in elemB.EnumerateArray())
                {
                    var newPathB = $"{pathB}[{index}]";
                    ExploreAndMatch(valueA, pathA, itemB, newPathB, matches, comparer);
                    index++;
                }
                break;

            default: // valor simple
                if (IsComparable(valueA) && IsComparable(elemB))
                {
                    var strA = valueA.ToString();
                    var strB = elemB.ToString();
                    if (comparer(strA, strB))
                    {
                        matches[pathA] = pathB;
                    }
                }
                break;
        }
    }

    private static bool IsComparable(JsonElement elem)
    {
        return elem.ValueKind == JsonValueKind.String
            || elem.ValueKind == JsonValueKind.Number
            || elem.ValueKind == JsonValueKind.True
            || elem.ValueKind == JsonValueKind.False;
    }
}