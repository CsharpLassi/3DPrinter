using System;
using System.Threading;

namespace Printer
{
	public class HandContext
	{
		public CNCContext Context { get; set; }

		public HandContext (CNCContext context)
		{
			Context = context;
		}

		public void MoveX(double x)
		{
			Context.SendDefaultSpeed ();
			int mx = (int)(x * Context.XStepsPerMilimeter);
			Context.SendMoveX (mx);
		}
		public void MoveY(double y)
		{
			Context.SendDefaultSpeed ();
			int my = (int)(y * Context.YStepsPerMilimeter);
			Context.SendMoveY (my);
		}
		public void MoveZ(double z)
		{
			Context.SendSpeed (100);
			int mz = (int)(z * Context.ZStepsPerMilimeter);
			Context.SendMoveZ (mz);
			Context.SendDefaultSpeed ();
		}
		public void MoveE(double e)
		{
			Context.SendDefaultSpeed ();
			int me = (int)(e * Context.EStepsPerMilimeter);
			Context.SendMoveE (me);
		}
		public void SetTemp(byte temp)
		{
			Context.SendCommand (CNCComands.SetTemp, temp);
		}
		public byte GetTemp()
		{
			return Context.SendCommandWithReturn (CNCComands.GetTemp, 0);
		}

		public void SetCheckTemp(byte temp)
		{
            SetTemp ((byte)temp);
            byte rtemp = 0;

            do 
            {
                Thread.Sleep(1000);

                rtemp = GetTemp();

            } while (rtemp < temp);
		}

		public void SearchHome()
		{
			Context.SendCommandWithReturn (CNCComands.SearchHome);
		}
	}
}

