using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.IO.Compression;

namespace Printer
{
	public class GCodeContext
	{
		public CNCContext CNC { get; private set; }

		public int XPosition { get; private set; }
		public int YPosition { get; private set; }
		public int ZPosition { get; private set; }
		public int EPosition { get; private set; }

		public bool Simulate { get; set; }

		public GCodeContext (CNCContext context)
		{
			CNC = context;
		}

		public void WorkFile(string path)
		{
			XPosition = 0;
			ZPosition = 0;
			YPosition = 0;

			using(var fs = File.Open(path,FileMode.Open,FileAccess.Read))
			{
				StreamReader sr = new StreamReader (fs);

				while (!sr.EndOfStream) 
				{
					var line = sr.ReadLine ();
					IntepretLine (line);
				}

			}
		}

        public void CompileWorkFile(string path)
        {
            XPosition = 0;
            ZPosition = 0;
            YPosition = 0;

            using (MemoryStream ms = new MemoryStream())
            {
                using (var fs = File.Open(path, FileMode.Open, FileAccess.Read))
                {
                    fs.CopyTo(ms);
                }

                ms.Seek(0, SeekOrigin.Begin);

                using(GZipStream zip = new GZipStream(ms,CompressionMode.Decompress))
                {
                    using(BinaryReader br = new BinaryReader(zip))
                    {
                        var lastposition = 0;


                        try
                        {
                            while (true)
                            {

                                var cmd = (CNCComands)br.ReadByte();
                                var value = br.ReadByte();

                                CNC.SendCommand(cmd, value);

                                var position = (int)((double)ms.Position / ms.Length * 1000);
                                if (position > lastposition)
                                {
                                    Console.WriteLine("{0} {1}/{2} [{3}%]",DateTime.Now.ToLongTimeString(),ms.Position,ms.Length,position /10.0);
                                    lastposition = position;
                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            
                        }
                    }
                }
            }
        }

        public void EndMove()
        {
            if (!Simulate)
            {
                CNC.SendMoveE((int)(-10 * CNC.EStepsPerMilimeter));
                CNC.SendMoveZ((int)(10 * CNC.ZStepsPerMilimeter));
                CNC.SendCommand(CNCComands.SetTemp, 0);

            }
        }

		public void IntepretLine(string line)
		{
			line = line.Split (';') [0];

			if (line != string.Empty) 
			{
				line = line.Trim ();

				var splits = line.Split (new String[]{" "},StringSplitOptions.RemoveEmptyEntries);

				var cmd = splits [0];
				List<GCodeValue> values = new List<GCodeValue> ();
				if (splits.Length > 1) 
				{
					for (int i = 1; i < splits.Length; i++) 
					{
						values.Add (GCodeValue.Intepret (splits [i]));
					}
				}

				switch (cmd) 
				{
				case "G1": // liniear
					G1(values);
					break;
				case "G21": //Set Milimeter
					break;
				case "G28": //Home
					break;
				case "G90": //Set Absolute position
					break;
				case "G92": //Set Position
					SetPosition(values);
					break;
				case "M82": //Set extruder to absolute mode 
					break;
				case "M84": //Stop idle hold 
					break;
				case "M104": // Set Extruder Temperature
					break;
				case "M109": // Set Extruder Temperature and Wait
					break;
				case "M106": //Fan on:
					break;
				case "M107": //Fan off:
					break;
				default:
					Console.WriteLine ("Unbekanntes Kommando:{0}", cmd);
					break;
				}
			}
		}

		private void SetPosition(List<GCodeValue> values)
		{
			foreach (var item in values) 
			{
				if (item.Type == GCodeValueType.XAxis)
				{
					XPosition = (int)Math.Round(item.Value * CNC.XStepsPerMilimeter);
					Console.WriteLine ("Set X Position to {0}",XPosition);	
				}
				else if (item.Type == GCodeValueType.YAxis)
				{
					YPosition = (int)Math.Round(item.Value * CNC.YStepsPerMilimeter);
					Console.WriteLine ("Set Y Position to {0}",YPosition);	
				}
				else if (item.Type == GCodeValueType.ZAxis)
				{
					ZPosition = (int)Math.Round(item.Value * CNC.ZStepsPerMilimeter);
					Console.WriteLine ("Set Z Position to {0}",ZPosition);	
				}
				else if (item.Type == GCodeValueType.ExtruderAxis)
				{
					EPosition = (int)Math.Round(item.Value * CNC.EStepsPerMilimeter);
					Console.WriteLine ("Set E Position to {0}",EPosition);	
				}
			}
		}

		private void G1(List<GCodeValue> values)
		{
			int ix = XPosition;
			int iy = YPosition;
			int iz = ZPosition;
			int ie = EPosition;

			foreach (var item in values) 
			{

				if (item.Type == GCodeValueType.XAxis) 
				{
					ix = (int)Math.Round (item.Value * CNC.XStepsPerMilimeter);
				}	
				else if (item.Type == GCodeValueType.YAxis) 
				{
					iy = (int)Math.Round (item.Value * CNC.YStepsPerMilimeter);
				}
				else if (item.Type == GCodeValueType.ZAxis) 
				{
					iz = (int)Math.Round (item.Value * CNC.ZStepsPerMilimeter);
				}
				else if (item.Type == GCodeValueType.ExtruderAxis) 
				{
					ie = (int)Math.Round (item.Value * CNC.EStepsPerMilimeter);
				}
				else if (item.Type == GCodeValueType.FeedRate) 
				{
					var feedpersec = item.Value / 60;
					var stepspersec = feedpersec * CNC.XStepsPerMilimeter;
					var stepspermicosec = stepspersec / 1E6;
					var micosec = 1 / stepspermicosec /2;
					CNC.SendSpeed ((short)micosec);
					Console.WriteLine ("SetFeedRate:{0}mm/s [{1}]",feedpersec,micosec);
				}

			}

			var mx = ix - XPosition;
			var my = iy - YPosition;
			var mz = iz - ZPosition;
			var me = ie - EPosition;

			Console.WriteLine ("G1 X{0} Y{1} Z{2} E{3}",mx,my,mz,me);
			//Console.WriteLine ("GF1 X{0} Y{1} Z{2}",fx,fy,fz);

            var amx = mx < 0 ? -1 * mx : mx;
            var amy = my < 0 ? -1 * my : my;

            if (mx == 0 || my == 0 || amx == amy)
            {
                if (!Simulate) 
                {
                    CNC.SendXYZE (mx, my, mz, me);
                }
            }
            else
            {
                var m = (double)my / mx;
                var lenght = amx + amy;

                int lastx = 0;
                int lasty = 0;

                for (int i = 1; i <= amx; i++)
                {
                    var nx = (mx < 0 ? -1 * i : i);

                    var ny = (int) (m * nx);

                    if ((ny > lasty && ny > 0) || (ny < lasty && ny < 0) )
                    {
                        var dx = nx - lastx;
                        var dy = ny - lasty + 1;

                        if ((dy +lasty > my && my > 0) || (dy + lasty < my && my < 0))
                        {
                            if (my > 0)
                                dy -= 1;
                            else if (my < 0)
                                dy += 1;
                        }

                        var xe = (int)((double)dx / lenght * me);
                        var ye = (int)((double)dy / lenght * me);

                        CNC.SendXYZE (dx, 0, 0, xe);
                        CNC.SendXYZE (0, dy, 0, ye);

                        lastx += dx;
                        lasty += dy;
                    }
                }
            }

			

			XPosition = ix;
			YPosition = iy;
			ZPosition = iz;
			EPosition = ie;
		}
	}
}

