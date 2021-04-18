using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using DTOGenerator.Attributes;
using DTOGenerator.Extension;

namespace DTOGenerator
{
    [Generator]
    public class Generator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
/*#if DEBUG
            if (!Debugger.IsAttached) 
            { 
                Debugger.Launch(); 
            }
 #endif*/
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var nodes = context.Compilation.SyntaxTrees
                .SelectMany(s => s.GetRoot().DescendantNodes());

            var classDeclarationSyntaxes = nodes
                .Select(s => s as ClassDeclarationSyntax)
                .Where(s => s != null);
            
            foreach (var classDeclarationSyntax in classDeclarationSyntaxes)
            {
                var attributes = classDeclarationSyntax.AttributeLists
                    .SelectMany(a => a.Attributes);

                var hasGenerateAttribute = attributes.FirstOrDefault(a => a.Name.ToString() == nameof(GenerateDto));
                if (hasGenerateAttribute == null)
                {
                    continue;
                }

                var originalName = classDeclarationSyntax.Identifier.ToString();
                var arguments = GetAttributeArguments(hasGenerateAttribute);
                var useDynamic = ExtractBool(arguments);
                if (!arguments.Any())
                {
                    arguments.Add($"{originalName}DTO");
                }

                foreach (var className in arguments)
                {
                    var ignoredProperties = GetIgnoredProperties(classDeclarationSyntax, className);
                    var getClassWithoutIgnoredProperties =
                        classDeclarationSyntax.RemoveNodes(ignoredProperties, SyntaxRemoveOptions.KeepEndOfLine);
                    if (getClassWithoutIgnoredProperties == null)
                    {
                        continue;
                    }
                
                    var properties = getClassWithoutIgnoredProperties.ChildNodes()
                        .Select(s => s as PropertyDeclarationSyntax)
                        .Where(s => s != null);
                    var directives = classDeclarationSyntax.SyntaxTree.GetRoot().DescendantNodes()
                        .Select(s => s as UsingDirectiveSyntax)
                        .Where(u => u != null);
                
                    var namespaces = classDeclarationSyntax.SyntaxTree.GetRoot().DescendantNodes()
                        .Select(s => s as NamespaceDeclarationSyntax).Where(u => u != null);

                    var generatedClass = GenerateClass(originalName, className, namespaces, directives, properties, useDynamic);
                    context.AddSource(className, SourceText.From(generatedClass, Encoding.UTF8));
                }
            }
        }

        private IEnumerable<SyntaxNode> GetIgnoredProperties(ClassDeclarationSyntax declaration, string className)
        {
            var nodes = declaration.ChildNodes()
                .OfType<PropertyDeclarationSyntax>()
                .Where(p => p.AttributeLists.SelectMany(a => a.Attributes)
                    .Any(a => a.Name.ToString() == nameof(ExcludeProperty) && 
                              (!GetAttributeArguments(a).Any() || GetAttributeArguments(a).Contains(className))));
            
            return nodes;
        }

        private AttributeSyntax GetUsingExistingAttribute(PropertyDeclarationSyntax property)
        {
            return property.AttributeLists
                .SelectMany(a => a.Attributes)
                .FirstOrDefault(a => a.Name.ToString() == nameof(UseExistingDto));
        }

        private List<string> GetAttributeArguments(AttributeSyntax attribute)
        {
            if (attribute.ArgumentList == null)
            {
                return new List<string>();
            }
            
            var arguments = attribute.ArgumentList.Arguments
                .Select(s => s.NormalizeWhitespace().ToFullString().Replace("\"", "")).ToList();

            return arguments;
        }

        private bool ExtractBool(List<string> arguments)
        {
            if (arguments.Any() && bool.TryParse(arguments.First(), out var parsedValue))
            {
                arguments.RemoveAt(0);
                return parsedValue;
            }
            return false;
        }

        private string GetUsingArgument(AttributeSyntax usingSyntax, string className)
        {
            var argument = GetAttributeArguments(usingSyntax)
                .Where(u => u.StartsWith(className) && u.Contains(" > "));
            return argument.FirstOrDefault()?.Split(" > ")[1];
        }
        
        private string GenerateClass(string originalName, string className, IEnumerable<NamespaceDeclarationSyntax> namespaces, IEnumerable<UsingDirectiveSyntax> usingDirectives, 
            IEnumerable<PropertyDeclarationSyntax> properties, bool useDynamic)
        {
            var classBuilder = new StringBuilder();
            
            classBuilder.AppendLine("using System.Dynamic;");
            classBuilder.AppendLine("using System.Collections;");
            classBuilder.AppendLine("using SourceDto;");
            foreach (var namespaceDirective in namespaces)
            {
                classBuilder.AppendLine($"using {namespaceDirective.Name.ToString()};");
            }

            foreach (var usingDirective in usingDirectives)
            {
                classBuilder.AppendLine(usingDirective.ToString());
            }
            
            classBuilder.AppendLine($@"
namespace SourceDto
{{
    public class {className}
    {{");
            
            foreach (var property in properties)
            {
                var useExisting = GetUsingExistingAttribute(property);
                if (useExisting == null)
                {
                    classBuilder.AppendLine($"\t\t{property}");
                }
                else
                {
                    var replace = GetUsingArgument(useExisting, className);
                    var dto = property.ToString().GetLastPart("]")
                        .ReplaceFirst(property.Type.ToString(), replace == null ? $"{property.Type}DTO" : replace);

                    classBuilder.AppendLine($"\t\t{dto}");
                }
            }

            var param = useDynamic ? "dynamic" : originalName;
            classBuilder.AppendLine($@"
        public {className} Map({param} instance)
        {{");
            foreach (var property in properties)
            {
                var useExisting = GetUsingExistingAttribute(property);
                if (useExisting == null)
                {
                    classBuilder.AppendLine($"\t\t\t{property.Identifier} = instance.{property.Identifier};");
                }
                else
                {
                    var replace = GetUsingArgument(useExisting, className);
                    var name = replace == null ? $"{property.Type}DTO" : replace;
                    classBuilder.AppendLine($"\t\t\t{property.Identifier} = new {name}().Map(instance.{property.Identifier});");
                }
            }
            
            classBuilder.AppendLine("\t\t\treturn this;");
            
            classBuilder.AppendLine("\t\t}");
            classBuilder.AppendLine("    }");
            return classBuilder.AppendLine("}").ToString();
        }
    }
}