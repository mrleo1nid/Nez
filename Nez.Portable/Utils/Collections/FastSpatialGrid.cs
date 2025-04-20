using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Nez
{
	public class FastSpatialGrid<T>
	{
		private readonly float cellSize;
		private readonly int maxCapacityPerCell;
		private readonly Func<T, Vector2> getPosition;
		private readonly Dictionary<long, List<T>> grid;

		public FastSpatialGrid(float cellSize, Func<T, Vector2> getPosition, int maxCapacityPerCell = 16)
		{
			this.cellSize = cellSize;
			this.getPosition = getPosition;
			this.maxCapacityPerCell = maxCapacityPerCell;
			this.grid = new Dictionary<long, List<T>>(capacity: 2048);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private long HashCell(int x, int y)
		{
			// Быстрый хэш из двух int
			return ((long)x << 32) | (uint)y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private long HashCell(Vector2 position)
		{
			int x = (int)(position.X / cellSize);
			int y = (int)(position.Y / cellSize);
			return HashCell(x, y);
		}

		public void Add(T item)
		{
			var pos = getPosition(item);
			long hash = HashCell(pos);

			if (!grid.TryGetValue(hash, out var list))
			{
				list = new List<T>(maxCapacityPerCell);
				grid[hash] = list;
			}

			list.Add(item);
		}

		public void Clear()
		{
			foreach (var kv in grid)
			{
				kv.Value.Clear();
			}

			grid.Clear();
		}

		public List<T> Query(Vector2 position)
		{
			long hash = HashCell(position);
			return grid.TryGetValue(hash, out var list) ? list : null;
		}

		public List<T> QueryArea(RectangleF area, List<T> resultBuffer = null)
		{
			if (resultBuffer == null)
				resultBuffer = new List<T>();

			int minX = (int)(area.Left / cellSize);
			int maxX = (int)(area.Right / cellSize);
			int minY = (int)(area.Top / cellSize);
			int maxY = (int)(area.Bottom / cellSize);

			for (int y = minY; y <= maxY; y++)
			{
				for (int x = minX; x <= maxX; x++)
				{
					long hash = HashCell(x, y);
					if (grid.TryGetValue(hash, out var list))
					{
						resultBuffer.AddRange(list);
					}
				}
			}

			return resultBuffer;
		}
	}
}
