using System.Text;

namespace Valve.Sockets.Tasks.Helper
{
    // Source: https://github.com/Ryujinx/Ryujinx/blob/df758eddd1d61f776415422dc4dd1fa8a776719c/Ryujinx.Horizon.Generators/CodeGenerator.cs

    class CodeGenerator
    {
        private const string Indent = "    ";
        private readonly StringBuilder _sb;
        private string _currentIndent;

        public CodeGenerator()
        {
            _sb = new StringBuilder();
        }

        public void EnterScope(string header = null)
        {
            if (header != null)
            {
                AppendLine(header);
            }

            AppendLine("{");
            IncreaseIndentation();
        }

        public void LeaveScope()
        {
            DecreaseIndentation();
            AppendLine("}");
        }

        public void IncreaseIndentation()
        {
            _currentIndent += Indent;
        }

        public void DecreaseIndentation()
        {
            _currentIndent = _currentIndent.Substring(0, _currentIndent.Length - Indent.Length);
        }

        public void AppendLine()
        {
            _sb.AppendLine();
        }

        public void AppendLine(string text)
        {
            _sb.AppendLine(_currentIndent + text);
        }

        public override string ToString()
        {
            return _sb.ToString();
        }
    }
}