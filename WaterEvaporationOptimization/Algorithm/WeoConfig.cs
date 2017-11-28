using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaterEvaporationOptimization.Algorithm
{
    public class WeoParameters
    {
        public CostFunction CostFucntion { get; set; }

        //The number of water molecules
        public int Nwm { get; set; }

        //Maximum number of algorithm interations
        public int TMax { get; set; }

        //Minimum value of monolayer evaporation probability
        public double MepMin { get; set; }

        //Maximum value of monolayer evaporation probability
        public double MepMax { get; set; }

        ////Minimum value of droplet evaporation probability (It is better to use thetaMin instead.)
        //public double DepMin { get; set; }

        ////Maximum value of droplet evaporation probability (It is better to use thetaMax instead.)
        //public double DepMax { get; set; }

        //Minimum value of contact angle
        public double ThetaMin { get; set; }

        //Maximum value of contact angle
        public double ThetaMax { get; set; }

        //Search space dimensions
        public double[][] Space { get; set; }
    }
}
