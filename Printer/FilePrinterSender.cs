using System;
using System.IO;
using System.IO.Compression;

namespace Printer
{
    public class FilePrinterSender : IPrinterSender
    {
        public bool CanRead {get { return false; }}
        public string FileName { get; private set; }

        private Stream _fs;
        private BinaryWriter _bw;

        public FilePrinterSender(string filename)
        {
            FileName = filename;
        }

        public void Open()
        {
            _fs = File.Open(FileName, FileMode.Create, FileAccess.ReadWrite);
			_bw = new BinaryWriter(_fs);
        }

        public void Close()
        {
            _bw.Close();
            _fs.Close();
        }

        public void SendByte(byte sendbyte)
        {
            _bw.Write(sendbyte);
        }
        public byte ReadByte()
        {
            throw new Exception("FilePrinterSender kann nicht lesen");
        }
    }
}

