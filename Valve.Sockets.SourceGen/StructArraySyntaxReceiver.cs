using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Valve.Sockets.SourceGen
{
    class StructArraySyntaxReceiver : ISyntaxReceiver
    {
        public List<int> ArraySizes { get; }

        public StructArraySyntaxReceiver()
        {
            ArraySizes = new List<int>();
        }

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is TypeSyntax type && type.ToString().StartsWith("Array"))
            {
                string rawArrayType = type.ToString().Split('<')[0];
                if (int.TryParse(rawArrayType.Substring(5), out int size))
                {
                    ArraySizes.Add(size);
                }
            }
        }
    }
}