using System;

namespace DMM_lab1 
{
    public class Program
    {
        static void Main(string[] args)
        {
            SLAE s = new SLAE();
            bool hasSolution = s.ConvertToGeneralSolution();

            if (hasSolution)
                s.PrintFreeVariables();
            else
            {
                Console.WriteLine("NO SOLUTIONS");
            }
        }
    }
}