// $Id$

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Input;

namespace LD.World
{
	internal class Camera : Entity
	{
		#region Constructor

		public Camera( Game game )
			: base( game )
		{
		}

		#endregion Constructor

		#region Methods

		public override void Initialize()
		{
			base.Initialize();

			ViewMatrix = Matrix.Identity;
		}

		public override void Update( GameTime gameTime )
		{
			base.Update( gameTime );

			Vector2 halfSize = new Vector2( Game.GraphicsDevice.BackBuffer.Width / 2.0f, Game.GraphicsDevice.BackBuffer.Height / 2.0f );
			Position = EntityToFollow.Position - halfSize;

			ViewMatrix =  Matrix.Translation( new Vector3( Position, 0 ) );

			ViewFrustum = new RectangleF( EntityToFollow.Position.X - halfSize.X, EntityToFollow.Position.Y - halfSize.Y, Game.GraphicsDevice.BackBuffer.Width, Game.GraphicsDevice.BackBuffer.Height );
		}

		#endregion Methods

		#region Properties

		public Entity EntityToFollow { get; set; }

		public Matrix ProjectionMatrix { get; private set; }

		public RectangleF ViewFrustum { get; private set; }

		public Matrix ViewMatrix { get; private set; }

		#endregion Properties

		#region Attributes

		#endregion Attributes
	}
}