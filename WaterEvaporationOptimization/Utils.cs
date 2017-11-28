using System;

namespace WaterEvaporationOptimization
{
    public static class Utils
    {
        private static Random Rand = new Random((int)DateTime.Now.Ticks);

        public static double RandomDouble(double start, double end)
        {
            double doubleRandom = Rand.NextDouble();
            doubleRandom = doubleRandom * (end - start) + start;
            return doubleRandom;
        }


        public static int[] GenerateRandomItegerNumbers(int number)
        {
            int[] nums = new int[number];
            for (int i = 0; i < nums.Length; i++)
            {
                nums[i] = i;
            }

            for (int i = 0; i < nums.Length; i++)
            {
                int r = Rand.Next(0, nums.Length);

                int tmp = nums[i];
                nums[i] = nums[r];
                nums[r] = tmp;
            }

            return nums;
        }
    }
}