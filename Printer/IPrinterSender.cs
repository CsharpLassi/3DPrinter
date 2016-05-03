using System;

namespace Printer
{
    public interface IPrinterSender
    {
        bool CanRead {get;}

        void Open();
        void Close();

		bool SendByte(byte command,byte sendbyte);
        byte ReadByte();
    }
}

