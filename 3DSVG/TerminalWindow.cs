using System;
using System.Threading;
using System.Collections.Generic;
using Printer;

namespace DSVG
{
	public class TerminalWindow : ITextPrinter
	{
		private string _printerport = "/dev/null";
		public string PrinterPort
		{
			get 
			{
				return _printerport;
			}
			set
			{
                if (_printerport != value)
                {
                    _printerport = value;
                    DrawHeader();
                }
			}
		}

        private byte _printertempature = 0;
        public byte PrinterTempature
        {
            get
            {
                return _printertempature;
            }
            set
            {
                if (_printertempature != value)
                {
                    _printertempature = value;
                    DrawHeader();
                }
            }
        }

        private ConsoleColor _stdbackground;
        private ConsoleColor _stdforeground;

        private string _inputext = string.Empty;
        private int _selectposition = 0;

        public List<WindowInterface> Interfaces{ get; private set; }

        private Stack<WindowInterface> _windowstack = new Stack<WindowInterface>();

        private List<string> _messages = new List<string>();

		public TerminalWindow ()
		{
            _stdbackground = Console.BackgroundColor;
            _stdforeground = Console.ForegroundColor;

            Interfaces = new List<WindowInterface>();

		}

        public void Run()
        {
            Thread work = new Thread(ResizeMethod);
            work.IsBackground = true;

            work.Start();

            Draw();
            bool abort = false;
            while (!abort )
            {
                var key = Console.ReadKey(true);

                var interfacelist = _windowstack.Count == 0 ? Interfaces : _windowstack.Peek().Interfaces;

                if (key.Key == ConsoleKey.Escape)
                {
                    if (_windowstack.Count > 0)
                    {
                        _windowstack.Pop();
                        Draw();
                    }
                    else
                    {
                        abort = true;
                    }
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    if (Interfaces.Count != 0)
                    {
                        var item = interfacelist[_selectposition];

                        if (item.Interfaces.Count > 0 && item.CanChangeWindow(_inputext))
                        {
                            _windowstack.Push(item);
                            _selectposition = 0;
                        }

                        item.OnInteract(_inputext);



                        DrawLeft();
                    }

                    _inputext = string.Empty;
                    DrawFooter();
                }
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    _selectposition++;

                    var height = Console.LargestWindowHeight;

                    var max = height < interfacelist.Count ? height : interfacelist.Count;

                    if (_selectposition >= max)
                        _selectposition = max - 1;

                    DrawLeft();
                }
                else if (key.Key == ConsoleKey.UpArrow)
                {
                    _selectposition--;

                    if (_selectposition < 0)
                        _selectposition = 0;

                    DrawLeft();
                }
                else if (key.Key == ConsoleKey.Backspace && _inputext.Length > 0)
                {
                    _inputext = _inputext.Remove(_inputext.Length - 1, 1);
                    DeleteInputChar();
                }
                else if (key.Key == ConsoleKey.Spacebar)
                {
                    _inputext += ' ';
                    DrawInputChar(' ');
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    _inputext += key.KeyChar;
                    DrawInputChar(key.KeyChar);
                }

            }

            Console.Clear();
        }

        private void ResizeMethod()
        {
            var _oldheight = Console.LargestWindowHeight;
            var _oldwidth = Console.LargestWindowWidth;
            while (true)
            {
                var height = Console.LargestWindowHeight;
                var width = Console.LargestWindowWidth;

                if (height != _oldheight || width != _oldwidth)
                {
                    Draw();
                }

                _oldwidth = width;
                _oldheight = height;

                Thread.Sleep(100);
            }
        }

		public void Draw()
		{
            bool redraw = false;

            do
            {
                redraw = false;

                try 
                {
                    Console.Clear();

                    DrawHeader ();
                    DrawFooter();
                    DrawLeft();
                    DrawRight();
                } 
                catch (Exception ex) 
                {
                    redraw = true;
                }
            } while (redraw);


		}

