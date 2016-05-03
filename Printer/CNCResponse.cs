using System;

namespace Printer
{
	public enum CNCResponse : byte
	{
		MoveEnd = (byte)'M',
		Acknowledgement = (byte)'a',
		OK = (byte)'O',
		WaitForConnect = (byte)'w',
		ConnectionOpen = (byte)'o',
	}
}

