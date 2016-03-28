using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CrystallineStructure
{
	class Program
	{
		static void Main(string[] args)
		{
			UnitCell uc = new UnitCell();
			uc.Add(new LatticePoint("Cl", -1, 0, 0, 0, 1.16F));
			uc.Add(new LatticePoint("Cl", -1, 1, 0, 0, 1.16F));
			uc.Add(new LatticePoint("Cl", -1, 0, 1, 0, 1.16F));
			uc.Add(new LatticePoint("Cl", -1, 0, 0, 1, 1.16F));
			uc.Add(new LatticePoint("Cl", -1, 1, 1, 0, 1.16F));
			uc.Add(new LatticePoint("Cl", -1, 1, 0, 1, 1.16F));
			uc.Add(new LatticePoint("Cl", -1, 0, 1, 1, 1.16F));
			uc.Add(new LatticePoint("Cl", -1, 1, 1, 1, 1.16F));
			uc.Add(new LatticePoint("Cs", 1, .5F, .5F, .5F, 1.88F));
			uc.ValidateBCCCell();
			Space s = new Space();
			s.CubicFill(uc, 4, 0, 0, 0);

			List<int> hull = Space.CubicConvexHull(ref s);
		}
	}

	class LatticePoint
	{
		public float x, y, z, r;
		public string element;
		public sbyte charge;
		/// <summary>
		/// Initializes a new instance of the <see cref="LatticePoint" /> class.
		/// </summary>
		/// <param name="e">The element name.</param>
		/// <param name="x">The x coordinate within a unit cell. values are 0 - 1.</param>
		/// <param name="y">The y coordinate within a unit cell. values are 0 - 1.</param>
		/// <param name="z">The z coordinate within a unit cell. values are 0 - 1.</param>
		/// <param name="r">The radius of the complete atom in angstroms. 1A = 100 picometers.</param>
		/// <param name="c">The charge of the ion.</param>
		public LatticePoint(string e, sbyte c, float x, float y, float z, float r)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.r = r;
			element = e;
			charge = c;
		}
	}
	class UnitCell
	{
		public List<LatticePoint> Atoms;
		public float dimX, dimY, dimZ;
		public int pCation = -1, pAnion = -1;           //pointers to cation and anion
		public UnitCell()
		{
			Atoms = new List<LatticePoint>();
			dimX = 0;
			dimY = 0;
			dimZ = 0;
		}
		public bool Add(LatticePoint a)
		{

			for (int i = 0; i < Atoms.Count; i++)   //check that the coordinate is not already occupied. Allows overlapping orbitals
			{
				if (Atoms[i].x == a.x && Atoms[i].y == a.y && Atoms [i].z == a.z)
				{
					return false;                       //return the index of the atom occupying the coordinate
				}
			}
			Atoms.Add(a);
			return true;                              //return -1 if successful			
		}
		public bool AddFCC(string e,  float r, sbyte charge)
		{
			float[,] Positions = new float[,] { { 0, 0, 0 }
											  , { 0, 0, 1 }
											  , { 0, 1, 0 }
											  , { 0, 1, 1 }
											  , { 1, 0, 0 }
											  , { 1, 0, 1 }
											  , { 1, 1, 0 }
											  , { 1, 1, 1 }
											  , { 0, .5F, .5F }
											  , { .5F, 0, .5F }
											  , { .5F, .5F, 0 }
											  , { 1, .5F, .5F }
											  , { .5F, 1, .5F }
											  , { .5F, .5F, 1 } };
			for (int i = 0; i < Positions.GetUpperBound(0); i++)
			{
                if (!Add(new LatticePoint(e, charge, Positions[i, 0], Positions[i, 1], Positions[i, 2], r)))//if location is already occupied
                {
                    return false;                    
                }
            }
			return true;
		}
		public void Resize ()
		{

			if (Atoms.Count == 1)                   //if simple cubic cell
			{
				dimX = Atoms[0].r * 2;              //set cell size to 2*radius
				dimY = Atoms[0].r * 2;
				dimZ = Atoms[0].r * 2;
				Atoms[0].x = .5F;                    //center atom in cell
				Atoms[0].y = .5F;
				Atoms[0].z = .5F;
			}
			/*if (Atoms.Count >= 8)
			{
				//double maxX=0, maxY=0, maxZ=0;
				for (int i = 0; i < Atoms.Count-1; i++)
				{
				   for (int j = i+1; j < Atoms.Count; j++)
				   {
							 if (Atoms[i].x != Atoms[j].x && Atoms[i].y == Atoms[j].y && Atoms [i].z == Atoms [j].z && Atoms[i].r + Atoms [j].r > dimX )
					   {     
							dimX= Atoms[i].r + Atoms [j].r;
					   }
					   else  if ( Atoms[i].y != Atoms[j].y && Atoms[i].x == Atoms[j].x && Atoms [i].z == Atoms [j].z && Atoms[i].r + Atoms [j].r > dimY )
					   {
							dimY = Atoms [i].r + Atoms[j].r;
					   }
					   else  if ( Atoms[i].z != Atoms[j].z && Atoms[i].x == Atoms[j].x && Atoms [i].y == Atoms [j].y && Atoms[i].r + Atoms [j].r > dimZ )
					   {
							dimZ = Atoms [i].r + Atoms[j].r;
					   }

					}
				}
			}
			else
			{}
		 return true; */


			//this loop measures distance between all pairs of atoms and outputs pairs that overlap
			/*for (int p = 0; p < Atoms.Count-1 && Atoms.Count > 0; p++)                          //start at first atom, ignoring the last
			{

				for (int i = p+1; i < Atoms.Count; i++)                                        //compare atom to each successive atom
				{
					if ( Math.Sqrt((Atoms[i].x - Atoms[p].x) * (Atoms[i].x - Atoms[p].x)        //if distance between atoms is less than the two radii
								 + (Atoms[i].y - Atoms[p].y) * (Atoms[i].y - Atoms[p].y)
								 + (Atoms[i].z - Atoms[p].z) * (Atoms[i].z - Atoms[p].z)) < Atoms[i].r + Atoms[p].r)
					{
						//if new atom overlaps another atom
						return new int[] { p,i };                   //return zero-based index of atom which overlaps 
					}
				} 
			}*/



		}
		
		public bool ValidateBCCCell()
		{
			float[,] Positions = new float[,] { { 0, 0, 0 }, { 0, 0, 1 }, { 0, 1, 0 }, { 0, 1, 1 }, { 1, 0, 0 }, { 1, 0, 1 }, { 1, 1, 0 }, { 1, 1, 1 }, { .5F, .5F, .5F } };
			int[] Found = new int[9];
			for (int i = 0; i < Atoms.Count; i++)
			{
				for (int j = 0; j < Positions.Length; j++)
				{
					if (Atoms[i].x == Positions[j, 0] && Atoms[i].y == Positions[j, 1] && Atoms[i].z == Positions[j, 2])
					{
						Found[j]++;
						break;
					}

				}
				if (Atoms[i].x == .5 && Atoms[i].y == .5 && Atoms[i].z == .5)
				{
					pAnion = i;
				}
				else
				{
					if (pCation != -1 && Atoms[pCation].r != 0 && Atoms[i].r != Atoms[pCation].r)
					{
						throw new Exception("Corner Atoms are not same radius");
					}
					pCation = i;

				}
			}
			for (int i = 0; i < Found.Length; i++)
			{
				if (Found[i] != 1)
				{
					throw new Exception("Lattice Point is missing or already exists");
				}
			}
			/*r0 and r1 set*/
			float l = (2 * Atoms[pCation].r + 2 * Atoms[pAnion].r) / (float)Math.Sqrt(3);    //length of side of cubic unit cell
			this.dimX = l;
			this.dimY = l;
			this.dimZ = l;
			return true;
		}

		public bool ValidateFCCCell()
		{
			float[,] Positions = new float[,] { { 0, 0, 0 }, { 0, 0, 1 }, { 0, 1, 0 }, { 0, 1, 1 }, { 1, 0, 0 }, { 1, 0, 1 }, { 1, 1, 0 }, { 1, 1, 1 }, { 0, .5F, .5F }, { .5F, 0, .5F }, { .5F, .5F, 0 }, { 1, .5F, .5F }, { .5F, 1, .5F }, { .5F, .5F, 1 } };
			int[] Found = new int[14];
			for (int i = 0; i < Atoms.Count; i++)
			{
				for (int j = 0; j < Positions.Length; j++)
				{
					if (Atoms[i].x == Positions[j,0] && Atoms[i].y == Positions[j,1] && Atoms[i].z == Positions[j,2])
					{
						Found[j]++;
						break;
					}
				   
				}
				if (Atoms[i].x == .5 || Atoms[i].y == .5 || Atoms[i].z == .5)
				{
					if (pAnion == -1)
					{
						pAnion = i;                        
					}
					if (Atoms[i].r != Atoms[pAnion].r || Atoms[i].element != Atoms[pAnion].element)
					{
						throw new Exception("Anions are not same element");
					}
				}
				else
				{
					if (Atoms[i].x == 0 || Atoms[i].x == 1 && Atoms[i].y == 0 || Atoms[i].y == 1 &&Atoms[i].z == 0 || Atoms[i].z == 1)
					{
						if (pCation == -1)
						{
							pCation = 1;
						}
						if (Atoms[i].r != Atoms[pCation].r || Atoms[i].element != Atoms[pCation].element)
						{
							throw new Exception("Cations are not same element");
						} 
					}
				}
			}
			for (int i = 0; i < Found.Length; i++)
			{
				if (Found[i] != 1)
				{
					throw new Exception("Lattice Point is missing or already exists");
				}
			}
			return true;
		}
	}
}
