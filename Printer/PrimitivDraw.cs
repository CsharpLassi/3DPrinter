using System;
using System.Collections.Generic;

namespace DSVG
{
	public static class PrimitivDraw
	{
		public static IEnumerable<IPath> RectanglePath(int heigh, int width)
		{
			//Start
			yield return new StdPath (new Vector (-width / 2, heigh / 2), MoveType.StartPoint);

			yield return new StdPath (new Vector (width / 2, heigh / 2), MoveType.Line);
			yield return new StdPath (new Vector (width / 2, -heigh / 2), MoveType.Line);
			yield return new StdPath (new Vector (-width / 2, -heigh / 2), MoveType.Line);
			yield return new StdPath (new Vector (-width / 2, heigh / 2), MoveType.Line);

			yield return new StdPath (new Vector (-width / 2, heigh / 2), MoveType.EndPoint);
		}
	}
}

