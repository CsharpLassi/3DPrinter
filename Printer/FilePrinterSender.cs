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
        private GZipStream _zips;
        private BinaryWriter _bw;

        public FilePrinterSender(string filename)
        {
            FileName = filename;
        }

        public void Open()
        {
            _fs = File.Open(FileName, FileMode.Create, FileAccess.ReadWrite);
            _zips = new GZipStream(_fs, CompressionLevel.Optimal);
            _bw = new BinaryWriter(_zips);
        }

        public void Close()
        {
            _bw.Close();
            _zips.Close();
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

