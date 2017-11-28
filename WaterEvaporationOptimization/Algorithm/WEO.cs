using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaterEvaporationOptimization.Algorithm
{
    //http://www.sciencedirect.com/science/article/pii/S0045794916000092
    public class WEO
    {
        private double[] bestSolution;
        private double bestCost;
        private CostFunction costFucntion;
        private int nwm;
        private int tmax;
        private double mepMin;
        private double mepMax;
        //private double depMin;
        //private double depMax;
        private double thetaMin;
        private double thetaMax;
        private double[][] space;

        public WEO(WeoParameters conf)
        {
            this.costFucntion = conf.CostFucntion;
            this.nwm = conf.Nwm;
            this.tmax = conf.TMax;
            this.mepMin = conf.MepMin;
            this.mepMax = conf.MepMax;
            //this.depMin = conf.DepMin;
            //this.depMax = conf.DepMax;
            this.thetaMin = conf.ThetaMin;
            this.thetaMax = conf.ThetaMax;
            this.space = conf.Space;
        }

        public double[] BestSolution
        {
            get { return this.bestSolution; }
        }

        public double BestCost
        {
            get { return this.bestCost; }
        }

        public void Run()
        {
            //0. Initialization
            //0.1 Generate initial population
            Molecule[] pops = this.generateInitialPop();

            //0.2 Save best solution and cost
            Molecule bestMole = this.getMoleculeWithMinCost(pops);
            this.bestSolution = bestMole.CloneMolecule().Position;
            this.bestCost = bestMole.Cost;

            int it = 0;
            do
            {
                it = it + 1;

                Molecule[] newPops = new Molecule[pops.Length];

                //1. Global Search (Monolayer Evaporation Phase)
                if (it <= this.tmax / 2)
                {
                    //1.1 Generate Esub vector using Eq. (5)
                    double[] esub = new double[pops.Length];
                    Molecule minCostMole = this.getMoleculeWithMinCost(pops);
                    Molecule maxCostMole = this.getMoleculeWithMaxCost(pops);
                    double emax = Math.Log(this.mepMax);
                    double emin = Math.Log(this.mepMin);
                    double minCost = minCostMole.Cost;
                    double maxCost = maxCostMole.Cost;
                    for (int i = 0; i < esub.Length; i++)
                    {
                        double cost = pops[i].Cost;

                        double a = (emax - emin) * (cost - minCost);
                        double b = maxCost - minCost;
                        esub[i] = a / b + emin;
                    }

                    //1.2 Generate MEP matrix using Eq. (6)
                    double[,] mep = new double[pops.Length, this.space.Length];
                    for (int i = 0; i < mep.GetLength(0); i++)
                    {
                        for (int j = 0; j < mep.GetLength(1); j++)
                        {
                            double rnd = Utils.RandomDouble(0, 1);
                            if (rnd < Math.Exp(esub[i]))
                            {
                                mep[i, j] = 1;
                            }
                            else
                            {
                                mep[i, j] = 0;
                            }
                        }
                    }

                    //1.3 Generate S matrix using Eq. (10)
                    int[] perm1 = Utils.GenerateRandomItegerNumbers(pops.Length);
                    int[] perm2 = Utils.GenerateRandomItegerNumbers(pops.Length);

                    double[,] sMatrix = new double[pops.Length, this.space.Length];

                    for (int i = 0; i < sMatrix.GetLength(0); i++)
                    {
                        double rand = Utils.RandomDouble(0, 1);
                        for (int j = 0; j < sMatrix.GetLength(1); j++)
                        {
                            sMatrix[i, j] =
                                rand *
                                (pops[perm1[i]].Position[j] -
                                 pops[perm2[i]].Position[j]);
                        }
                    }

                    //1.4 Generate evaporated molecules: WM = WM + S * MEP
                    for (int i = 0; i < pops.Length; i++)
                    {
                        double[] s_prod_mep = new double[this.space.Length];
                        for (int j = 0; j < s_prod_mep.Length; j++)
                        {
                            s_prod_mep[j] = sMatrix[i, j] * mep[i, j];
                        }

                        double[] newPosition = new double[this.space.Length];
                        for (int j = 0; j < newPosition.Length; j++)
                        {
                            newPosition[j] = pops[i].Position[j] + s_prod_mep[j];
                        }

                        double newCost = this.costFucntion.GetCost(newPosition);
                        newPops[i] = new Molecule(newPosition, newCost);
                    }
                }

                //2. Local Search (Droplet Evaporation Phase)
                else
                {
                    //2.1 Generate theta vector using Eq. (8)
                    double[] theta = new double[pops.Length];
                    Molecule minCostMole = this.getMoleculeWithMinCost(pops);
                    Molecule maxCostMole = this.getMoleculeWithMaxCost(pops);
                    double tMax = this.thetaMax;
                    double tMin = this.thetaMin;
                    double minCost = minCostMole.Cost;
                    double maxCost = maxCostMole.Cost;
                    for (int i = 0; i < theta.Length; i++)
                    {
                        double cost = pops[i].Cost;

                        double a = (tMax - tMin) * (cost - minCost);
                        double b = maxCost - minCost;
                        theta[i] = a / b + tMin;
                    }

                    //2.2 Generate DEP matrix using Eq. (9)
                    double[,] dep = new double[pops.Length, this.space.Length];
                    for (int i = 0; i < dep.GetLength(0); i++)
                    {
                        for (int j = 0; j < dep.GetLength(1); j++)
                        {
                            double rnd = Utils.RandomDouble(0, 1);
                            if (rnd < this.jEvaporationFlux(theta[i]))
                            {
                                dep[i, j] = 1;
                            }
                            else
                            {
                                dep[i, j] = 0;
                            }
                        }
                    }

                    //2.3 Generate S matrix using Eq. (10)
                    int[] perm1 = Utils.GenerateRandomItegerNumbers(pops.Length);
                    int[] perm2 = Utils.GenerateRandomItegerNumbers(pops.Length);

                    double[,] sMatrix = new double[pops.Length, this.space.Length];

                    for (int i = 0; i < sMatrix.GetLength(0); i++)
                    {
                        double rand = Utils.RandomDouble(0, 1);
                        for (int j = 0; j < sMatrix.GetLength(1); j++)
                        {
                            sMatrix[i, j] =
                                rand *
                                (pops[perm1[i]].Position[j] -
                                 pops[perm2[i]].Position[j]);
                        }
                    }

                    //2.4 Generate evaporated molecules: WM = WM + S * DEP
                    for (int i = 0; i < pops.Length; i++)
                    {
                        double[] s_prod_dep = new double[this.space.Length];
                        for (int j = 0; j < s_prod_dep.Length; j++)
                        {
                            s_prod_dep[j] = sMatrix[i, j] * dep[i, j];
                        }

                        double[] newPosition = new double[this.space.Length];
                        for (int j = 0; j < newPosition.Length; j++)
                        {
                            newPosition[j] = pops[i].Position[j] + s_prod_dep[j];
                        }

                        double newCost = this.costFucntion.GetCost(newPosition);
                        newPops[i] = new Molecule(newPosition, newCost);
                    }

                }

                //3. Comparing and updateing water molecules
                for (int i = 0; i < pops.Length; i++)
                {
                    if (newPops[i].Cost < pops[i].Cost)
                    {
                        pops[i] = newPops[i];
                    }
                }

                Molecule bestCurrentMolecule = this.getMoleculeWithMinCost(newPops);
                if (bestCurrentMolecule.Cost < this.bestCost)
                {
                    this.bestSolution = bestCurrentMolecule.CloneMolecule().Position;
                    this.bestCost = bestCurrentMolecule.Cost;
                }

            } while (it <= this.tmax); //4. Terminating condition check
        }

        private double jEvaporationFlux(double theta)
        {
            double thetaInRad = (theta / 180) * Math.PI;
            double j0p0 = 1.0 / 2.6;
            double a = Math.Pow(Math.Cos(thetaInRad), 3) / 3.0;
            double b = 2.0 / 3.0 + a - Math.Cos(thetaInRad);
            double c = Math.Pow(b, -2.0 / 3.0);
            double d = j0p0 * c * (1 - Math.Cos(thetaInRad));
            return d;
        }

        private Molecule getMoleculeWithMaxCost(Molecule[] molecules)
        {
            Molecule maxCostMolecule = molecules[0];
            for (int i = 1; i < molecules.Length; i++)
            {
                if (molecules[i].Cost > maxCostMolecule.Cost)
                {
                    maxCostMolecule = molecules[i];
                }
            }

            return maxCostMolecule;
        }

        private Molecule getMoleculeWithMinCost(Molecule[] molecules)
        {
            Molecule minCostMolecule = molecules[0];
            for (int i = 1; i < molecules.Length; i++)
            {
                if (molecules[i].Cost < minCostMolecule.Cost)
                {
                    minCostMolecule = molecules[i];
                }
            }

            return minCostMolecule;
        }

        private Molecule[] generateInitialPop()
        {
            List<Molecule> individualsList = new List<Molecule>();
            for (int i = 0; i < this.nwm; i++)
            {
                Molecule mu = this.generateOneRandomMolecule();
                individualsList.Add(mu);
            }

            return individualsList.ToArray();
        }


        private Molecule generateOneRandomMolecule()
        {
            int solSize = this.space.Length;

            double[] position = new double[solSize];
            for (int i = 0; i < solSize; i++)
            {
                double min = this.space[i][0];
                double max = this.space[i][1];
                position[i] = Utils.RandomDouble(min, max);
            }

            double cost = this.costFucntion.GetCost(position);
            Molecule ml = new Molecule(position, cost);

            return ml;
        }
    }
}
