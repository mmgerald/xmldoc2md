using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace XMLDoc2Markdown.Utils;

internal static class AssemblyExtensions
{
    internal static IEnumerable<string> GetDeclaredNamespaces(this Assembly assembly)
    {
        return assembly.GetTypes().Select(type => type.Namespace).Distinct();
    }

    internal static string GetRootNamespace(this Assembly assembly)
    {
        var namespaces = assembly.GetTypes()
            .Where(t => t.Namespace != null)
            .Select(t => t.Namespace)
            .GroupBy(ns => ns);

        return namespaces.First().Key;
    }
    
    internal static string GetSubNamespace(this Assembly assembly, string @namespace)
    {
        RequiredArgument.NotNull(assembly, nameof(assembly));

        var rootNamespace = assembly.GetRootNamespace();
        var subNamespace = @namespace.Replace(rootNamespace, "");
        if (string.IsNullOrWhiteSpace(subNamespace))
        {
            return null;
        }

        return subNamespace.Substring(0);
    }
    
    internal static IEnumerable<string> GetSubNamespaceParts(this Assembly assembly, string @namespace)
    {
        RequiredArgument.NotNull(assembly, nameof(assembly));

        var rootNamespace = assembly.GetRootNamespace();
        var subNamespace = @namespace.Replace(rootNamespace, "");
        if (string.IsNullOrWhiteSpace(subNamespace))
        {
            return new List<string>();
        }

        return subNamespace.Substring(0).Split(".");
    }
    
    internal static string GetRelativeFolderPath(this Assembly assembly, string @namespace)
    {
        RequiredArgument.NotNull(assembly, nameof(assembly));

        return assembly.GetSubNamespace(@namespace)?.Replace(".", "/").ToLower();
    }
}
