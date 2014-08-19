// $Id$

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace LD
{
	internal class Highscore
	{
		#region Constructor

		public Highscore()
		{
			if( !File.Exists( "highscore.dat" ) )
			{
				File.OpenWrite( "highscore.dat" ).Close();
			}

			Reload();
		}

		#endregion Constructor

		#region Methods

		public void AddScore( ulong score )
		{
			Scores.Add( score );

			Save();
		}

		public IEnumerable<ulong> GetHighscores()
		{
			return Scores.OrderByDescending( i => i ).Take( Constants.HighscoreEntries );
		}

		public void Reload()
		{
			using( TextReader reader = new StreamReader( "highscore.dat" ) )
			{
				for( int i = 0; i < Constants.HighscoreEntries; ++i )
				{
					try
					{
						string str = reader.ReadLine();
						if( !string.IsNullOrWhiteSpace( str ) )
						{
							Scores.Add( Convert.ToUInt64( str, CultureInfo.InvariantCulture ) );
						}
					}
					catch
					{
					}
				}
			}
		}

		private void Save()
		{
			using( TextWriter writer = new StreamWriter( "highscore.dat" ) )
			{
				foreach( int score in GetHighscores() )
				{
					writer.WriteLine( score.ToString( CultureInfo.InvariantCulture ) );
				}
			}
		}

		#endregion Methods

		#region Properties

		#endregion Properties

		#region Attributes

		private List<ulong> Scores = new List<ulong>();

		#endregion Attributes
	}
}