// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LD.Sound;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;

namespace LD
{
	internal class MenuState : Gamestate
	{
		#region Constructor

		public MenuState( Game game )
			: base( game )
		{
			Options.Add( "Start Game" );
			Options.Add( "How to play" );
			Options.Add( "Highscores" );
			Options.Add( "Quit" );
		}

		#endregion Constructor

		#region Methods

		public override void Draw( GameTime gameTime )
		{
			base.Draw( gameTime );

			Batch.Begin( SpriteSortMode.Deferred, GraphicsDevice.BlendStates.NonPremultiplied, GraphicsDevice.SamplerStates.PointClamp );

			const int width = 256;
			const int height = 128;
			Rectangle source = new Rectangle( 0, 480, 64, 32 );
			RectangleF dest = new RectangleF( (int)( GraphicsDevice.BackBuffer.Width / 2 - width / 2 ), 50, width, height );

			Batch.Draw( TilesTexture, dest, source, Color.White );

			const int startOffset = 400;

			for( int i = 0; i < Options.Count; ++i )
			{
				string text = Options[i];
				if( i == SelectedIndex )
				{
					text = "> " + text + " <";
				}

				Vector2 size = Font.MeasureString( text );
				Vector2 position = new Vector2( (int)( GraphicsDevice.BackBuffer.Width / 2 - size.X / 2 ), startOffset + i * 30 );

				Batch.DrawString( Font, text, position, i != SelectedIndex ? Color.White : Color.LightGray );
			}

			Batch.End();
		}

		public override void Init()
		{
			base.Init();

			LastKeyboard = Game.Services.GetService<KeyboardManager>().GetState();
		}

		public override void Update( GameTime gameTime )
		{
			base.Update( gameTime );

			//Game.Services.GetService<GamestateManager>().ChangeState( GamestateType.Game );
			//return;

			KeyboardState keyboard = Game.Services.GetService<KeyboardManager>().GetState();
			if( LastKeyboard.IsKeyUp( Keys.Down ) && keyboard.IsKeyDown( Keys.Down ) )
			{
				SelectedIndex = ( ++SelectedIndex ) % Options.Count;
			}
			else if( LastKeyboard.IsKeyUp( Keys.Up ) && keyboard.IsKeyDown( Keys.Up ) )
			{
				SelectedIndex--;

				if( SelectedIndex < 0 )
				{
					SelectedIndex = Options.Count - 1;
				}
			}
			else if( ( LastKeyboard.IsKeyUp( Keys.Space ) && keyboard.IsKeyDown( Keys.Space ) )
				|| ( LastKeyboard.IsKeyUp( Keys.Enter ) && keyboard.IsKeyDown( Keys.Enter ) ) )
			{
				Game.Services.GetService<SoundManager>().PlaySound( SoundType.Menu );

				switch( SelectedIndex )
				{
					case 0:
						Game.Services.GetService<GamestateManager>().ChangeState( GamestateType.Game );
						break;

					case 1:
						Game.Services.GetService<GamestateManager>().ChangeState( GamestateType.Help );
						break;

					case 2:
						Game.Services.GetService<GamestateManager>().ChangeState( GamestateType.Highscore );
						break;

					case 3:
						Game.Exit();
						break;
				}
			}

			LastKeyboard = keyboard;
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			Batch = Game.Services.GetService<SpriteBatch>();
			Font = Game.Content.Load<SpriteFont>( "Font" );
			TilesTexture = Game.Content.Load<Texture>( "Tiles.png" );
		}

		#endregion Methods

		#region Properties

		#endregion Properties

		#region Attributes

		private SpriteBatch Batch;
		private SpriteFont Font;
		private KeyboardState LastKeyboard;
		private List<string> Options = new List<string>();
		private int SelectedIndex = 0;
		private Texture TilesTexture;

		#endregion Attributes
	}
}