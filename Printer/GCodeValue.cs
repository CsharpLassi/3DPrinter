using System;
using System.Globalization;

namespace Printer
{
	public class GCodeValue
	{
		public GCodeValueType Type { get; set; }
		public double Value { get; set; }

		public GCodeValue (GCodeValueType type, double value)
		{
			Type = type;
			Value = value;
		}

		public GCodeValue ()
		{
			Type = GCodeValueType.Unknow;
		}

		public static GCodeValue Intepret(string value)
		{
            value = value.ToLower();

			var result = new GCodeValue ();

			var fc = value [0];

			var valuestring = value.Remove (0, 1);


			result.Value = double.Parse(valuestring,CultureInfo.InvariantCulture);

			switch (fc) 
			{
			case  'x':
				result.Type = GCodeValueType.XAxis;
				break;
			case  'y':
				result.Type = GCodeValueType.YAxis;
					break;
			case  'z':
				result.Type = GCodeValueType.ZAxis;
					break;
			case  'e':
				result.Type = GCodeValueType.ExtruderAxis;
					break;
			case  's':
				result.Type = GCodeValueType.ExtruderTemperatur;
					break;
			case 'f':
				result.Type = GCodeValueType.FeedRate;
				break;
			default:
				break;
			}

			return result;
		}

	

	}

	public enum GCodeValueType
	{
		Unknow,
		XAxis,
		YAxis,
		ZAxis,
		ExtruderAxis,
		ExtruderTemperatur,
		FeedRate
	}
}

