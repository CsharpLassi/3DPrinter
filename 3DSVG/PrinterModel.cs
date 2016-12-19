using System;
using Printer;
using System.Threading;

namespace DSVG
{
    public class PrinterModel
    {
        public CNCContext Context { get; private set; }
        public GCodeContext GCodeContext { get; private set; }
        public HandContext HandContext { get; private set; }

        public TerminalWindow Window { get; set; }

        private Thread _workthread;

        public PrinterModel(TerminalWindow window)
        {
            Window = window;

            SetContext(new CNCContext(new NullPrinterSender()));


            _workthread = new Thread(Work);
            _workthread.IsBackground = true;

            _workthread.Start();
        }

        public void SetContext(CNCContext context)
        {
            Context = context;
            GCodeContext = new GCodeContext(context,Window);
            HandContext = new HandContext(context);
        }

        private void Work()
        {
            while (true)
            {
                Window.PrinterTempature = HandContext.GetTemp();


                Thread.Sleep(1000);
            }

        }

    }
}

