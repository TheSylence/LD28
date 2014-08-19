// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace LD.World
{
	internal class Entity : GameSystem
	{
		#region Constructor

		public Entity( Game game )
			: base( game )
		{
			Enabled = Visible = true;

			Dimension = new Vector2( Constants.TileSize, Constants.TileSize );
		}

		#endregion Constructor

		#region Methods

		public override void Initialize()
		{
			base.Initialize();

			Batch = Game.Services.GetService<SpriteBatch>();
		}

		public void Load()
		{
			LoadContent();
		}

		public override void Update( GameTime gameTime )
		{
			base.Update( gameTime );

			BoundingBox = new RectangleF( Position.X + 0.5f, Position.Y + 0.5f, Dimension.X, Dimension.Y );
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			TilesTexture = Game.Content.Load<Texture>( "Tiles.png" );
		}

		#endregion Methods

		#region Properties

		public SpriteBatch Batch { get; private set; }

		public RectangleF BoundingBox { get; set; }

		public Vector2 Position { get; set; }

		protected Texture TilesTexture { get; private set; }

		#endregion Properties

		#region Attributes

		protected Vector2 Dimension;

		#endregion Attributes
	}
}