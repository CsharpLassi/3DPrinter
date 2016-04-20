using System;
using System.IO.Ports;
using System.IO;
using System.Threading;

namespace Printer
{
    public class DirectPrintSender : IPrinterSender
    {
        public string Port { get; set; }
        public bool CanRead {get { return true; }}

        SerialPort _port= null;
        private BinaryWriter _bw;
        private BinaryReader _br;

        private int bytebuffer = 0;

        public DirectPrintSender(string port)
        {
            Port = port;
        }

        public void Open()
        {
            _port = new SerialPort (Port, 115200);
            _port.Open ();

            _bw = new BinaryWriter (_port.BaseStream);
            _br = new BinaryReader (_port.BaseStream);

            Thread.Sleep (2000);

            byte sb = (byte)'s';

            _bw.Write (sb);

            char rc = '\0'; 

            do 
            {
                Thread.Sleep (100);
                rc = (char)_br.ReadByte ();
            } while(rc != 'o');
        }

        public void Close()
        {
            if (_port != null &&  _port.IsOpen) {
                _bw.Close();
                _br.Close();
                _port.Close ();
            }


        }

        public void SendByte(byte sendbyte)
        {
            if (bytebuffer > 30) {
                Thread.Sleep (10);
                bytebuffer = 0;
            }

            bytebuffer++;
            _bw.Write(sendbyte);
        }

        public byte ReadByte()
        {
            return _br.ReadByte();
        }
    }
}

