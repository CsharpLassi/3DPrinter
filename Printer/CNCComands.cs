using System;

namespace Printer
{
	public enum CNCComands : byte
	{
		None = 0,
		Start = 1,
		Home = 2,
		SearchHome = 3,
		ChangeMode=8,
		AddX = 16,
		SubX = 17,
		AddY = 18,
		SubY = 19,
		AddZ = 20,
		SubZ = 21,
		AddE = 22,
		SubE = 23,
		SetTemp=32,
		GetTemp=33,
		SetDefaultSpeed = 40,
		SetFirstSpeedByte = 41,
		SetSecondSpeedByte = 42,

	}
}

