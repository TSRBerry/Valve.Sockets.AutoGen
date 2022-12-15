using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Valve.Sockets.Tasks.SyntaxWalker
{
    class ArrayTypeCollector : CSharpSyntaxWalker
    {
        private readonly TaskLoggingHelper Log;

        public ICollection<TypeSyntax> ArrayTypes { get; } = new List<TypeSyntax>();
        public ICollection<int> ArraySizes { get; } = new List<int>();

        public ArrayTypeCollector(TaskLoggingHelper log)
        {
            Log = log;
        }

        // https://learn.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax?view=roslyn-dotnet-4.3.0
        private void VisitType(TypeSyntax node)
        {
            Log.LogMessage($"VISITING TYPE: {node} - {node.Kind()}");

            if (node.ToString().StartsWith("Array"))
            {
                string rawArrayType = node.ToString().Split('<')[0];
                if (int.TryParse(rawArrayType.Substring(5), out int size))
                {
                    ArrayTypes.Add(node);

                    if (!ArraySizes.Contains(size))
                    {
                        ArraySizes.Add(size);
                    }
                }
            }
        }

        public override void Visit(SyntaxNode? node)
        {
            if (node != null && !(node is CompilationUnitSyntax))
            {
                Log.LogMessage(MessageImportance.High, $"Found node: {node} - {node.Kind()}");
            }
            base.Visit(node);
        }

        public override void VisitGenericName(GenericNameSyntax node)
        {
            if (node.ToString().StartsWith("Array"))
            {
                Log.LogMessage($"VISITING ARRAY NAME: {node}");
                string rawArrayType = node.ToString().Split('<')[0];
                if (int.TryParse(rawArrayType.Substring(5), out int size))
                {
                    ArrayTypes.Add(node);

                    if (!ArraySizes.Contains(size))
                    {
                        ArraySizes.Add(size);
                    }
                }
            }
        }

        public override void VisitPredefinedType(PredefinedTypeSyntax node) => this.VisitType(node);
        public override void VisitArrayType(ArrayTypeSyntax node) => this.VisitType(node);
        public override void VisitPointerType(PointerTypeSyntax node) => this.VisitType(node);
        public override void VisitFunctionPointerType(FunctionPointerTypeSyntax node) => this.VisitType(node);
        public override void VisitNullableType(NullableTypeSyntax node) => this.VisitType(node);
        public override void VisitTupleType(TupleTypeSyntax node) => this.VisitType(node);
        public override void VisitRefType(RefTypeSyntax node) => this.VisitType(node);
        public override void VisitScopedType(ScopedTypeSyntax node) => this.VisitType(node);
    }
}