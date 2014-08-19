// $Id$

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Multimedia;
using SharpDX.Toolkit;
using SharpDX.XAudio2;

namespace LD.Sound
{
	internal class SoundManager : GameSystem
	{
		#region Constructor

		public SoundManager( Game game )
			: base( game )
		{
			Device = ToDispose( new XAudio2( XAudio2Flags.None, ProcessorSpecifier.DefaultProcessor ) );
			MasterVoice = new MasteringVoice( Device );

			MasterVoice.SetVolume( 0.7f );

			Sources.Add( SoundType.Hurt1, LoadSound( "hurt1.wav" ) );
			Sources.Add( SoundType.Hurt2, LoadSound( "hurt2.wav" ) );
			Sources.Add( SoundType.OpenChest, LoadSound( "chest.wav" ) );
			Sources.Add( SoundType.Select, LoadSound( "select.wav" ) );
			Sources.Add( SoundType.Hit, LoadSound( "hit.wav" ) );
			Sources.Add( SoundType.Menu, LoadSound( "menu.wav" ) );
		}

		#endregion Constructor

		#region Methods

		public void PlaySound( SoundType type )
		{
#if DEBUG
			Debug.WriteLine( "PlaySound: {0}", type );
#endif
			SoundSource soure = Sources[type];

			soure.Voice.SubmitSourceBuffer( soure.Buffer, soure.PacketInfo );
			soure.Voice.Start();
		}

		private SoundSource LoadSound( string name )
		{
			Stream stream = File.OpenRead( Path.Combine( Game.Content.RootDirectory, name ) );
			return ToDispose( new SoundSource( Game, this, stream ) );
		}

		#endregion Methods

		#region Properties

		#endregion Properties

		#region Attributes

		internal XAudio2 Device;
		private MasteringVoice MasterVoice;
		private Dictionary<SoundType, SoundSource> Sources = new Dictionary<SoundType, SoundSource>();

		#endregion Attributes
	}
}