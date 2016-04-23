using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using System.IO;

namespace Printer
{
    public class CNCContext 
    {
		public double XStepsPerMilimeter { get; set; }
		public double YStepsPerMilimeter { get; set; }
		public double ZStepsPerMilimeter { get; set; }
		public double EStepsPerMilimeter { get; set; }

        public IPrinterSender Sender { get; private set; }

        public CNCContext(IPrinterSender sender)
        {
            Sender = sender;

			XStepsPerMilimeter = 80; //84;
			YStepsPerMilimeter = 80; //84;
			ZStepsPerMilimeter = 4000;
		 	EStepsPerMilimeter = 615;
        }

		public void Open()
		{
            Sender.Open();
		}

		public void Close ()
		{
            Sender.Close();
		}

		public void SendXYZE(int x, int y, int z, int e)
		{
			int ax = x < 0 ? -x: x;
			int ay = y < 0 ? -y: y;
			int az = z < 0 ? -z: z;
			int ae = e < 0 ? -e: e;


			if (ax != 0) 
			{
				CNCComands cmd = x < 0 ? CNCComands.SubX : CNCComands.AddX;
				for (int i = 0; i < ax/255; i++) 
				{
					SendCommand (cmd, 255);
				}
				SendCommand (cmd,(byte)(ax % 255));
			}

			if (ay != 0) 
			{
				CNCComands cmd = y < 0 ? CNCComands.SubY : CNCComands.AddY;
				for (int i = 0; i < ay/255; i++) 
				{
					SendCommand (cmd, 255);
				}
				SendCommand (cmd,(byte)(ay % 255));
			}

			if (az != 0) 
			{
				CNCComands cmd = z < 0 ? CNCComands.SubZ : CNCComands.AddZ;
				for (int i = 0; i < az/255; i++) 
				{
					SendCommand (cmd, 255);
				}
				SendCommand (cmd,(byte)( az % 255));
			}

			if (ae != 0) 
			{
				CNCComands cmd = e < 0 ? CNCComands.SubE : CNCComands.AddE;
				for (int i = 0; i < ae/255; i++) 
				{
					SendCommand (cmd, 255);
				}
				SendCommand (cmd,(byte)( ae % 255));
			}



			SendCommand (CNCComands.Start, 0);
		}



		public void SendCommand(CNCComands cmd, byte value = 0)
        {
            Sender.SendByte((byte)cmd);
            Sender.SendByte(value);


            if (Sender.CanRead &&( cmd == CNCComands.Start || cmd == CNCComands.Home || cmd == CNCComands.GetTemp || cmd == CNCComands.SearchHome) )
            {
                Sender.ReadByte();
			}
		}
		public byte SendCommandWithReturn(CNCComands cmd, byte value = 0)
		{
            if (!Sender.CanRead)
                throw new Exception("Sender kann nicht lesen");

            Sender.SendByte((byte)cmd);
            Sender.SendByte(value);

            var result = Sender.ReadByte();

			return result;
		}

		public void SendDefaultSpeed()
		{
			SendCommand (CNCComands.SetDefaultSpeed);
		}
		public void SendSpeed(short speed)
		{
			var bytes = BitConverter.GetBytes (speed);

			SendCommand (CNCComands.SetFirstSpeedByte,bytes[1]);
			SendCommand (CNCComands.SetSecondSpeedByte,bytes[0]);
		}

		public void SendMoveX(int x)
		{
			SendXYZE (x, 0, 0, 0);

			return;
		}

		public void SendLine(int steps,int x ,int y)
		{

			for (int i = 0; i < steps; i++) 
			{
				SendXYZE (x, y,0,0);
			}
		}

		public void SendGoHome()
		{
			SendCommand (CNCComands.Home, 0);
		}

		public void SendMoveY(int y)
		{
			SendXYZE (0, y, 0, 0);
		}
		public void SendMoveZ(int z)
		{
			SendXYZE (0, 0, z, 0);
		}

		public void SendMoveE(int e)
		{
			SendXYZE (0, 0, 0, e);
		}
    }
}

