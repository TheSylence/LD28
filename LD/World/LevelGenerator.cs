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
	public class LevelGenerator
	{
		// TODO: Noise code ersetzen

		#region Constructor

		public LevelGenerator( int? seed = null )
		{
			if( seed.HasValue )
			{
				Rand = new Random( seed.Value );
			}
			else
			{
				Rand = new Random();
			}

			GenerateAndValidate();
		}

		#endregion Constructor

		#region Methods

		public TileType GetTile( int x, int y )
		{
			return TileData[x * Constants.LevelSize + y];
		}

		internal Vector2 GetMobPosition()
		{
			int q = Rand.Next( 1, 4 );
			int tries = 0;

			while( true && tries++ < 10000 )
			{
				int x = Rand.Next( Constants.LevelSize / 2 * q % 2, Constants.LevelSize / 2 * ( q % 2 + 1 ) );
				int y = Rand.Next( Constants.LevelSize / 2 * q / 2, Constants.LevelSize / 2 * ( q / 2 + 1 ) );

				if( GetTile( x, y ) == TileType.Grass )
				{
					return new Vector2( x * Constants.TileSize, y * Constants.TileSize );
				}
			}

			return new Vector2( 40 * Constants.TileSize, 40 * Constants.TileSize );
		}

		private void AddChests()
		{
			Chests = new List<Point>();

			int cnt = 0;
			bool found = false;
			// Top right
			while( !found )
			{
				++cnt;
				if( cnt > 5000 )
					break;

				int x = Rand.Next( Constants.LevelSize / 2, Constants.LevelSize );
				int y = Rand.Next( Constants.LevelSize / 2 );

				if( GetTile( x, y ) == TileType.Grass )
				{
					Chests.Add( new Point( x, y ) );
					found = true;
				}
			}

			// Bottom left
			cnt = 0;
			found = false;
			while( !found )
			{
				++cnt;
				if( cnt > 5000 )
					break;

				int x = Rand.Next( Constants.LevelSize / 2 );
				int y = Rand.Next( Constants.LevelSize / 2, Constants.LevelSize );

				if( GetTile( x, y ) == TileType.Grass )
				{
					Chests.Add( new Point( x, y ) );
					found = true;
				}
			}

			// Bottom right
			cnt = 0;
			found = false;
			while( !found )
			{
				++cnt;
				if( cnt > 5000 )
					break;

				int x = Rand.Next( Constants.LevelSize / 2, Constants.LevelSize );
				int y = Rand.Next( Constants.LevelSize / 2, Constants.LevelSize );

				if( GetTile( x, y ) == TileType.Grass )
				{
					Chests.Add( new Point( x, y ) );
					found = true;
				}
			}

			WinningChestIndex = Rand.Next( Chests.Count );
			WinningChest = Chests[WinningChestIndex];
		}

		private void Generate()
		{
			Rand.NextBytes( SimplexNoise.perm );

			for( int x = 0; x < Constants.LevelSize; ++x )
			{
				for( int y = 0; y < Constants.LevelSize; ++y )
				{
					double v = GenerateFractal( x, y, 4 );

					if( v < -0.95 )
					{
						SetTile( x, y, TileType.Water );
					}
					else if( v < 0.5 )
					{
						SetTile( x, y, TileType.Grass );
					}
					else
					{
						SetTile( x, y, TileType.Stone );
					}
				}
			}

			Flowers = new List<FlowerInfo>();
			int numFlowers = Rand.Next( 100, 750 );
			for( int i = 0; i < numFlowers; ++i )
			{
				bool found = false;
				int tries = 0;
				while( !found && ++tries < 1000 )
				{
					int x = Rand.Next( Constants.LevelSize );
					int y = Rand.Next( Constants.LevelSize );

					if( GetTile( x, y ) == TileType.Grass )
					{
						Flowers.Add( new FlowerInfo() { Position = new Point( x, y ), Type = Rand.Next( 0, 4 ) } );
						found = true;
					}
				}
			}

			int numTrees = Rand.Next( 25, 175 );
			Trees = new List<TreeInfo>();
			for( int i = 0; i < numTrees; ++i )
			{
				bool found = false;
				int tries = 0;
				while( !found && ++tries < 1000 )
				{
					int x = Rand.Next( Constants.LevelSize - 2 );
					int y = Rand.Next( Constants.LevelSize - 2 );

					if( GetTile( x, y ) == TileType.Grass && GetTile( x + 1, y ) == TileType.Grass &&
						GetTile( x, y + 1 ) == TileType.Grass && GetTile( x + 1, y + 1 ) == TileType.Grass )
					{
						Trees.Add( new TreeInfo() { Position = new Point( x, y ), Type = TileType.Tree1 } );
						found = true;
					}
				}
			}
		}

		private void GenerateAndValidate()
		{
			Generate();

			int cnt = 0;
			bool valid = true;
			do
			{
				valid = true;
				cnt++;
				for( int x = 0; x < 2; ++x )
				{
					for( int y = 0; y < 2; ++y )
					{
						int qx = x * Constants.LevelSize / 2;
						int qy = y * Constants.LevelSize / 2;

						int grass = 0;
						for( int tx = 0; tx < Constants.LevelSize / 2; ++tx )
						{
							for( int ty = 0; ty < Constants.LevelSize / 2; ++ty )
							{
								if( GetTile( qx + tx, qy + ty ) == TileType.Grass )
								{
									grass++;
								}
							}
						}

						int total = Constants.LevelSize / 2 * Constants.LevelSize / 2;
						if( grass / (float)total < 0.45f )
						{
							valid = false;
							break;
						}
					}
				}

				SetSpawn();

				for( int x = -1; x <= 1 && valid; ++x )
				{
					for( int y = -1; y <= 1 && valid; ++y )
					{
						int sx = Spawn.X + x;
						int sy = Spawn.Y + y;

						if( sx <= 0 || sy <= 0 || GetTile( sx, sy ) != TileType.Grass )
						{
							valid = false;
							break;
						}
					}
				}

				AddChests();
				foreach( Point p in Chests )
				{
					for( int x = -1; x <= 1 && valid; ++x )
					{
						for( int y = -1; y <= 1 && valid; ++y )
						{
							int sx = p.X + x;
							int sy = p.Y + y;

							if( sx <= 0 || sy <= 0 || sx == Constants.LevelSize || sy == Constants.LevelSize || GetTile( sx, sy ) != TileType.Grass )
							{
								valid = false;
								break;
							}
						}
					}

					if( !valid )
						break;
				}

				if( !valid )
				{
					Generate();
				}
			} while( !valid );

			SetSign();

			Debug.WriteLine( "{0}. level", cnt );
		}

		private double GenerateFractal( float x, float y, int octaves )
		{
			double value = 0;
			float frequency = 100.0f;

			for( int i = 0; i < octaves; ++i )
			{
				value += SimplexNoise.Generate( x / frequency, y / frequency ) * Math.Pow( 2, -i );

				frequency *= 0.5f;
			}

			return value;
		}

		private void SetSign()
		{
			int radius = 2;

			for( int x = -radius; x <= radius; ++x )
			{
				for( int y = -radius; y <= radius; ++y )
				{
					if( x == 0 && y == 0 )
						continue;

					Point p = new Point( x + Spawn.X, y + Spawn.Y );

					if( p.X < 0 || p.Y < 0 )
						continue;

					if( GetTile( p.X, p.Y ) == TileType.Grass )
					{
						Sign = p;
						break;
					}
				}
			}
		}

		private void SetSpawn()
		{
			bool found = false;

			while( !found )
			{
				int x = Rand.Next( Constants.LevelSize / 4 );
				int y = Rand.Next( Constants.LevelSize / 4 );

				if( GetTile( x, y ) == TileType.Grass )
				{
					Spawn = new Point( x, y );
					found = true;
				}
			}
		}

		private void SetTile( int x, int y, TileType type )
		{
			TileData[x * Constants.LevelSize + y] = type;
		}

		#endregion Methods

		#region Properties

		public List<Point> Chests { get; private set; }

		public List<FlowerInfo> Flowers { get; private set; }

		public Point Sign { get; private set; }

		public Point Spawn { get; private set; }

		public List<TreeInfo> Trees { get; private set; }

		public Point WinningChest { get; private set; }

		public int WinningChestIndex { get; private set; }

		#endregion Properties

		#region Attributes

		private Random Rand;
		private TileType[] TileData = new TileType[Constants.LevelSize * Constants.LevelSize];

		#endregion Attributes
	}
}