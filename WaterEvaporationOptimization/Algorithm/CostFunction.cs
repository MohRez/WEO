using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaterEvaporationOptimization.Algorithm
{
    public class CostFunction
    {
        public double GetCost(double[] solution)
        {
            double cost = this.exampleCostFunction(solution);
            return cost;
        }

        private double exampleCostFunction(double[] solution)
        {
            double cost = 0;
            foreach (double v in solution)
            {
                cost += Math.Pow(v, 2);
            }

            return cost;
        }
    }
}
