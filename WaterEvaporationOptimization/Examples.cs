using System;
using System.IO;
using System.Text;
using WaterEvaporationOptimization.Algorithm;

namespace WaterEvaporationOptimization
{
    public static class Examples
    {
        public static void SelectExample()
        {
            Example1();
        }

        public static void Example1()
        {
            //Defining cost function
            CostFunction cost = new CostFunction();

            //Initializing WEO
            WeoParameters parameters = new WeoParameters()
            {
                CostFucntion = cost,
                Nwm = 50,
                TMax = 100,
                MepMin = 0.03, //Best value from the paper
                MepMax = 0.6, //Best value from the paper
                ThetaMin = 20, //Best value from the paper which is equivalent to DepMin = 0.6
                ThetaMax = 50, //Best value from the paper which is equivalent to DepMax = 1
                Space = new double[][]
                {
                    new double[]{-100, 100}, 
                    new double[]{-100, 100} 
                }

            };
            WEO weo = new WEO(parameters);

            //Running WEO
            weo.Run();

            //Results
            double[] bestSol = weo.BestSolution;
            double bestCost = weo.BestCost;

            //Printing best solution
            StringWriter stringWr = new StringWriter();
            stringWr.Write(bestSol[0]);
            for (int i = 1; i < bestSol.Length; i++)
            {
                stringWr.Write(", {0}", bestSol[i]);
            }
            Console.WriteLine("Best solution: {0}", stringWr.ToString());
            
            //Printing best cost
            Console.WriteLine(bestCost);

            Console.ReadKey();
        }
    }
}