using System;

namespace Printer
{
    public interface ITextPrinter
    {
        void WriteLine();
        void WriteLine(string text);
        void WriteLine(string text, params object[] values);
    }
}

