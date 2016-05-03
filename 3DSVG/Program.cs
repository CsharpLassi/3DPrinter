using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using System.IO;
using Printer;
using System.IO.Ports;

namespace DSVG
{
    class MainClass
    {
        public static void Main(string[] args)
        {
			Console.WriteLine ("Modis");
			Console.WriteLine ("0)Beenden");
			Console.WriteLine ("1)GCode-Datei");
            Console.WriteLine ("2)Kompillierte GCode-Datei");
			Console.WriteLine ("3)Hand");

			int input = 0;
			do 
			{
				Console.Write ("Eingabe:");
				input = int.Parse(Console.ReadLine());

				if (input == 1) 
                {
                    ModGCode();
                }
                if (input == 2) 
                {
                    ModGCode(true);
                }
				if (input == 3) 
                {
                    try
                    {
                        var context = OpenContext();
                        ModHand(context);
                        context.Close ();
                    } 
                    catch (Exception ex) 
                    {
                        Console.WriteLine (ex.Message);
                    }
                }


			} while (input != 0);

			

        }

        private static string ReadPort()
        {
            bool isvalid = false;

            do
            {
                Console.Write ("Port:");
                var portname = Console.ReadLine ();

                if (portname == string.Empty) 
                    throw new Exception("Vorgang wurde abgebrochen");

                isvalid = SerialPort.GetPortNames().Any(i => i == portname);

                if (!isvalid) 
                    Console.WriteLine ("Port ist nicht gültig");
                else 
                    return portname;
            } while (true);


        }

        private static CNCContext OpenContext()
        {
            var portname = ReadPort();

            Console.WriteLine ("Öffne Port");
            DirectPrintSender sender = new DirectPrintSender(portname);

            CNCContext context = new CNCContext(sender);
            context.Open ();
            Console.WriteLine ("Port geöffnet");

            return context;
        }

		private static void ModHand(CNCContext context)
		{
			HandContext hand = new HandContext (context);

			Console.WriteLine ("Hand:");
			Console.WriteLine ("0)Zurück");
			Console.WriteLine ("1)X");
			Console.WriteLine ("2)Y");
			Console.WriteLine ("3)Z");
			Console.WriteLine ("4)Extruder");
			Console.WriteLine ("5)SetTemp");
			Console.WriteLine ("6)GetTemp");
			Console.WriteLine ("7)SetWaitTemp");
			Console.WriteLine ("8)Search Home");
			Console.WriteLine ("9)Move Start");
			int input = 0;
			do 
			{
				Console.Write ("Eingabe:");

				input = int.Parse(Console.ReadLine());
				if (input == 1) 
				{
					double value = 0;
					do 
					{
						Console.Write ("X[mm]:");
						value = double.Parse(Console.ReadLine());
						hand.MoveX(value);

					} while (value != 0);
				}
				else if (input == 2) 
				{
					double value = 0;
					do 
					{
						Console.Write ("Y[mm]:");
						value = double.Parse(Console.ReadLine());
						hand.MoveY(value);
					
					} while (value != 0);
				}
				else if (input == 3) 
				{
					double value = 0;
					do 
					{
						Console.Write ("Z[mm]:");
						value = double.Parse(Console.ReadLine());
						hand.MoveZ(value);

					} while (value != 0);
				}
				else if (input == 4) 
				{
					double value = 0;
					do 
					{
						Console.Write ("E[mm]:");
						value = double.Parse(Console.ReadLine());
						hand.MoveE(value);
					} while (value != 0);
				}
				else if (input == 5) 
				{
					byte value = 0;
					Console.Write ("Temp[°C]:");
					value = byte.Parse(Console.ReadLine());
					hand.SetTemp(value);
				}
				else if (input == 6) 
				{

					Console.WriteLine ("Temp[°C]:{0}",hand.GetTemp());

				}
				else if (input == 7) 
				{
					byte value = 0;
					Console.Write ("Temp[°C]:");
					value = byte.Parse(Console.ReadLine());
					hand.SetCheckTemp(value);
					Console.WriteLine ("Wait");
				}
				else if (input == 8) 
				{
					hand.SearchHome();
				}
				else if (input == 9) 
				{
					hand.MoveToStart();
				}

			} while (input != 0);

		}

        private static void ModGCode(bool compile = false)
        {
			
            Console.Write("Datei:");
            var path = Console.ReadLine();
            if (!File.Exists(path))
            {
                Console.WriteLine("Datei nicht gefunden");
                return;
            }

            CNCContext context = null;
            bool simulate = true;

            if (!compile)
            {
                Console.WriteLine("Dateidirekt senden(y,N):");
                var key = Console.ReadKey();

                if (key.KeyChar == 'y')
                {
                    context = OpenContext();
                }
                else
                {
                    var filepath = path + ".cmp";
                    FilePrinterSender filesender = new FilePrinterSender(filepath);
                    context = new CNCContext(filesender);
                    context.Open();
                    simulate = false;
                }
            }
            else
            {
                context = OpenContext();
                simulate = false;
            }




            GCodeContext gcode = new GCodeContext(context);

            if (simulate && ! compile)
            {
                gcode.Simulate = true;
                gcode.WorkFile(path);
                gcode.Simulate = false;
            }

            if (!compile)
            {
                gcode.WorkFile(path);
                gcode.EndMove();
            }
            else
            {
                gcode.CompileWorkFile(path);
            }


            context.Close();



		}
    }
}
