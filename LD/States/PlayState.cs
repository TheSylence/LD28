// $Id$

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LD.Sound;
using LD.World;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;

namespace LD
{
	internal class PlayState : Gamestate
	{
		#region Constructor

		public PlayState( Game game )
			: base( game )
		{
			Game.Services.AddService<PlayState>( this );

			Cam = new Camera( Game );
			Player = new Player( Game );
			Cam.EntityToFollow = Player;
			Entities.Add( Cam );
			Level = new World.Level( Game, Cam, Player );
			Entities.Add( Level );
		}

		#endregion Constructor

		#region Methods

		public override void Draw( GameTime gameTime )
		{
			Batch.Begin( SpriteSortMode.Deferred, GraphicsDevice.BlendStates.NonPremultiplied, GraphicsDevice.SamplerStates.PointClamp, (DepthStencilState)null, (RasterizerState)null, (Effect)null, Matrix.Invert( Cam.ViewMatrix ) );

			foreach( Entity entity in Entities )
			{
				entity.Draw( gameTime );
			}

			Batch.End();

			Batch.Begin( SpriteSortMode.Deferred, GraphicsDevice.BlendStates.NonPremultiplied, GraphicsDevice.SamplerStates.PointClamp, (DepthStencilState)null, (RasterizerState)null, (Effect)null, Matrix.Translation( Game.GraphicsDevice.BackBuffer.Width / 2, Game.GraphicsDevice.BackBuffer.Height / 2, 0 ) );

			Player.Draw( gameTime );

			Batch.End();

			Batch.Begin( SpriteSortMode.Deferred, GraphicsDevice.BlendStates.NonPremultiplied, GraphicsDevice.SamplerStates.PointClamp );
			Batch.DrawString( Font, string.Format( "Level: {0}", LevelNum ), new Vector2( 10, 10 ), Color.Blue );
			Batch.DrawString( Font, string.Format( "Score: {0}", Score ), new Vector2( 10, 30 ), Color.Blue );

			// 448, 416
			const int xOff = 10;
			int y = GraphicsDevice.BackBuffer.Height - Constants.TileSizeHalf - xOff;
			Rectangle source = new Rectangle( 448, 416, Constants.TileSizeHalf, Constants.TileSizeHalf );
			const int maxHearts = 10;
			int x = xOff;
			int hearts = (int)( Player.Health * 2 ) / 10;

			source.Y += Constants.TileSizeHalf;
			for( int i = 0; i < maxHearts; ++i )
			{
				// Empty
				RectangleF dest = new RectangleF( x, y, Constants.TileSizeHalf, Constants.TileSizeHalf );
				Batch.Draw( TilesTexture, dest, source, Color.White );
				x += Constants.TileSizeHalf;
			}

			x = xOff;
			source = new Rectangle( 448, 416, Constants.TileSizeHalf, Constants.TileSizeHalf );
			for( int i = 0; i < hearts / 2; ++i )
			{
				// Full
				RectangleF dest = new RectangleF( x, y, Constants.TileSizeHalf, Constants.TileSizeHalf );
				Batch.Draw( TilesTexture, dest, source, Color.White );
				x += Constants.TileSizeHalf;
			}

			source.X += Constants.TileSizeHalf;
			if( hearts > hearts / 2 * 2 )
			{
				// Half
				RectangleF dest = new RectangleF( x, y, Constants.TileSizeHalf, Constants.TileSizeHalf );
				Batch.Draw( TilesTexture, dest, source, Color.White );
				x += Constants.TileSizeHalf;
			}

			y -= xOff * 2;

			// 416, 416
			source = new Rectangle( 416, 416, Constants.TileSizeHalf, Constants.TileSizeHalf );
			for( int i = 0; i < Player.AttacksLeft; ++i )
			{
				RectangleF dest = new RectangleF( xOff + i * Constants.TileSize * 0.5f, y, Constants.TileSize * 0.5f, Constants.TileSize * 0.5f );
				Batch.Draw( TilesTexture, dest, source, Color.White );
			}

			source.X += Constants.TileSizeHalf;
			for( int i = (int)Player.AttacksLeft; i < Constants.PlayerAttacks; ++i )
			{
				RectangleF dest = new RectangleF( xOff + i * Constants.TileSize * 0.5f, y, Constants.TileSize * 0.5f, Constants.TileSize * 0.5f );
				Batch.Draw( TilesTexture, dest, source, Color.White );
			}

			Batch.End();
		}

