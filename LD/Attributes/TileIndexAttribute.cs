// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD
{
	[AttributeUsage( AttributeTargets.All, Inherited = false, AllowMultiple = false )]
	internal sealed class TileIndexAttribute : Attribute
	{
		public TileIndexAttribute( int index )
		{
			Index = index;
		}

		public int Index { get; private set; }
	}
}