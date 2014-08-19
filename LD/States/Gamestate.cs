using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.Toolkit;

namespace LD
{
	internal abstract class Gamestate : GameSystem
	{
		#region Constructor

		public Gamestate( Game game )
			: base( game )
		{
		}

		#endregion Constructor

		#region Methods

		public virtual void Init()
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			LoadContent();
		}

		public virtual void Popped()
		{
		}

		#endregion Methods

		#region Properties

		#endregion Properties

		#region Attributes

		#endregion Attributes
	}
}