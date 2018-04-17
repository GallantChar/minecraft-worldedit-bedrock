using System;
using System.Collections.Generic;
using System.Linq;

namespace ShapeGenerator.Generators.ComplexStructures
{
    public class Structure
    {
        public StructurePoint[,,] Matrix { get; private set; }

        public Structure()
        {

        }

        public Structure(int dx, int dy, int dz)
        {
            SetBounds(dx, dy, dz);
        }

        public int Dx { get; set; }
        public int Dy { get; set; }
        public int Dz { get; set; }

        public void SetBounds(int dx, int dy, int dz)
        {
            if (Matrix == null)
            {
                Matrix = new StructurePoint[dx, dy, dz];
                Dx = dx;
                Dy = dy;
                Dz = dz;
                return;
            }

            var newMatrix = new StructurePoint[dx, dy, dz];
            for (var x = 0; x < Dx; x++)
            for (var y = 0; y < Dy; y++)
            for (var z = 0; z < Dz; z++)
                if (x < dx && y < dy && z < dz)
                    newMatrix[x, y, z] = Matrix[x, y, z];

            Dx = dx;
            Dy = dy;
            Dz = dz;

            Matrix = newMatrix;
        }

        public List<StructurePoint> ToList()
        {
            var points = new List<StructurePoint>();
            for (var x = 0; x < Dx; x++)
            for (var y = 0; y < Dy; y++)
            for (var z = 0; z < Dz; z++)
                if (Matrix[x, y, z] != null)
                    points.Add(Matrix[x, y, z]);

            return points;
        }

        public List<Point> ToPoints()
        {
            return ToList().Select(RenderPoint).ToList();
        }

        public virtual Point RenderPoint(StructurePoint structurePoint)
        {
            return ((Point)structurePoint).Clone();
        }

        public void ReplaceBlocksByName(string oldName, string newName)
        {
            for (var x = 0; x < Dx; x++)
            for (var y = 0; y < Dy; y++)
            for (var z = 0; z < Dz; z++)
                if (Matrix[x, y, z] != null)
                    Matrix[x, y, z].BlockName = Matrix[x, y, z].BlockName.Replace(oldName, newName);
        }

        public StructurePoint GetPoint(int x, int y, int z)
        {
            if (x < 0 || y < 0 || z < 0 || x >= Dx || y >= Dx || z >= Dz) return null;
            return Matrix[x, y, z];
        }

        public void AddPoints(IEnumerable<StructurePoint> points)
        {
            var array = points.ToArray();
            var lx = array.Min(p => p.X);
            var ly = array.Min(p => p.Y);
            var lz = array.Min(p => p.Z);
            var hx = array.Max(p => p.X);
            var hy = array.Max(p => p.Y);
            var hz = array.Max(p => p.Z);

            if (lx < 0 || lz < 0 || ly < 0 || hx >= Dx || hy >= Dy || hz >= Dz)
            {
                var ndx = Math.Max(hx - lx + 1, Dx);
                var ndy = Math.Max(hy - ly + 1, Dy);
                var ndz = Math.Max(hz - lz + 1, Dz);
                SetBounds(ndx, ndy, ndz);
            }

            foreach (var p in array)
            {
                var np = p.Clone();
                np.X -= lx;
                np.Y -= ly;
                np.Z -= lz;
                Matrix[np.X, np.Y, np.Z] = np;
            }
        }

        public void GetXPointRange(StructurePoint currentPoint, out int lower,
            out int higher, int? dx = null)
        {
            GetPointRange(currentPoint, dx ?? Dx, null, out lower, out higher);
        }

        public void GetZPointRange(StructurePoint currentPoint, out int lower,
            out int higher, int? dz = null)
        {
            GetPointRange(currentPoint, null, dz ?? Dz, out lower, out higher);
        }

        public void GetPointRange(StructurePoint currentPoint, int? dx, int? dz, out int lower,
            out int higher)
        {
            lower = -1;
            higher = -1;

            for (var pos = 0; pos < (dx ?? dz); pos++)
            {
                if (Matrix[dz.HasValue ? currentPoint.X : pos, 0, dx.HasValue ? currentPoint.Z : pos] ==
                    null) continue;
                if (lower == -1) lower = pos;
                higher = pos;
            }
        }
    }
}