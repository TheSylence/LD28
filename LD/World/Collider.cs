// $Id$

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace LD.World
{
	internal class Collider
	{
		#region Constructor

		public Collider( Entity entity, Level level )
		{
			Entity = entity;
			Level = level;
		}

		#endregion Constructor

		#region Methods

		public bool IsWalkable( Vector2 movement )
		{
			Vector2 offset = new Vector2( Entity.BoundingBox.Width / 2.0f, Entity.BoundingBox.Height / 2.0f );

			bool w1, w2, w3, w4;
			w1 = IsWalkablePoint( Entity.BoundingBox.TopLeft + movement - offset );
			w2 = IsWalkablePoint( Entity.BoundingBox.TopRight + movement - offset );
			w3 = IsWalkablePoint( Entity.BoundingBox.BottomRight + movement - offset );
			w4 = IsWalkablePoint( Entity.BoundingBox.BottomLeft + movement - offset );

			return w1 && w2 && w3 && w4;
		}

		private bool IsWalkablePoint( Vector2 pos )
		{
			Point tile = Level.GetTileIndex( (int)pos.X, (int)pos.Y );
			if( tile.X < 0 || tile.Y < 0 || tile.X >= Constants.LevelSize || tile.Y >= Constants.LevelSize )
			{
				return false;
			}

			TileInfo inf = Level.GetTile( tile.X, tile.Y );

			return inf.IsWalkable;
		}

		#endregion Methods

		#region Properties

		#endregion Properties

		#region Attributes

		private Entity Entity;
		private Level Level;

		#endregion Attributes
	}
}