# XMLDoc2Markdown

Tool to generate markdown from C# XML documentation. This is the Meshmakers fork of
[XMLDoc2Markdown](https://github.com/charlesdevandiere/xmldoc2md), targeting .NET 10 and shipped as
the `Meshmakers.XMLDoc2Markdown` NuGet package with the `mmxmldoc2md` command.

## How to use

### Install tool

```shell
dotnet tool install -g Meshmakers.XMLDoc2Markdown
```

### Generate documentation

```shell
mmxmldoc2md <DLL_SOURCE_PATH> <OUTPUT_DIRECTORY>
```

The XML documentation file (`<assembly>.xml`) must sit next to the DLL, so build the assembly with
`<GenerateDocumentationFile>` (or `<DocumentationFile>`) enabled.

#### Example

```shell
mmxmldoc2md Sample.dll docs
```

### Display command line help

```shell
mmxmldoc2md -h
```

## Output layout

Public types are grouped by namespace. Each namespace below the assembly's root namespace becomes a
sub folder with a Docusaurus `_category_.json`; the assembly's root namespace is written to the
output root.

Types declared in the **global namespace** have no namespace at all. They are listed under the
`<global namespace>` heading and their pages are written to the output root. This matters for
executables: since .NET 10 the compiler emits the entry point wrapper of a top-level statement app
as `public partial class Program` in the global namespace, so every such assembly contains at least
one global-namespace type.

Documenting a host executable is usually pointless - it typically has no public API besides that
generated `Program` class. Point the tool at the libraries instead.

See the complete documentation of the upstream project
[here](https://charlesdevandiere.github.io/xmldoc2md/).
