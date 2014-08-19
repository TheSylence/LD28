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

namespace LD.States
{
	internal class PauseState : Gamestate
	{
		#region Constructor

		public PauseState( Game game )
			: base( game )
		{
			Options.Add( "Resume" );
			Options.Add( "Exit" );
		}

		#endregion Constructor

		#region Methods

		public override void Draw( GameTime gameTime )
		{
			base.Draw( gameTime );

			Batch.Begin();

			int startOffset = 200;

			string text = "PAUSE";
			Vector2 size = Font.MeasureString( text );
			Vector2 position = new Vector2( (int)( GraphicsDevice.BackBuffer.Width / 2 - size.X / 2 ), startOffset );

			Batch.DrawString( Font, text, position, Color.White );

			startOffset = 400;

			for( int i = 0; i < Options.Count; ++i )
			{
				text = Options[i];
				if( i == SelectedIndex )
				{
					text = "> " + text + " <";
				}

				size = Font.MeasureString( text );
				position = new Vector2( (int)( GraphicsDevice.BackBuffer.Width / 2 - size.X / 2 ), startOffset + i * 30 );

				Batch.DrawString( Font, text, position, i != SelectedIndex ? Color.White : Color.LightGray );
			}

			Batch.End();
		}

		public override void Init()
		{
			base.Init();

			Gui gui = Game.Services.GetService<Gui>();
			WasGuiVisible = gui.Visible;
			gui.Visible = false;

			SelectedIndex = 0;
			LastKeyboard = Game.Services.GetService<KeyboardManager>().GetState();
		}

		public override void Popped()
		{
			base.Popped();

			Game.Services.GetService<Gui>().Visible = WasGuiVisible;
		}

		public override void Update( GameTime gameTime )
		{
			base.Update( gameTime );

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
						Game.Services.GetService<GamestateManager>().Pop();
						break;

					case 1:
						Game.Services.GetService<GamestateManager>().ChangeState( GamestateType.Menu );
						break;

					case 2:
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
		private bool WasGuiVisible;

		#endregion Attributes
	}
}