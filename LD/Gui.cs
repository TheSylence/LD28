// $Id$

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace LD
{
	internal class Gui : GameSystem
	{
		#region Constructor

		public Gui( Game game )
			: base( game )
		{
			Rand = new Random();

			for( int i = 1; i <= 3; ++i )
			{
				List<string> texts = new List<string>();

				using( TextReader reader = new StreamReader( Assembly.GetExecutingAssembly().GetManifestResourceStream( string.Format( "LD.Texts{0}.txt", i ) ) ) )
				{
					StringBuilder sb = new StringBuilder();

					string line;
					while( ( line = reader.ReadLine() ) != null )
					{
						if( string.IsNullOrWhiteSpace( line ) )
						{
							texts.Add( sb.ToString() );
							sb.Clear();
						}
						else
						{
							sb.AppendLine( line );
						}
					}

					texts.Add( sb.ToString() );
				}

				RandomTexts.Add( texts );
			}
		}

		#endregion Constructor

		#region Methods

		public override void Draw( GameTime gameTime )
		{
			base.Draw( gameTime );

			const int width = 512;
			const int height = 320;

			RectangleF dest = new Rectangle( Game.GraphicsDevice.BackBuffer.Width / 2 - width / 2, Game.GraphicsDevice.BackBuffer.Height / 2 - height / 2, width, height );
			Rectangle source = new Rectangle( 480, 416, Constants.TileSize, Constants.TileSize );

			Batch.Begin( SpriteSortMode.Deferred, null, GraphicsDevice.SamplerStates.PointClamp );
			Batch.Draw( TilesTexture, dest, source, Color.White );

			int x = (int)dest.X;
			int y = (int)dest.Y + 30;
			const int LineHeight = 30;

			foreach( string line in Text )
			{
				Vector2 size = Font.MeasureString( line );

				Batch.DrawString( Font, line, new Vector2( x + width / 2 - size.X / 2, y ), Color.White );
				y += LineHeight;
			}

			Batch.End();
		}

		public override void Initialize()
		{
			base.Initialize();

			Batch = Game.Services.GetService<SpriteBatch>();
		}

		public void SetText( string text )
		{
			Text.Clear();

			foreach( string str in text.Split( new[] { Environment.NewLine }, StringSplitOptions.None ) )
			{
				Text.Add( str );
			}

			Text.Add( string.Empty );
			Text.Add( "Press Space to close" );
			Visible = true;
		}

		internal string GetRandomText( int chest )
		{
			return RandomTexts[chest][Rand.Next( RandomTexts[chest].Count )];
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			TilesTexture = Game.Content.Load<Texture>( "Tiles.png" );
			Font = Game.Content.Load<SpriteFont>( "Font" );
		}

		#endregion Methods

		#region Properties

		#endregion Properties

		#region Attributes

		private SpriteBatch Batch;
		private SpriteFont Font;
		private Random Rand;
		private List<List<string>> RandomTexts = new List<List<string>>();
		private List<string> Text = new List<string>();
		private Texture TilesTexture;

		#endregion Attributes
	}
}