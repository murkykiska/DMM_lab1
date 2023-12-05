using System;

namespace DMM_lab1 
{
    public class Program
    {
        static string path = "D:\\ACS\\Study\\Дискретные матмодели\\DMM_lab1\\DMM_lab1\\Tests\\";
        static void Main(string[] args)
        {
            SLAE s = new SLAE(path + "test1.txt");
            s.ConvertToGeneralSolution();
        }
    }
}