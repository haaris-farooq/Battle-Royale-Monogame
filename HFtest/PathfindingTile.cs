using ComputingProjectHF;
using HFtest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HFtest
{
    public class PathfindingTile
    {
        public Point Position { get; set; }
        public int Distance { get; set; } //distance from this tile to the target tile
        public int Cost { get; set; } //how many tiles have come before this tile
        public int CostDistance => Cost + Distance;
        public PathfindingTile Parent { get; set; } //tile that this tile was found by

        public void CalculateDistance(Point Target)
        {
            //calculate distance between from this tile to target tile
            //by adding difference in x positions and difference in y positions
            Distance = Math.Abs(Target.X - Position.X) + Math.Abs(Target.Y - Position.Y);
        }
    }
}
