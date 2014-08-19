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
	internal class LDGame : Game
	{
		#region Constructor

		public LDGame()
		{
			GraphicsDeviceManager devMgr = new GraphicsDeviceManager( this );
			devMgr.PreferredBackBufferWidth = 800;
			devMgr.PreferredBackBufferHeight = 600;
			IsFixedTimeStep = false;

			devMgr.SetPreferredGraphicsProfile( SharpDX.Direct3D.FeatureLevel.Level_9_1 );

			Content.RootDirectory = "Content";

			this.Activated += LDGame_Activated;
			this.Deactivated += LDGame_Deactivated;
		}

		private void LDGame_Activated( object sender, EventArgs e )
		{
		}

		private void LDGame_Deactivated( object sender, EventArgs e )
		{
			if( StateManager.IsActive( GamestateType.Game ) )
			{
				StateManager.PushState( GamestateType.Pause );
			}
		}

		#endregion Constructor

		#region Methods

		protected override void Draw( GameTime gameTime )
		{
			GraphicsDevice.Clear( Color.Black );

			base.Draw( gameTime );
		}

		protected override void Initialize()
		{
			Window.Title = "One";

			Keyboard = new KeyboardManager( this );
			Services.AddService<KeyboardManager>( Keyboard );

			StateManager = new GamestateManager( this );

			Batch = new SpriteBatch( GraphicsDevice, 16000 );
			Services.AddService<SpriteBatch>( Batch );

			Gui = new Gui( this );
			GameSystems.Add( Gui );
			Services.AddService<Gui>( Gui );

			Sound = new SoundManager( this );
			GameSystems.Add( Sound );
			Services.AddService<SoundManager>( Sound );

			base.Initialize();
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			Content.Load<Texture>( "Tiles.png" );
		}

		protected override void Update( GameTime gameTime )
		{
			base.Update( gameTime );
		}

		#endregion Methods

		#region Properties

		private SpriteBatch Batch;
		private Gui Gui;
		private KeyboardManager Keyboard;
		private SoundManager Sound;
		private GamestateManager StateManager;

		#endregion Properties

		#region Attributes

		#endregion Attributes
	}
}