		public override void Init()
		{
			base.Init();

			LevelNum = 1;
			Score = 0;
			MobCount = 3;

			Player.Reset();
			LastKeyboard = Game.Services.GetService<KeyboardManager>().GetState();

			Gui = Game.Services.GetService<Gui>();
			GenerateLevel();

			Sound = Game.Services.GetService<SoundManager>();
		}

		public override void Initialize()
		{
			base.Initialize();

			Batch = Game.Services.GetService<SpriteBatch>();
			Font = Game.Content.Load<SpriteFont>( "Font" );

			foreach( Entity entity in Entities )
			{
				entity.Initialize();
			}

			Player.Initialize();
		}

		public override void Update( GameTime gameTime )
		{
			base.Update( gameTime );

			if( Player.Health < 0 )
			{
				Game.Services.GetService<GamestateManager>().ChangeState( GamestateType.GameOver );
				return;
			}

			KeyboardState key = Game.Services.GetService<KeyboardManager>().GetState();
			bool isSpacePressed = key.IsKeyDown( Keys.Space );

			if( !Gui.Visible )
			{
				foreach( Entity entity in Entities )
				{
					entity.Update( gameTime );
				}
				Player.Update( gameTime );

				if( LastKeyboard.IsKeyUp( Keys.Escape ) && key.IsKeyDown( Keys.Escape ) )
				{
					Game.Services.GetService<GamestateManager>().PushState( GamestateType.Pause );
				}
			}
			else
			{
				if( LastKeyboard.IsKeyUp( Keys.Space ) && isSpacePressed )
				{
					Gui.Visible = false;
				}
			}

			LastKeyboard = key;
		}

		internal void KillMobs( int amount )
		{
			int factor = (int)Math.Ceiling( Math.Log( MobCount ) );
			Score += (ulong)( amount * factor );
		}

		internal void OpenChest( TileInfo inf )
		{
			Point pt = inf.TilePosition;

			Sound.PlaySound( SoundType.OpenChest );

			if( Level.WinningChest == pt )
			{
				Debug.WriteLine( "Win" );
				Score += (ulong)LevelsWon;

				LevelsWon++;
			}
			else
			{
				Debug.WriteLine( "Loose" );

				MobCount++;
			}

			LevelNum++;
			Player.Reset();
			GenerateLevel();
		}

		internal void ShowSign()
		{
			if( !Gui.Visible )
			{
				Gui.SetText( LevelHint );
			}
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			TilesTexture = Game.Content.Load<Texture>( "Tiles.png" );

			foreach( Entity entity in Entities )
			{
				entity.Load();
			}

			Player.Load();
		}

		private void GenerateLevel()
		{
			Level.MobCount = MobCount;
			Level.Generate();
			LevelHint = Gui.GetRandomText( Level.WinningChestIndex );
		}

		#endregion Methods

		#region Properties

		public Camera Cam { get; private set; }

		public Level Level { get; private set; }

		public int LevelNum { get; private set; }

		public ulong Score { get; private set; }

		private Player Player { get; set; }

		#endregion Properties

		#region Attributes

		private SpriteBatch Batch;
		private List<Entity> Entities = new List<Entity>();
		private SpriteFont Font;
		private Gui Gui;
		private KeyboardState LastKeyboard;
		private string LevelHint;
		private int LevelsWon = 0;
		private int MobCount;
		private SoundManager Sound;
		private Texture TilesTexture;

		#endregion Attributes
	}
}