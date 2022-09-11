using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace TestCodeGen
{
    [Generator]
    public partial class BackedPropertyGenerator : IIncrementalGenerator
    {
        private record PropertyAndClassInfo(string PropertyName, string FullTypeName, string ContainingClassName, string NameSpace);

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterPostInitializationOutput(
                static postInitCtx =>
                {
                    postInitCtx.AddSource(
                        "TestLib.BackedPropertyAttribute.g.cs",
                        SourceText.From(@"
namespace TestLib
{
    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    internal class BackedPropertyAttribute : System.Attribute {
        public BackedPropertyAttribute() : base() { }
    }
}", Encoding.UTF8)
                    );
                }
            );

            var namesAndClasses =
                context.SyntaxProvider
                    .ForAttributeWithMetadataName(
                        "TestLib.BackedPropertyAttribute",
                        static (node, _) => node is PropertyDeclarationSyntax { AttributeLists.Count: > 0 },
                        static (ctx, _) =>
                        {
                            var propDecNode = (PropertyDeclarationSyntax)ctx.TargetNode;

                            var propertyName = propDecNode.Identifier.Text;
                            var propertyTypeName = propDecNode.Type.ToString();

                            var propertySymbol = ctx.TargetSymbol;
                            var className = propertySymbol.ContainingType.Name;

                            var nameSpace = propertySymbol.ContainingNamespace.Name;

                            return new PropertyAndClassInfo(propertyName, propertyTypeName, className, nameSpace);
                        }
                    );

            context.RegisterSourceOutput(
                namesAndClasses,
                static (SourceProductionContext sourceProdContext, PropertyAndClassInfo propInfo) =>
                {
                    var (propName, propType, className, nameSpace) = propInfo;

                    sourceProdContext.AddSource(
                        className + "." + propName + ".g.cs",
                        SourceText.From($@"
namespace {nameSpace}
{{
    public partial class {className} {{
        private {propType} _{propName};
    }}
}}", Encoding.UTF8)
                        );
                }
            );
        }
    }
}