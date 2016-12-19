using System;
using Printer;
using System.IO.Ports;
using System.Linq;

namespace DSVG
{
    public class ConnectionInterface : BasePrinterInterface
    {
        public ConnectionInterface(PrinterModel printer,TerminalWindow window) : base(window,printer)
        {
            Name = "Connection";
            Interfaces.Add(new ActionInterface("Connect",Connect));
            Interfaces.Add(new ActionInterface("Disconnect",(s) => Disconnect()));
        }

        public override void OnInteract(string input)
        {
            
        }

        private void Connect(string portname)
        {
            if (portname == string.Empty)
                return;

            var isvalid = SerialPort.GetPortNames().Any(i => i == portname);

            if (isvalid)
            {
                Printer.Context.Close();

                DirectPrinterSender dps = new DirectPrinterSender(portname); 
                CNCContext context  = new CNCContext(dps);
                context.Open();
                Window.PrinterPort = portname;
                Printer.SetContext(context);
            }
           

        }

        private void Disconnect()
        {
            Printer.Context.Close();
            Window.PrinterPort = "/dev/null";
        }

    }

}

