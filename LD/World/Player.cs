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
using SharpDX.Toolkit.Input;

namespace LD.World
{
	internal class Player : Entity
	{
		#region Constructor

		public Player( Game game )
			: base( game )
		{
			Reset();
			Rand = new Random();
		}

		#endregion Constructor

		#region Methods

		public override void Draw( GameTime gameTime )
		{
			base.Draw( gameTime );

			int index = MoveDir * Constants.TileSize;
			Batch.Draw( TilesTexture,
				new RectangleF( -Constants.TileSize / 2.0f, -Constants.TileSize / 2.0f, Constants.TileSize, Constants.TileSize ),
				new Rectangle( index, ( UseAttackAnim ? 1 : 0 ) * Constants.TileSize, Constants.TileSize, Constants.TileSize ), Color.White );
		}

		public void Hit( float damage )
		{
			Sound.PlaySound( Rand.Next( 2 ) == 0 ? SoundType.Hurt1 : SoundType.Hurt2 );
			Health -= damage;
		}

		public override void Initialize()
		{
			base.Initialize();

			State = Game.Services.GetService<PlayState>();
			Sound = Game.Services.GetService<SoundManager>();
		}

		public override void Update( GameTime gameTime )
		{
			KeyboardState key = Game.Services.GetService<KeyboardManager>().GetState();
			Vector2 movment = new Vector2();

			if( key.IsKeyDown( Keys.W ) )
			{
				MoveDir = 1;
				movment.Y -= MovementSpeed;
			}
			else if( key.IsKeyDown( Keys.S ) )
			{
				MoveDir = 0;
				movment.Y += MovementSpeed;
			}

			if( key.IsKeyDown( Keys.A ) )
			{
				MoveDir = 2;
				movment.X -= MovementSpeed;
			}
			else if( key.IsKeyDown( Keys.D ) )
			{
				MoveDir = 3;
				movment.X += MovementSpeed;
			}

			movment.Normalize();
			movment *= MovementSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

			bool isAttacking = key.IsKeyDown( Keys.Space ) && AttacksLeft >= 1 && ( DateTime.Now - LastAttack ) > AttackDelay;
			UseAttackAnim = DateTime.Now - LastAttack < AttackDelay;
			bool countsAsAttack = isAttacking;

			if( isAttacking )
			{
				LastAttack = DateTime.Now;
			}

			if( movment != Vector2.Zero )
			{
				if( Collision.IsWalkable( movment ) )
				{
					Position += movment;
				}
			}

			float length = ( Level.SignPosition - Position ).Length();
			if( !WasAttacking && isAttacking && length <= ActionRadius )
			{
				countsAsAttack = false;
				Sound.PlaySound( SoundType.Select );
				State.ShowSign();
			}

			foreach( TileInfo inf in Level.GetChests() )
			{
				length = ( new Vector2( inf.BoundingBox.Center.X, inf.BoundingBox.Center.Y ) - Position ).Length();

				if( !WasAttacking && isAttacking && length <= ActionRadius )
				{
					countsAsAttack = false;
					State.OpenChest( inf );
				}
			}

			if( !WasAttacking && isAttacking )
			{
				if( countsAsAttack && AttacksLeft > 0 )
				{
					Mob m = Level.GetClosestMob( Position );
					if( m != null )
					{
						length = ( m.Position - Position ).Length();
						if( length <= AttackRange )
						{
							m.Hit( AttackDamage );
						}
					}

					if( countsAsAttack )
					{
						AttacksLeft--;
					}
				}
			}

			Health += HealthReg * (float)gameTime.ElapsedGameTime.TotalSeconds;
			if( Health > Constants.PlayerHealth )
			{
				Health = Constants.PlayerHealth;
			}

			AttacksLeft += AttackReg * (float)gameTime.ElapsedGameTime.TotalSeconds;
			if( AttacksLeft > Constants.PlayerAttacks )
			{
				AttacksLeft = Constants.PlayerAttacks;
			}

			WasAttacking = isAttacking;

			base.Update( gameTime );
		}

		internal void Reset()
		{
			Health = Constants.PlayerHealth;
			AttacksLeft = Constants.PlayerAttacks;
		}

		internal void SetCollision( Level level )
		{
			Level = level;
			Collision = new Collider( this, level );
		}

		#endregion Methods

		#region Properties

		public float AttacksLeft { get; private set; }

		public float Health { get; private set; }

		#endregion Properties

		#region Attributes

		private const float ActionRadius = 75;
		private const float AttackDamage = 10f;
		private const float AttackRange = 30;
		private const float AttackReg = 0.75f;
		private const float HealthReg = 1f;
		private const float MovementSpeed = 175.0f;
		private TimeSpan AttackDelay = TimeSpan.FromSeconds( 0.25 );
		private Collider Collision;
		private DateTime LastAttack;
		private Level Level;
		private int MoveDir = 0;
		private Random Rand;
		private SoundManager Sound;
		private PlayState State;

		// 0 = down, 1 = up, 2 = left, 3 = right
		private bool UseAttackAnim = false;

		private bool WasAttacking = false;

		#endregion Attributes
	}
}