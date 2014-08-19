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

namespace LD
{
	internal class HelpState : Gamestate
	{
		#region Constructor

		public HelpState( Game game )
			: base( game )
		{
			Text.Add( "Three chests but only one key" );
			Text.Add( "Find the right chest and open it" );
			Text.Add( "You get one hint on the right one" );
			Text.Add( "If you open the wrong one the next level" );
			Text.Add( "will be more difficult" );
			Text.Add( "Oh and watch out for the zombies" );
			Text.Add( "" );
			Text.Add( "P.S: The hint are not always useful ;)" );
			Text.Add( "" );
			Text.Add( "Controls" );
			Text.Add( "WASD - Move" );
			Text.Add( "Space - Attack / Interact" );
			Text.Add( "" );
			Text.Add( "" );
			Text.Add( "Press Escape to go back to the menu" );
		}

		#endregion Constructor

		#region Methods

		public override void Draw( GameTime gameTime )
		{
			base.Draw( gameTime );

			Batch.Begin();
			const int startOffset = 75;

			for( int i = 0; i < Text.Count; ++i )
			{
				string text = Text[i];

				Vector2 size = Font.MeasureString( text );
				Vector2 position = new Vector2( (int)( GraphicsDevice.BackBuffer.Width / 2 - size.X / 2 ), startOffset + i * 30 );

				Batch.DrawString( Font, text, position, Color.White );
			}
			Batch.End();
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

			LastKeyboard = Game.Services.GetService<KeyboardManager>().GetState();
		}

		#endregion Methods

		#region Properties

		#endregion Properties

		#region Attributes

		private SpriteBatch Batch;
		private SpriteFont Font;
		private KeyboardState LastKeyboard;
		private List<string> Text = new List<string>();

		#endregion Attributes
	}
}