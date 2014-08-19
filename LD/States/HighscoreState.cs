using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;

namespace LD.States
{
	internal class HighscoreState : Gamestate
	{
		#region Constructor

		public HighscoreState( Game game )
			: base( game )
		{
		}

		#endregion Constructor

		#region Methods

		public override void Draw( GameTime gameTime )
		{
			base.Draw( gameTime );

			Batch.Begin();

			int startOffset = 100;
			Vector2 position = new Vector2();
			Vector2 size;
			for( int i = 0; i < Scores.Count; ++i )
			{
				string text = Scores[i];

				size = Font.MeasureString( text );
				position = new Vector2( (int)( GraphicsDevice.BackBuffer.Width / 2 - size.X / 2 ), startOffset + i * 30 );

				Batch.DrawString( Font, text, position, Color.White );
			}

			Batch.End();
		}

		public override void Init()
		{
			base.Init();

			Scores.Clear();

			Highscore score = new Highscore();
			foreach( ulong s in score.GetHighscores() )
			{
				Scores.Add( s.ToString() );
			}

			Scores.Add( string.Empty );
			Scores.Add( string.Empty );
			Scores.Add( "Press Escape to go back to the menu" );

			LastKeyboard = Game.Services.GetService<KeyboardManager>().GetState();
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
		}

		#endregion Methods

		#region Properties

		#endregion Properties

		#region Attributes

		private SpriteBatch Batch;
		private SpriteFont Font;
		private KeyboardState LastKeyboard;
		private List<string> Scores = new List<string>();

		#endregion Attributes
	}
}