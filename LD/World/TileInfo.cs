// $Id$

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace LD.World
{
	internal class TileInfo
	{
		public Rectangle BoundingBox;
		public bool FullTile = true;
		public TileType Type;
		public int xOffset = 0;
		public int yOffset = 0;

		public bool IsWalkable
		{
			get
			{
				WalkableAttribute attr = typeof( TileType ).GetMember( Type.ToString() ).First().GetCustomAttribute<WalkableAttribute>();
				if( attr == null )
				{
					return false;
				}

				return attr.Walkable;
			}
		}

		public Rectangle SourceRect
		{
			get
			{
				int tileIndex = typeof( TileType ).GetMember( Type.ToString() ).First().GetCustomAttribute<TileIndexAttribute>().Index;

				int tx = tileIndex % Constants.TilesPerRow;
				int ty = tileIndex / Constants.TilesPerRow;

				int size = IsTree ? Constants.TileSizeDouble : ( FullTile ? Constants.TileSize : Constants.TileSizeHalf );
				return new Rectangle( xOffset + tx * Constants.TileSize, yOffset + ty * Constants.TileSize, size, size );
			}
		}

		public Point TilePosition
		{
			get
			{
				return new Point( BoundingBox.Center.X / Constants.TileSize, BoundingBox.Center.Y / Constants.TileSize );
			}
		}

		public RectangleF TransformedBoundingBox
		{
			get
			{
				RectangleF b = BoundingBox;
				//b.Left -= b.Width / 2;
				//b.Top -= b.Height / 2;

				//b.Left += 0.5f;
				//b.Top += 0.5f;

				return b;
			}
		}

		private bool IsTree
		{
			get
			{
				return Type == TileType.Tree1;
			}
		}
	}
}