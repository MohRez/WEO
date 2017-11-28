using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaterEvaporationOptimization.Algorithm
{
    public class Molecule
    {
        private double[] position;
        private double cost;

        public Molecule(double[] position, double cost)
        {
            this.position = position;
            this.cost = cost;
        }

        public double[] Position
        {
            get { return position; }
        }

        public double Cost
        {
            get { return cost; }
        }

        public Molecule CloneMolecule()
        {
            Molecule newClone = new Molecule((double[])this.position.Clone(), this.cost);
            return newClone;
        }
    }
}
