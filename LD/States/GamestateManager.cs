// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LD.States;
using SharpDX.Toolkit;

namespace LD
{
	internal class GamestateManager : GameSystem
	{
		#region Constructor

		public GamestateManager( Game game )
			: base( game )
		{
			Enabled = Visible = true;
			Game.Services.AddService<GamestateManager>( this );
			Game.GameSystems.Add( this );

			StateMap.Add( GamestateType.Game, new PlayState( game ) );
			StateMap.Add( GamestateType.Menu, new MenuState( game ) );
			StateMap.Add( GamestateType.Help, new HelpState( game ) );
			StateMap.Add( GamestateType.GameOver, new GameOverState( game ) );
			StateMap.Add( GamestateType.Pause, new PauseState( game ) );
			StateMap.Add( GamestateType.Highscore, new HighscoreState( game ) );

			ChangeState( GamestateType.Menu );
		}

		#endregion Constructor

		#region Methods

		public void ChangeState( GamestateType state )
		{
			CurrentGamestate = StateMap[state];
			CurrentGamestate.Init();
		}

		public override void Draw( GameTime gameTime )
		{
			base.Draw( gameTime );

			CurrentGamestate.Draw( gameTime );
		}

		public override void Initialize()
		{
			base.Initialize();

			foreach( Gamestate state in StateMap.Values )
			{
				state.Initialize();
			}
		}

		public void Pop()
		{
			CurrentGamestate.Popped();
			CurrentGamestate = PushedState;
		}

		public void PushState( GamestateType state )
		{
			PushedState = CurrentGamestate;
			ChangeState( state );
		}

		public override void Update( GameTime gameTime )
		{
			base.Update( gameTime );

			CurrentGamestate.Update( gameTime );
		}

		internal bool IsActive( GamestateType gamestateType )
		{
			return CurrentGamestate == StateMap[gamestateType];
		}

		#endregion Methods

		#region Properties

		#endregion Properties

		#region Attributes

		private Gamestate CurrentGamestate;
		private Gamestate PushedState;
		private Dictionary<GamestateType, Gamestate> StateMap = new Dictionary<GamestateType, Gamestate>();

		#endregion Attributes
	}
}