// $Id$

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;

namespace LD.World
{
	internal class Level : Entity
	{
		#region Constructor

		public Level( Game game, Camera cam, Player player )
			: base( game )
		{
			MobCount = 3;
			Cam = cam;
			Player = player;

			player.SetCollision( this );
		}

		#endregion Constructor

		#region Methods

		public override void Draw( GameTime gameTime )
		{
			base.Draw( gameTime );

			for( int x = 0; x < Constants.LevelSize; ++x )
			{
				for( int y = 0; y < Constants.LevelSize; ++y )
				{
					TileInfo tile = GetTile( x, y );

					if( !IsCulled( tile.BoundingBox ) )
					{
						Batch.Draw( TilesTexture, tile.TransformedBoundingBox, tile.SourceRect, Color.White );
					}
				}
			}

			for( int x = 0; x < Constants.LevelSize; ++x )
			{
				for( int y = 0; y < Constants.LevelSize; ++y )
				{
					TileInfo item = GetItem( x, y );

					if( item != null )
					{
						if( !IsCulled( item.BoundingBox ) )
						{
							Batch.Draw( TilesTexture, item.BoundingBox, item.SourceRect, Color.White );
						}
					}
				}
			}

			for( int x = -1; x <= Constants.LevelSize; ++x )
			{
				TileInfo tile = new TileInfo()
				{
					BoundingBox = new Rectangle( x * Constants.TileSize, -Constants.TileSize, Constants.TileSize, Constants.TileSize ),
					Type = TileType.Stone
				};

				if( !IsCulled( tile.BoundingBox ) )
				{
					Batch.Draw( TilesTexture, tile.TransformedBoundingBox, tile.SourceRect, Color.White );
				}

				tile = new TileInfo()
				{
					BoundingBox = new Rectangle( x * Constants.TileSize, ( Constants.LevelSize ) * Constants.TileSize, Constants.TileSize, Constants.TileSize ),
					Type = TileType.Stone
				};

				if( !IsCulled( tile.BoundingBox ) )
				{
					Batch.Draw( TilesTexture, tile.TransformedBoundingBox, tile.SourceRect, Color.White );
				}
			}
			for( int y = -1; y <= Constants.LevelSize; ++y )
			{
				TileInfo tile = new TileInfo()
				{
					BoundingBox = new Rectangle( -Constants.TileSize, y * Constants.TileSize, Constants.TileSize, Constants.TileSize ),
					Type = TileType.Stone
				};

				if( !IsCulled( tile.BoundingBox ) )
				{
					Batch.Draw( TilesTexture, tile.TransformedBoundingBox, tile.SourceRect, Color.White );
				}

				tile = new TileInfo()
				{
					BoundingBox = new Rectangle( ( Constants.LevelSize ) * Constants.TileSize, y * Constants.TileSize, Constants.TileSize, Constants.TileSize ),
					Type = TileType.Stone
				};

				if( !IsCulled( tile.BoundingBox ) )
				{
					Batch.Draw( TilesTexture, tile.TransformedBoundingBox, tile.SourceRect, Color.White );
				}
			}

			foreach( Mob m in Mobs )
			{
				if( !IsCulled( m.BoundingBox ) )
				{
					m.Draw( gameTime );
				}
			}
		}

