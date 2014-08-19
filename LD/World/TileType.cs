// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD.World
{
	public enum TileType
	{
		[TileIndex( 254 )]
		[Walkable( true )]
		Grass,

		[TileIndex( 255 )]
		Stone,

		[TileIndex( 253 )]
		Chest,

		[TileIndex( 252 )]
		Water,

		[TileIndex( 251 )]
		Sign,

		[TileIndex( 250 )]
		Flower,

		[TileIndex( 232 )]
		Tree1
	}
}