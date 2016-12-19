using System;

namespace DSVG
{
    
    public class DirectModeInterface : BasePrinterInterface
    {
        public DirectModeInterface(PrinterModel printer,TerminalWindow window)  : base(window,printer)
        {
            Name = "DirectMode";
            Interfaces.Add(new ActionInterface("SetTemperature",SetTemperature));
            Interfaces.Add(new ActionInterface("G-Code",ExecuteGCODE));
        }

        private void SetTemperature(string temp)
        {
            byte btemp = 0;

            if (byte.TryParse(temp,out btemp))
            {
                base.Printer.HandContext.SetTemp(btemp);
            }
        }

        private void ExecuteGCODE(string cmd)
        {
            base.Printer.GCodeContext.IntepretLine(cmd);
        }

    }

}

