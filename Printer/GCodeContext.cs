﻿using System;
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

        public ITextPrinter TextPrinter { get; set; }

		public GCodeContext (CNCContext context, ITextPrinter textprinter)
		{
			CNC = context;
            TextPrinter = textprinter;
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

            EndMove();
		}

        public void CompileWorkFile(string path)
        {
            XPosition = 0;
            ZPosition = 0;
            YPosition = 0;

			using (MemoryStream ms = new MemoryStream ()) {
				using (var fs = File.Open (path, FileMode.Open, FileAccess.Read)) {
					fs.CopyTo (ms);
				}

				ms.Seek (0, SeekOrigin.Begin);


				using (BinaryReader br = new BinaryReader (ms)) {
					var lastposition = 0;


					try {
						while (true) {

							var cmd = (CNCComands)br.ReadByte ();
							var value = br.ReadByte ();

							CNC.SendCommand (cmd, value);

							var position = (int)((double)ms.Position / ms.Length * 1000);
							if (position > lastposition) {
                                TextPrinter.WriteLine ("{0} {1}/{2} [{3}%]", DateTime.Now.ToLongTimeString (), ms.Position, ms.Length, position / 10.0);
								lastposition = position;
							}

						}
					} catch (Exception ex) {
                            
					}
				}
                
			}
        }

        public void EndMove()
        {
            if (!Simulate)
            {
                CNC.SendMoveE((int)(-4 * CNC.EStepsPerMilimeter));
                CNC.SendMoveZ((int)(20 * CNC.ZStepsPerMilimeter));
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
                case "G1R": // liniear relative
                    G1R(values);
                    break;
				case "G21": //Set Milimeter
					break;
				case "G28": //Home
					G28(values);
					break;
				case "G90": //Set Absolute position
					break;
				case "G92": //Set Position
					G92(values);
					break;
				case "M82": //Set extruder to absolute mode 
					break;
				case "M84": //Stop idle hold 
					break;
				case "M104": // Set Extruder Temperature
					M104(values);
					break;
				case "M109": // Set Extruder Temperature and Wait
					M109(values);
					break;
				case "M106": //Fan on:
					break;
				case "M107": //Fan off:
					break;
				default:
                        TextPrinter.WriteLine ("Unbekanntes Kommando:{0}", cmd);
					break;
				}
			}
		}

		private void G28(List<GCodeValue> values)
		{
			if (values.Count == 0) 
			{
                TextPrinter.WriteLine ("Alle Axen auf Homeposition");
				CNC.MoveToStartPosition ();
			}	

		}

		private void G92(List<GCodeValue> values)
		{
			foreach (var item in values) 
			{
				if (item.Type == GCodeValueType.XAxis)
				{
					XPosition = (int)Math.Round(item.Value * CNC.XStepsPerMilimeter);
                    TextPrinter.WriteLine ("Set X Position to {0}",XPosition);	
				}
				else if (item.Type == GCodeValueType.YAxis)
				{
					YPosition = (int)Math.Round(item.Value * CNC.YStepsPerMilimeter);
                    TextPrinter.WriteLine ("Set Y Position to {0}",YPosition);	
				}
				else if (item.Type == GCodeValueType.ZAxis)
				{
					ZPosition = (int)Math.Round(item.Value * CNC.ZStepsPerMilimeter);
                    TextPrinter.WriteLine ("Set Z Position to {0}",ZPosition);	
				}
				else if (item.Type == GCodeValueType.ExtruderAxis)
				{
					EPosition = (int)Math.Round(item.Value * CNC.EStepsPerMilimeter);
                    TextPrinter.WriteLine ("Set E Position to {0}",EPosition);	
				}
			}
		}

		private void M104(List<GCodeValue> values)
		{
			foreach (var value in values) 
			{
				if (value.Type == GCodeValueType.ExtruderTemperatur) 
				{
                    TextPrinter.WriteLine ("Set Extruder Temp:{0}°C",value.Value);
					CNC.SendCommand (CNCComands.SetTemp, (byte)value.Value);
				}
			}
		}

		private void M109(List<GCodeValue> values)
		{
			foreach (var value in values) 
			{
				if (value.Type == GCodeValueType.ExtruderTemperatur) 
				{
                    TextPrinter.WriteLine ("Set Extruder Temp:{0}°C",value.Value);
                    TextPrinter.WriteLine ("Wait");
					CNC.SendCommand (CNCComands.SetWaitTemp, (byte)value.Value);
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
                    var micosec = (1 / stepspermicosec) *0.25;
					CNC.SendSpeed ((short)micosec);
                    TextPrinter.WriteLine ("SetFeedRate:{0}mm/s [{1}]",feedpersec,micosec);
				}

			}

			var mx = ix - XPosition;
			var my = iy - YPosition;
			var mz = iz - ZPosition;
			var me = ie - EPosition;

            TextPrinter.WriteLine ("G1 X{0} Y{1} Z{2} E{3}",mx,my,mz,me);
			//Console.WriteLine ("GF1 X{0} Y{1} Z{2}",fx,fy,fz);

            var amx = mx < 0 ? -1 * mx : mx;
            var amy = my < 0 ? -1 * my : my;

			CNC.SendXYZE (mx, my, mz, me);

			XPosition = ix;
			YPosition = iy;
			ZPosition = iz;
			EPosition = ie;
		}

        private void G1R(List<GCodeValue> values)
        {
            int ix = 0;
            int iy = 0;
            int iz = 0;
            int ie = 0;

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
                    var micosec = (1 / stepspermicosec) *0.25;
                    CNC.SendSpeed ((short)micosec);
                    TextPrinter.WriteLine ("SetFeedRate:{0}mm/s [{1}]",feedpersec,micosec);
                }

            }

            var mx = ix;
            var my = iy ;
            var mz = iz ;
            var me = ie;

            TextPrinter.WriteLine ("G1R X{0} Y{1} Z{2} E{3}",mx,my,mz,me);
            //Console.WriteLine ("GF1 X{0} Y{1} Z{2}",fx,fy,fz);

            var amx = mx < 0 ? -1 * mx : mx;
            var amy = my < 0 ? -1 * my : my;

            CNC.SendXYZE (mx, my, mz, me);

            XPosition = ix;
            YPosition = iy;
            ZPosition = iz;
            EPosition = ie;
        }
	}
}