		public void Generate()
		{
			Mobs.Clear();

			Generator = new LevelGenerator();
			Tiles = new List<TileInfo>( Constants.LevelSize * Constants.LevelSize );

			for( int x = 0; x < Constants.LevelSize; ++x )
			{
				for( int y = 0; y < Constants.LevelSize; ++y )
				{
					//Tiles[x * Constants.LevelSize + y] = new TileInfo()
					Tiles.Add( new TileInfo()
					{
						Type = Generator.GetTile( x, y ),
						BoundingBox = new Rectangle( x * Constants.TileSize, y * Constants.TileSize, Constants.TileSize, Constants.TileSize )
					} );
				}
			}

			Items = new List<TileInfo>();
			foreach( Point p in Generator.Chests )
			{
				Items.Add( new TileInfo()
				{
					Type = TileType.Chest,
					BoundingBox = new Rectangle( p.X * Constants.TileSize, p.Y * Constants.TileSize, Constants.TileSize, Constants.TileSize )
				} );
			}

			WinningChest = Generator.WinningChest;
			WinningChestIndex = Generator.WinningChestIndex;

			Items.Add( new TileInfo()
			{
				Type = TileType.Sign,
				BoundingBox = new Rectangle( Generator.Sign.X * Constants.TileSize, Generator.Sign.Y * Constants.TileSize, Constants.TileSize, Constants.TileSize )
			} );

			foreach( FlowerInfo inf in Generator.Flowers )
			{
				Items.Add( new TileInfo()
					{
						Type = TileType.Flower,
						FullTile = false,
						xOffset = ( inf.Type % 2 ) * Constants.TileSizeHalf,
						yOffset = ( inf.Type / 2 ) * Constants.TileSizeHalf,
						BoundingBox = new Rectangle( inf.Position.X * Constants.TileSize, inf.Position.Y * Constants.TileSize, Constants.TileSizeHalf, Constants.TileSizeHalf )
					} );
			}

			foreach( TreeInfo inf in Generator.Trees )
			{
				Items.Add( new TileInfo()
				{
					Type = inf.Type,
					BoundingBox = new Rectangle( inf.Position.X * Constants.TileSize, inf.Position.Y * Constants.TileSize, Constants.TileSizeDouble, Constants.TileSizeDouble )
				} );
			}

			SignPosition = new Vector2( Generator.Sign.X * Constants.TileSize, Generator.Sign.Y * Constants.TileSize );

			Player.Position = new Vector2( Generator.Spawn.X, Generator.Spawn.Y ) * Constants.TileSize;

			for( int i = 0; i < MobCount; ++i )
			{
				GenerateMob();
			}
		}

		public IEnumerable<TileInfo> GetChests()
		{
			return Items.Where( it => it.Type == TileType.Chest );
		}

		public TileInfo GetTile( int x, int y )
		{
			return Tiles[x * Constants.LevelSize + y];
		}

		public Point GetTileIndex( int x, int y )
		{
			return new Point( x / Constants.TileSize, y / Constants.TileSize );
		}

		public Point GetTileIndex( Vector2 pos )
		{
			return GetTileIndex( (int)pos.X, (int)pos.Y );
		}

		public override void Initialize()
		{
			base.Initialize();

			State = Game.Services.GetService<PlayState>();
		}

		public override void Update( GameTime gameTime )
		{
			base.Update( gameTime );

			List<Mob> toDelete = new List<Mob>();
			foreach( Mob m in Mobs )
			{
				m.Update( gameTime );
				if( m.Health < 0 )
				{
					toDelete.Add( m );
				}
			}

			State.KillMobs( toDelete.Count );

			foreach( Mob m in toDelete )
			{
				Mobs.Remove( m );
			}
		}

		internal Mob GetClosestMob( Vector2 Position )
		{
			Mob minMob = null;
			float minDist = float.MaxValue;

			foreach( Mob m in Mobs )
			{
				float dist = ( m.Position - Position ).Length();

				if( dist < minDist )
				{
					minMob = m;
					minDist = dist;
				}
			}

			return minMob;
		}

		private void GenerateMob()
		{
			Mob m = new Mob( Game, Player );
			m.Initialize();
			m.Load();
			m.SetLevel( this );

			m.Position = Generator.GetMobPosition();

			Debug.WriteLine( m.Position.ToString() );

			Mobs.Add( m );
		}

		private TileInfo GetItem( int x, int y )
		{
			foreach( TileInfo inf in Items )
			{
				if( inf.BoundingBox.Top == y * Constants.TileSize && inf.BoundingBox.Left == x * Constants.TileSize )
				{
					return inf;
				}
			}

			return null;
		}

		private bool IsCulled( Rectangle boundingBox )
		{
			return !(
				Cam.ViewFrustum.Contains( boundingBox.TopLeft ) || Cam.ViewFrustum.Contains( boundingBox.BottomRight )
				|| Cam.ViewFrustum.Contains( boundingBox.TopRight ) || Cam.ViewFrustum.Contains( boundingBox.BottomLeft ) );
		}

		private bool IsCulled( RectangleF rect )
		{
			Rectangle r = new Rectangle( (int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height );

			return IsCulled( r );
		}

		#endregion Methods

		#region Properties

		public int MobCount { get; set; }

		public Vector2 SignPosition { get; private set; }

		public Point WinningChest { get; private set; }

		public int WinningChestIndex { get; private set; }

		#endregion Properties

		#region Attributes

		private Camera Cam;
		private LevelGenerator Generator;
		private List<TileInfo> Items;
		private List<Mob> Mobs = new List<Mob>();
		private Player Player;
		private PlayState State;
		private List<TileInfo> Tiles;

		#endregion Attributes
	}
}