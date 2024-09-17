using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;

namespace XMLDoc2Markdown.Utils;

internal class AssemblyLoadContext : System.Runtime.Loader.AssemblyLoadContext
{
    private readonly string _pluginPath;
    private readonly AssemblyDependencyResolver _resolver;

    public AssemblyLoadContext(string pluginPath, bool isCollectible) : base(Guid.NewGuid().ToString(), isCollectible)
    {
        this._pluginPath = pluginPath;
        this._resolver = new AssemblyDependencyResolver(pluginPath);
        
        this.Resolving += this.OnResolving;
        
        
    }

    private Assembly OnResolving(System.Runtime.Loader.AssemblyLoadContext context, AssemblyName name)
    {
            string[] possiblePaths = new[]
            {
                Path.GetDirectoryName(this._pluginPath),
                //@"C:\Program Files\dotnet\shared\Microsoft.AspNetCore.App\8.0.8",
                GetAspNetCoreSharedPath(),
                AppDomain.CurrentDomain.BaseDirectory
            };

            foreach (string searchPath in possiblePaths)
            {
                string filePath = Path.Combine(searchPath, name.Name + ".dll");
                Console.WriteLine($"Attempting to load: {filePath}");

                if (File.Exists(filePath))
                {
                    return context.LoadFromAssemblyPath(filePath);
                }
            }
            

            Console.WriteLine($"Unable to find assembly: {name.FullName}");
            return null;
    }

    protected override Assembly Load(AssemblyName assemblyName)
    {
        string assemblyPath = this._resolver.ResolveAssemblyToPath(assemblyName);
        if (assemblyPath != null)
        {
            return this.LoadFromAssemblyPath(assemblyPath);
        }

        return null;
    }

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        string libraryPath = this._resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        if (libraryPath != null)
        {
            return this.LoadUnmanagedDllFromPath(libraryPath);
        }

        return IntPtr.Zero;
    }
    
    static string GetDotNetRootPath()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),"dotnet");
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || 
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return "/usr/share/dotnet";
        }

        throw new PlatformNotSupportedException("Unsupported operating system.");
    }
    private static string GetAspNetCoreSharedPath()
    {
        string dotnetPath = GetDotNetRootPath();
        string aspNetPath = Path.Combine(dotnetPath, "shared", "Microsoft.AspNetCore.App");
        string fullPath = Path.Combine(aspNetPath, GetVersionDirectoryName(aspNetPath));
        return fullPath;
    }

    private static string GetVersionDirectoryName(string aspNetPath)
    {
        string[] versions = Directory.GetDirectories(aspNetPath, "8.0.*");
        if (versions.Length == 0)
        {
            throw new DirectoryNotFoundException("No directory found starting with 8.0.");
        }

        Array.Sort(versions);
        
        return versions[^1];
 
    }
}
