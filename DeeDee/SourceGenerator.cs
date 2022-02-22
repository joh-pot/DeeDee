using DeeDee.Builders;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using INamespaceSymbol = Microsoft.CodeAnalysis.INamespaceSymbol;

namespace DeeDee
{
    [Generator]
    internal class SourceGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {

            var invocations = context.SyntaxProvider.CreateSyntaxProvider
            (
                predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                transform: static (ctx, _) => (InvocationExpressionSyntax)ctx.Node
            ).Where(static m => m is not null);

            var compilationAndInvocations = context.CompilationProvider.Combine(invocations.Collect());

            context.RegisterSourceOutput
            (
                compilationAndInvocations,
                static (spc, source) => Execute(source.Left, source.Right, spc)
            );
        }

        private static bool IsSyntaxTargetForGeneration(SyntaxNode node)
        {
            return node is InvocationExpressionSyntax
            {
                Expression: MemberAccessExpressionSyntax { Name.Identifier.ValueText: "AddDispatcher" }
            };
        }

        private static void Execute(Compilation compilation, ImmutableArray<InvocationExpressionSyntax> targets, SourceProductionContext context)
        {
            if (targets.IsDefaultOrEmpty)
            {
                return;
            }

            Execute(compilation, context);
        }

        private static void Execute(Compilation compilation, SourceProductionContext context)
        {
            var ns = string.IsNullOrWhiteSpace(compilation.AssemblyName)
                ? string.Empty
                : $"{compilation.AssemblyName}.";

            var iocExtensions = IocExtensionsBuilder.Build(ns);

            context.AddSource("IocExtensions.cs", iocExtensions);

            var irequestsOfT = new List<(string RequestClassName, string ResponseClassName, bool isAsync)>();

            var irequests = new List<(string RequestClassName, bool isAsync)>();

            foreach (var namespaceOrTypeSymbol in compilation.GlobalNamespace.GetMembers())
            {
                FlattenNamespaces(namespaceOrTypeSymbol, irequestsOfT, irequests);
            }

            var distinctIrequest = irequests.Distinct().ToList();

            var distinctIrequestOfT = irequestsOfT.Distinct().ToList();

            var idispatcher = DispatcherInterfaceBuilder.Build(ns, distinctIrequest, distinctIrequestOfT);

            var dispatcher = DispatcherClassBuilder.Build(ns, distinctIrequest, distinctIrequestOfT);

            context.AddSource("IDispatcher.cs", FormatCode(idispatcher));

            context.AddSource("Dispatcher.cs", FormatCode(dispatcher));

        }

        private static void FlattenNamespaces
        (
            ISymbol namespaceOrTypeSymbol,
            List<(string RequestClassName, string ResponseClassName, bool isAsync)> irequestsOfT,
            List<(string RequestClassName, bool isAsync)> irequests
        )
        {
            if (namespaceOrTypeSymbol.Name is "System" or "Microsoft" or "<Module>") return;

            switch (namespaceOrTypeSymbol)
            {
                case INamespaceSymbol nsSymbol:
                    foreach (var x in nsSymbol.GetMembers())
                    {
                        FlattenNamespaces(x, irequestsOfT, irequests);
                    }
                    break;
                case ITypeSymbol tSymbol:
                    if (tSymbol.IsAbstract)
                    {
                        break;
                    }
                    foreach (var @interface in tSymbol.AllInterfaces)
                    {
                        if (@interface.ContainingNamespace.ToDisplayString() != "DeeDee.Models")
                            continue;
                        ;
                        switch (@interface.Name)
                        {
                            case "IPipelineActionAsync":
                                if (@interface.TypeArguments.Length == 1)
                                    irequests.Add(new(@interface.TypeArguments[0].ToDisplayString(), true));
                                if (@interface.TypeArguments.Length == 2)
                                    irequestsOfT.Add(new(@interface.TypeArguments[0].ToDisplayString(), @interface.TypeArguments[1].ToDisplayString(), true));
                                break;
                            case "IPipelineAction":
                                if (@interface.TypeArguments.Length == 1)
                                    irequests.Add(new(@interface.TypeArguments[0].ToDisplayString(), false));
                                if (@interface.TypeArguments.Length == 2)
                                    irequestsOfT.Add(new(@interface.TypeArguments[0].ToDisplayString(), @interface.TypeArguments[1].ToDisplayString(), false));
                                break;
                        }
                    }
                    break;
            }
        }

        private static string FormatCode(string source)
        {
            return SyntaxFactory.ParseCompilationUnit(source).NormalizeWhitespace().ToString();
        }
    }
}