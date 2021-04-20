using DeeDee.Builders;
using DeeDee.Builders.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace DeeDee
{
    [Generator]
    internal class SourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {

            if (context.SyntaxReceiver is not SyntaxReceiver rec) return;

            if(rec.ClassesWithBases.Count == 0) return;

            var asmName = context.Compilation.AssemblyName;
            var ns = asmName == null ? string.Empty : $"{asmName}.";

            var frugalDictionary = FrugalDictionaryBuilder.Build(ns);
            var ipipelineAction = IPipelineActionBuilder.Build(ns);
            var irequest = IRequestBuilder.Build(ns);
            var nextDelegate = NextDelegateBuilder.Build(ns);
            var pipelineContext = PipelineContextBuilder.Build(ns);
            var serviceProviderDelegate = ServiceProviderDelegateBuilder.Build(ns);
            var stepAttribute = StepAttributeBuilder.Build(ns);
            var throwHelper = ThrowHelperBuilder.Build(ns);

            context.AddSource("FrugalDictionary.cs", frugalDictionary);
            context.AddSource("IPipelineAction.cs", ipipelineAction);
            context.AddSource("IRequest.cs", irequest);
            context.AddSource("NextDelegate.cs", nextDelegate);
            context.AddSource("PipelineContext.cs", pipelineContext);
            context.AddSource("ServiceProviderDelegate.cs", serviceProviderDelegate);
            context.AddSource("StepAttribute.cs", stepAttribute);
            context.AddSource("ThrowHelper.cs", throwHelper);

            var options = (context.Compilation as CSharpCompilation)!.SyntaxTrees[0].Options as CSharpParseOptions;

            var compilation = context.Compilation.AddSyntaxTrees
            (
                CSharpSyntaxTree.ParseText(ipipelineAction, options)
            );

            var irequestsOfT = new List<(string RequestClassName, string ResponseClassName, bool isAsync)>();

            var irequests = new List<(string RequestClassName, bool isAsync)>();

            foreach (var cds in rec.ClassesWithBases)
            {
                var model = compilation.GetSemanticModel(cds.SyntaxTree);
                var type = model.GetDeclaredSymbol(cds);
                if (type == null) continue;
                if (type.IsAbstract) continue;

                foreach (var i in type.AllInterfaces)
                {
                    if (i.ContainingNamespace.ToDisplayString() != $"{ns}DeeDee.Models") continue;

                    switch (i.Name)
                    {
                        case "IPipelineActionAsync":
                            if (i.TypeArguments.Length == 1)
                                irequests.Add(new(i.TypeArguments[0].ToDisplayString(), true));
                            if (i.TypeArguments.Length == 2)
                                irequestsOfT.Add(new(i.TypeArguments[0].ToDisplayString(), i.TypeArguments[1].ToDisplayString(), true));
                            break;
                        case "IPipelineAction":
                            if (i.TypeArguments.Length == 1)
                                irequests.Add(new(i.TypeArguments[0].ToDisplayString(), false));
                            if (i.TypeArguments.Length == 2)
                                irequestsOfT.Add(new(i.TypeArguments[0].ToDisplayString(), i.TypeArguments[1].ToDisplayString(), false));
                            break;
                    }
                }
            }


            var distinctIrequest = irequests.Distinct().ToList();
            var distinctIrequestOfT = irequestsOfT.Distinct().ToList();

            var iocExtensions = IocExtensionsBuilder.Build(ns);

            var idispatcher = DispatcherInterfaceBuilder.Build(ns, distinctIrequest, distinctIrequestOfT);

            var dispatcher = DispatcherClassBuilder.Build(ns, distinctIrequest, distinctIrequestOfT);


            context.AddSource("IocExtensions.cs", iocExtensions);
            context.AddSource("IDispatcher.cs", FormatCode(idispatcher));
            context.AddSource("Dispatcher.cs", FormatCode(dispatcher));

        }


        private static string FormatCode(string source)
        {
            return SyntaxFactory.ParseCompilationUnit(source).NormalizeWhitespace().ToString();
        }


    }


    internal class SyntaxReceiver : ISyntaxReceiver
    {
        public List<ClassDeclarationSyntax> ClassesWithBases { get; } = new();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ClassDeclarationSyntax cds)
            {
                if (cds.BaseList == null) return;

                ClassesWithBases.Add(cds);
            }
        }
    }
}