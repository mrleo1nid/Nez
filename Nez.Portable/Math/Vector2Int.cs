using System;
using System.Collections.Generic;
using System.Text;

namespace Nez
{
    public class Vector2Int
    {
		public int X { get; set; }
		public int Y { get; set; }
		public Vector2Int(int x, int y)
		{
			X = x;
			Y = y;
		}
		public static Vector2Int Zero = new Vector2Int(0, 0);
	}
}
