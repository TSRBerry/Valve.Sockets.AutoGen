using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using Valve.Sockets.SourceGen.Helper;

namespace Valve.Sockets.SourceGen
{
    [Generator]
    public class StructArrayGenerator : ISourceGenerator
    {
        private const string Namespace = "StructArray";

        private List<int> _arraySizes = new List<int>();
        private string _globalNamespace;

        private string GenerateInterface()
        {
            CodeGenerator generator = new CodeGenerator();

            generator.AppendLine("// Source: https://github.com/Ryujinx/Ryujinx/blob/df758eddd1d61f776415422dc4dd1fa8a776719c/Ryujinx.Common/Memory/IArray.cs");
            generator.AppendLine();

            generator.EnterScope($"namespace {_globalNamespace}.{Namespace}");

            generator.AppendLine("/// <summary>");
            generator.AppendLine("/// Array interface.");
            generator.AppendLine("/// </summary>");
            generator.AppendLine("/// <typeparam name=\"T\">Element type</typeparam>);");
            generator.EnterScope("public interface IArray<T> where T : unmanaged");

            generator.AppendLine("/// <summary>");
            generator.AppendLine("/// Used to index the array.");
            generator.AppendLine("/// </summary>");
            generator.AppendLine("/// <param name=\"index\">Element index</param>");
            generator.AppendLine("/// <returns>Element at the specified index</returns>");
            generator.AppendLine("ref T this[int index] { get; }");

            generator.AppendLine();

            generator.AppendLine("/// <summary>");
            generator.AppendLine("/// Number of elements on the array.");
            generator.AppendLine("/// </summary>");
            generator.AppendLine("int Length { get; }");

            generator.LeaveScope();
            generator.LeaveScope();

            return generator.ToString();
        }

        private static int GetAvailableMaxSize(IReadOnlyList<int> availableSizes, ref int index, int missingSize)
        {
            if (availableSizes.Count == 0 || missingSize == 1)
            {
                return 1;
            }

            if (availableSizes.Count < index + 1)
            {
                return 0;
            }

            int size = 0;

            while (size == 0 || size > missingSize && availableSizes.Count - index > 0)
            {
                index++;
                size = availableSizes[availableSizes.Count - index];
            }

            return size;
        }

        private void GenerateArray(CodeGenerator generator, int size, IReadOnlyList<int> availableSizes)
        {
            generator.EnterScope($"public struct Array{size}<T> : IArray<T> where T : unmanaged");

            generator.AppendLine("T _e0;");

            if (size > 1)
            {
                generator.AppendLine("#pragma warning disable CS0169");
            }

            if (availableSizes.Count == 0)
            {
                for (int i = 1; i < size; i++)
                {
                    generator.AppendLine($"T _e{i};");
                }
            }
            else
            {
                int counter = 1;
                int currentSize = 1;
                int maxSizeIndex = 0;
                int maxSize = 0;

                while (currentSize < size)
                {
                    if (maxSize == 0 || currentSize + maxSize > size)
                    {
                        maxSize = GetAvailableMaxSize(availableSizes, ref maxSizeIndex, size - currentSize);
                    }

                    generator.AppendLine(maxSize> 1 ? counter == 1
                        ? $"Array{maxSize}<T> _other;" : $"Array{maxSize}<T> _other{counter};"
                        : $"T _e{counter};"
                    );

                    counter++;
                    currentSize += maxSize;
                }
            }

            if (size > 1)
            {
                generator.AppendLine("#pragma warning restore CS0169");
            }

            generator.AppendLine();
            generator.AppendLine($"public int Length => {size};");
            generator.AppendLine("public ref T this[int index] => ref AsSpan()[index];");
            generator.AppendLine("public Span<T> AsSpan() => MemoryMarshal.CreateSpan(ref _e0, Length);");

            generator.LeaveScope();
            generator.AppendLine();
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new StructArraySyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            StructArraySyntaxReceiver syntaxReceiver = (StructArraySyntaxReceiver)context.SyntaxReceiver;

            if (syntaxReceiver == null)
            {
                return;
            }

            if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("structarray_generator_parent_namespace", out string parentNamespace))
            {
                _globalNamespace = parentNamespace;
            }
            else
            {
                if (context.Compilation.Assembly.NamespaceNames.Count > 1)
                {
                    _globalNamespace = context.Compilation.Assembly.NamespaceNames.ToArray()[1];
                }
                else
                {
                    _globalNamespace = context.Compilation.Assembly.NamespaceNames.ToArray()[0];
                }
            }

            if (_globalNamespace.Trim().Length == 0)
            {
                _globalNamespace = "Common";
            }

            context.AddSource("IArray.g.cs", GenerateInterface());

            _arraySizes = syntaxReceiver.ArraySizes;

            if (_arraySizes.Count == 0)
            {
                return;
            }

            _arraySizes.Sort();

            CodeGenerator generator = new CodeGenerator();
            List<int> sizesAvailable = new List<int>();

            generator.AppendLine("using System;");
            generator.AppendLine("using System.Runtime.InteropServices;");
            generator.AppendLine();

            generator.EnterScope($"namespace {_globalNamespace}.{Namespace}");

            // Always generate Arrays for 1, 2 and 3
            GenerateArray(generator, 1, sizesAvailable);
            sizesAvailable.Add(1);
            GenerateArray(generator, 2, sizesAvailable);
            sizesAvailable.Add(2);
            GenerateArray(generator, 3, sizesAvailable);
            sizesAvailable.Add(3);

            foreach (var size in _arraySizes)
            {
                if (!sizesAvailable.Contains(size))
                {
                    GenerateArray(generator, size, sizesAvailable);
                    sizesAvailable.Add(size);
                }
            }

            generator.LeaveScope();

            context.AddSource("Arrays.g.cs", generator.ToString());
        }
    }
}