// $Id$

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LD.Sound;
using SharpDX;
using SharpDX.Toolkit;

namespace LD.World
{
	internal class Mob : Entity
	{
		#region Constructor

		public Mob( Game game, Player player )
			: base( game )
		{
			Health = Constants.MobHealth;
			Player = player;
			Rand = new Random();
		}

		#endregion Constructor

		#region Methods

		public override void Draw( GameTime gameTime )
		{
			base.Draw( gameTime );

			int index = ( 4 + MoveDir ) * Constants.TileSize;

			RectangleF rect = BoundingBox;
			rect.X -= Constants.TileSize / 2.0f;
			rect.Y -= Constants.TileSize / 2.0f;

			Batch.Draw( TilesTexture,
				rect,
				new Rectangle( index, 0 * Constants.TileSize, Constants.TileSize, Constants.TileSize ), Color.White );

			// 50 => 0
			// 40 => 1
			// 30 => 2
			// 20 => 3
			// 10 => 4

			// 368, 416
			index = (int)( Constants.MobHealth - Health ) / 10;

			int c = index % 3;
			int r = index / 3;

			Batch.Draw( TilesTexture,
				rect,
				new Rectangle( 368 + c * Constants.TileSizeHalf, 416 + r * Constants.TileSizeHalf, Constants.TileSizeHalf, Constants.TileSizeHalf ), Color.White );
		}

		public void Hit( float damage )
		{
			Health -= damage;
			Game.Services.GetService<SoundManager>().PlaySound( SoundType.Hit );
		}

		public void SetLevel( Level level )
		{
			Collision = new Collider( this, level );
		}

		public override void Update( GameTime gameTime )
		{
			base.Update( gameTime );

			float length = ( Player.Position - Position ).Length();
			SeesPlayer = length <= SightRange;

			Vector2 movement = Vector2.Zero;

			if( SeesPlayer )
			{
				if( length <= AttackRange )
				{
					if( DateTime.Now - LastAttack > AttackDelay )
					{
						Player.Hit( AttackDamage );
						LastAttack = DateTime.Now;
					}
				}
				else
				{
					movement = Player.Position - Position;
					movement.Normalize();

					movement *= RunSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
				}
			}
			else
			{
				if( DateTime.Now > NextDirChange )
				{
					float newDir = Rand.Next( 0, 360 );

					Vector2 v = Vector2.UnitX;
					v = v.Rotate( newDir, Vector2.Zero );

					v.Normalize();

					WalkDir = v;

					NextDirChange = DateTime.Now.AddSeconds( Rand.Next( 15, 45 ) );
				}

				movement = WalkDir * WalkSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
			}

			if( Collision.IsWalkable( movement ) )
			{
				Position += movement;
			}
			else if( !SeesPlayer )
			{
				WalkDir *= -1;
			}

			double angle = MathUtil.RadiansToDegrees( (float)Math.Acos( Vector2.Dot( -Vector2.UnitY, movement ) / ( movement.Length() ) ) );

			bool isLeft;
			if( SeesPlayer )
			{
				isLeft = BoundingBox.Left < Player.BoundingBox.Left;
			}
			else
			{
				isLeft = WalkDir.X > 0;
			}

			if( angle > 45.0f && angle < 135.0f )
			{
				MoveDir = isLeft ? 3 : 2;
			}
			else if( angle > 135.0f )
			{
				MoveDir = 0;
			}
			else if( angle < 45.0f )
			{
				MoveDir = 1;
			}
		}

		#endregion Methods

		#region Properties

		public float Health { get; private set; }

		#endregion Properties

		#region Attributes

		private const float AttackDamage = 5.0f;
		private const float AttackRange = 25;
		private const float RunSpeed = 176;
		private const float SightRange = 175;
		private const float WalkSpeed = 85;
		private TimeSpan AttackDelay = TimeSpan.FromSeconds( 1 );
		private Collider Collision;
		private DateTime LastAttack = DateTime.Now;
		private int MoveDir = 0;
		private DateTime NextDirChange;
		private Player Player;
		private Random Rand;
		private bool SeesPlayer = false;
		private Vector2 WalkDir;

		#endregion Attributes
	}
}