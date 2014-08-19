// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;

namespace LD.States
{
	internal class GameOverState : Gamestate
	{
		#region Constructor

		public GameOverState( Game game )
			: base( game )
		{
		}

		#endregion Constructor

		#region Methods

		public override void Draw( GameTime gameTime )
		{
			base.Draw( gameTime );

			Batch.Begin();
			const int startOffset = 200;

			for( int i = 0; i < Text.Count; ++i )
			{
				string text = Text[i];

				Vector2 size = Font.MeasureString( text );
				Vector2 position = new Vector2( (int)( GraphicsDevice.BackBuffer.Width / 2 - size.X / 2 ), startOffset + i * 30 );

				Batch.DrawString( Font, text, position, Color.White );
			}
			Batch.End();
		}

		public override void Init()
		{
			base.Init();

			LastKeyboard = Game.Services.GetService<KeyboardManager>().GetState();

			Highscore score = new Highscore();
			score.AddScore( Play.Score );

			Text.Clear();
			Text.Add( "Awww you lost" );
			Text.Add( "" );
			Text.Add( "However you made" );
			Text.Add( Play.Score.ToString() );
			Text.Add( "Points" );
			Text.Add( "" );
			Text.Add( "Press Escape to go back to the menu" );
		}

		public override void Update( GameTime gameTime )
		{
			base.Update( gameTime );

			KeyboardState keyboard = Game.Services.GetService<KeyboardManager>().GetState();

			if( LastKeyboard.IsKeyUp( Keys.Escape ) && keyboard.IsKeyDown( Keys.Escape ) )
			{
				Game.Services.GetService<GamestateManager>().ChangeState( GamestateType.Menu );
			}

			LastKeyboard = keyboard;
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			Batch = Game.Services.GetService<SpriteBatch>();
			Font = Game.Content.Load<SpriteFont>( "Font" );

			Play = Game.Services.GetService<PlayState>();
		}

		#endregion Methods

		#region Properties

		#endregion Properties

		#region Attributes

		private SpriteBatch Batch;
		private SpriteFont Font;
		private KeyboardState LastKeyboard;
		private PlayState Play;
		private List<string> Text = new List<string>();

		#endregion Attributes
	}
}