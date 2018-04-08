using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeGenerator.Generators.Patterns
{
    public class WhiteVillage : WeightedBlocks, ITransformer
    {
        public static string Name = "whitevillage";

        public WhiteVillage()
        {
            Blocks.Add(new WeightedBlock("concrete 0", 10));
        }
        
        static WhiteVillage()
        {
           
        }

        public override string GetBlock(List<Point> allPoints, Point currentPoint)
        {
            currentPoint.BlockName = "concrete 0";
            return currentPoint.BlockName;
        }

        public void Transform(List<Point> points)
        {
            var blocks = points.Where(p => p.BlockName == Name);
            if (!blocks.Any()) return;
            var source = blocks.OrderBy(a => a.Y).ThenBy(a => a.X).ThenBy(a => a.Z).ToList();
            var bottom = points.Min(a => a.Y)-1;
            var minX = points.Min(a => a.X);
            var minZ = points.Min(a => a.Z);

            foreach (var point in source)
            {
                var left = blocks.FirstOrDefault(b => b.X == point.X - 1 && b.Y == point.Y && b.Z == point.Z);
                if (left != null && IsAir(left)) left = null;
                var right = blocks.FirstOrDefault(b => b.X == point.X + 1 && b.Y == point.Y && b.Z == point.Z);
                if (right != null && IsAir(right)) right = null;
                var front = blocks.FirstOrDefault(b => b.X == point.X && b.Y == point.Y && b.Z == point.Z + 1);
                if (front != null && IsAir(front)) front = null;
                var back = blocks.FirstOrDefault(b => b.X == point.X && b.Y == point.Y && b.Z == point.Z - 1);
                if (back != null && IsAir(back)) back = null;

                var corner = (left == null || right == null) && (front == null || back == null);
                var side = left == null || right == null || front == null || back == null;
                
                if (corner)
                {

                    if (left == null)
                    {
                        left = point.Clone();
                        left.X--;
                        left.BlockName = "log 1";
                        points.Add(left);
                    }
                    if (right == null)
                    {
                        right = point.Clone();
                        right.X++;
                        right.BlockName = "log 1";
                        points.Add(right);
                    }
                    if (front == null)
                    {
                        front = point.Clone();
                        front.Z++;
                        front.BlockName = "log 1";
                        points.Add(front);
                    }
                    if (back == null)
                    {
                        back = point.Clone();
                        back.Z--;
                        back.BlockName = "log 1";
                        points.Add(back);
                    }
                }
                else if (side)
                {
                    var log = point.Clone();
                    if (left == null)
                    {
                        log.X--;
                    }
                    else if (right == null)
                    {
                        log.X++;
                    } else if (front == null)
                    {
                        log.Z++;
                    }
                    else
                    {
                        log.Z--;
                    }

                    if ((point.Y - bottom) % 6 == 0)
                    {
                        
                        if (left == null || right == null)
                        {
                            

                            

                            log.BlockName = "log 9";
                            //north to south
                        }
                        else
                        {
                            // east to west
                           

                            log.BlockName = "log 5";
                        }

                        points.Add(log);
                    }
                    else if ((left == null || right == null) && (point.Z - minZ) % 6 == 0)
                    {
                        log.BlockName = "fence 1";
                        points.Add(log);
                    } else if ((front == null || back == null) && (point.X - minX) % 6 == 0)
                    {
                        log.BlockName = "fence 1";
                        points.Add(log);
                    }
                }
            }
        }
    }
}
