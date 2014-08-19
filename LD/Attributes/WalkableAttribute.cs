// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD
{
	[AttributeUsage( AttributeTargets.All, Inherited = false, AllowMultiple = false )]
	internal sealed class WalkableAttribute : Attribute
	{
		public WalkableAttribute( bool walkable )
		{
			this.Walkable = walkable;
		}

		public bool Walkable { get; set; }
	}
}