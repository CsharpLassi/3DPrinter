using System;
using System.IO;

namespace DSVG
{
    public class FileModeInterface : BasePrinterInterface
    {
        public FileModeInterface(PrinterModel printer,TerminalWindow window) : base(window,printer)
        {
            Name = "FileMode";

            Interfaces.Add(new ActionInterface("Execute File",OpenFile));
        }

        private void OpenFile(string filename)
        {
            if (!File.Exists(filename))
            {
                Window.WriteLine("Datei existiert nicht");
                return;
            }

            Printer.GCodeContext.WorkFile(filename);
        }
    }
}

