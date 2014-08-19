// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace LD
{
	internal static class Extensions
	{
		internal static T GetCustomAttribute<T>( this MemberInfo inf ) where T : Attribute
		{
			object[] attr = inf.GetCustomAttributes( typeof( T ), false );
			if( attr == null || attr.Length == 0 )
			{
				return null;
			}

			return attr[0] as T;
		}

		internal static Vector2 Rotate( this Vector2 v, float degrees, Vector2 center )
		{
			v -= center;

			double theta = MathUtil.DegreesToRadians( degrees );
			double cs = Math.Cos( theta );
			double sn = Math.Sin( theta );

			float x = (float)( v.X * cs - v.Y * sn );
			float y = (float)( v.X * sn + v.Y * cs );

			return new Vector2( x, y ) + center;
		}
	}
}