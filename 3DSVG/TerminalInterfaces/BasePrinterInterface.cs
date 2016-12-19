using System;

namespace DSVG
{
    public abstract class BasePrinterInterface : WindowInterface
    {
        public TerminalWindow Window { get; private set; }
        public PrinterModel Printer { get; private set; }

        public BasePrinterInterface(TerminalWindow window,PrinterModel printer)
        {

            Window = window;
            Printer = printer;
        }
    }
}