		void DrawHeader()
		{
			var height = Console.LargestWindowHeight;
			var width = Console.LargestWindowWidth;

            SetColor(ConsoleColor.DarkGreen,ConsoleColor.Red);

			Console.SetCursorPosition (0, 0);

            var strprinter = "Printer:";
            var strtemp = "Temp:";

            var padding = width - strprinter.Length - strtemp.Length - 3;
            Console.Write(strprinter);

            if (padding < _printerport.Length)
            {
                Console.Write(_printerport.Substring(0,padding -3));
                Console.Write("...");
            }
            else
            {
                Console.Write(_printerport);
                for (int i = 0; i < padding-_printerport.Length; i++)
                {
                    Console.Write(" ");
                }
            }

            Console.Write(strtemp);
            Console.Write(PrinterTempature.ToString("D3"));
           

            SetColor();
		}

        void DrawFooter()
        {
            var height = Console.LargestWindowHeight;
            var width = Console.LargestWindowWidth;

            SetColor(ConsoleColor.DarkBlue, ConsoleColor.White);

            DrawLine( 0,height - 1);
            DrawChar(0, height - 1, '>');

            DrawInputText();

            SetColor();
        }

        void DrawLeft()
        {
            var height = Console.LargestWindowHeight - 2;
            var width = Console.LargestWindowWidth  / 2;

            SetColor(ConsoleColor.White,ConsoleColor.Black);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++) 
                {
                    DrawChar(i, j + 1, ' ');
                }
            }
             
            var interfacelist = _windowstack.Count == 0 ? Interfaces : _windowstack.Peek().Interfaces;

            var itemcount = height < interfacelist.Count ? height : interfacelist.Count;



            for (int i = 0; i < itemcount; i++)
            {
                if (_selectposition == i)
                {
                    SetColor(ConsoleColor.Red,ConsoleColor.Black);
                    DrawLine(0,i+1,width);
                }

                DrawString(0, i + 1, interfacelist[i].Name);

                SetColor(ConsoleColor.White,ConsoleColor.Black);
            }

            SetColor();
        }

        void DrawRight()
        {
            var height = Console.LargestWindowHeight - 2;
            var width = Console.LargestWindowWidth - Console.LargestWindowWidth  / 2  ;

            var widthstart = Console.LargestWindowWidth / 2;

            SetColor(ConsoleColor.Yellow,ConsoleColor.Black);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++) 
                {
                    DrawChar(j+widthstart, i + 1, ' ');

                }

                if ( i < _messages.Count)
                {
                    Console.SetCursorPosition(widthstart, i + 1);
                    Console.Write(_messages[i]);
                }


            }

            SetColor();
        }

        void DrawLine(int left,int top)
        {
            var width = Console.LargestWindowWidth;
            DrawLine(left, top, width);
        }

        void DrawLine(int left,int top,int width)
        {
            Console.SetCursorPosition(left, top);

            for (int i = 0; i < width; i++)
            {
                Console.Write(" ");
            }
        }

        void DrawString(int left,int top,string text)
        {
            Console.SetCursorPosition(left, top);
            Console.Write(text);

        }

        void DrawInputText()
        {
            SetColor(ConsoleColor.DarkBlue, ConsoleColor.White);

            var height = Console.LargestWindowHeight;
            Console.SetCursorPosition(1, height-1);
            Console.Write(_inputext);

        }

        void DrawChar(int left,int top,char cr)
        {
            Console.SetCursorPosition(left, top);
            Console.Write(cr);
        }

        void DrawInputChar(char cr)
        {
            SetColor(ConsoleColor.DarkBlue, ConsoleColor.White);

            var height = Console.LargestWindowHeight;
            DrawChar(_inputext.Length, height - 1, cr);
        }

        void DeleteInputChar()
        {
            SetColor(ConsoleColor.DarkBlue, ConsoleColor.White);

            var height = Console.LargestWindowHeight;
            DrawChar(_inputext.Length +1, height - 1, ' ');
            Console.SetCursorPosition(_inputext.Length+1, height - 1);

        }

        void SetColor(ConsoleColor back, ConsoleColor fore)
        {
            Console.BackgroundColor = back;
            Console.ForegroundColor = fore;
        }

        void SetColor()
        {
            Console.BackgroundColor = _stdbackground;
            Console.ForegroundColor = _stdforeground;
        }

        public void WriteLine()
        {
            WriteLine(string.Empty);
        }

        public void WriteLine(string text)
        {
            _messages.Add(text);

            var height = Console.LargestWindowHeight -2;

            if (_messages.Count > height)
                _messages.RemoveAt(0);

            DrawRight();
        }

        public void WriteLine(string text, params object[] values)
        {
            WriteLine(string.Format(text,values));
        }
	}
}

