using System;

namespace Printer
{
    public interface IPrinterSender
    {
        bool CanRead {get;}

        void Open();
        void Close();

        void SendByte(byte sendbyte);
        byte ReadByte();
    }
}

