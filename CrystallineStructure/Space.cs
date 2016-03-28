using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystallineStructure
{
	class Locale
	{
		public string element;
		public float[] coord = new float[3];
		public float r;
		public sbyte charge;

		public Locale(string element, sbyte charge, float x,float y,float z)
		{
			this.element = element;
			this.charge = charge;
			coord[0] = x;
			coord[1] = y;
			coord[2] = z;
		}
	}
	class Space
	{
		public List<Locale> Atoms = new List<Locale>(26214400);
		public int length = 0;
		public Space()
		{ }


		public void CubicFill(UnitCell a, int n, float offX, float offY, float offZ)
		{
			length = n;
			for (int x = 0; x < n; x++)
			{
				for (int y = 0; y < n; y++)
				{
					for (int z = 0; z < n; z++)
					{
						Mount(ref a, x, y, z, offX, offY, offZ);
						System.Diagnostics.Debug.WriteLine(x + " " + y + " " + z);
					} 
				}				
			}
		}
		private void Mount(ref UnitCell a, int x, int y, int z, float offX, float offY, float offZ)
		{
			Atoms.Add(new Locale(a.Atoms[a.pAnion].element, a.Atoms[a.pAnion].charge, a.dimX*x + a.Atoms[a.pAnion].x * a.dimX + offX, a.dimY*y + a.Atoms[a.pAnion].y * a.dimY + offY, a.dimZ*z + a.Atoms[a.pAnion].z * a.dimZ + offZ));
		
		}

		/// <summary>
		/// Input: integer coordinates (X,Y,Z) of a locale in a simple cubic structure
		/// </summary>
		/// <param name="c">The coordinates</param>
		/// <param name="l">The length of side of cube</param>
		/// <returns>index in array</returns>
		public static int CubicCoordinateToPointer(int l, int[] c)
		{
			int p = 0;

			p += c[0] * l * l;

			p += c[1] * l;
			p += c[2];
			return p;
		}

		public  static List<int> CubicConvexHull(ref Space s)
		{
			if (Math.Pow(s.length,3) != (double)s.Atoms.Count)  //if cubic root is not an integer then structure is not a cube
			{
				return null;
			}
			int x = s.length;     //x is length of side of cube

			List<int> hull = new List<int>();
			for (int i = 0; i < x; i++)
			{
				for (int j = 0; j < x; j++)
				{
					hull.Add(CubicCoordinateToPointer(s.length,new int[] { i, 0, j }));   //Left face  
					
				}
			}
			for (int i = 1; i < x; i++)
			{
				for (int j = 0; j < x; j++)
				{
					hull.Add(CubicCoordinateToPointer(s.length, new int[] { 0, i, j }));   //Back face                
				}
			}
			for (int i = 1; i < x; i++)
			{
				for (int j = 1; j < x; j++)
				{
					hull.Add(CubicCoordinateToPointer(s.length, new int[] { i, j, 0 }));   //Bottom face                    
				}
			}
			for (int i = 1; i < x; i++)
			{
				for (int j = 1; j < x; j++)
				{
					hull.Add(CubicCoordinateToPointer(s.length, new int[] { x - 1, i, j }));   //Front face                    
				}
			}
			for (int i = 1; i < x-1; i++)
			{
				for (int j = 1; j < x-1; j++)
				{
					hull.Add(CubicCoordinateToPointer(s.length, new int[] { i, j, x - 1 }));   //Top face                    
				}
			}
			for (int i = 1; i < x-1; i++)
			{
				for (int j = 1; j < x; j++)
				{
					hull.Add(CubicCoordinateToPointer(s.length, new int[] { i, x - 1, j }));   //Right face                    
				}
			}
			return hull;
		}
	}
}
