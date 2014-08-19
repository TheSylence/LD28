// $Id$

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Multimedia;
using SharpDX.Toolkit;
using SharpDX.XAudio2;

namespace LD.Sound
{
	internal class SoundSource : GameSystem
	{
		#region Constructor

		public SoundSource( Game game, SoundManager sndMgr, Stream stream )
			: base( game )
		{
			using( SoundStream sndStream = new SoundStream( stream ) )
			{
				Buffer = new AudioBuffer( sndStream.ToDataStream() );
				Voice = new SourceVoice( sndMgr.Device, sndStream.Format );
				PacketInfo = sndStream.DecodedPacketsInfo;
			}
		}

		#endregion Constructor

		#region Methods

		#endregion Methods

		#region Properties

		#endregion Properties

		#region Attributes

		internal AudioBuffer Buffer;
		internal uint[] PacketInfo;
		internal SourceVoice Voice;

		#endregion Attributes
	}
}