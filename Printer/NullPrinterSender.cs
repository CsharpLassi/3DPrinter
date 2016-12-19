using System;

namespace Printer
{
    public class NullPrinterSender : IPrinterSender
    {
        public NullPrinterSender()
        {
        }

        public bool CanRead {get { return false; }}

        public void Open()
        {
        }

        public void Close()
        {
        }


        public bool SendByte(byte command,byte sendbyte)
        {
            return true;
        }

        public byte ReadByte()
        {
            throw new Exception("Du Idiot");
        }
    }
}

