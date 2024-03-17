using System;

namespace DMM_lab1 
{
    public class Program
    {
        static string path = "D:\\ACS\\Study\\Дискретные матмодели\\DMM_lab1\\DMM_lab1\\";
        static void Main(string[] args)
        {
            var d = new DirectoryInfo(path + "Tests\\");

            //SLAE s = new SLAE(path + "test2.txt");
            //bool hasSolution = s.ConvertToGeneralSolution();
            //if (hasSolution) s.PrintFreeVariables((path + "test1.txt").Replace(".txt", "_result.txt"));


            foreach (var file in d.GetFiles())
            {
                Console.WriteLine(file.Name);
                SLAE s = new SLAE(file.FullName);
                bool hasSolution = s.ConvertToGeneralSolution();

                using (StreamWriter sw = new StreamWriter(file.FullName.Replace("\\Tests\\", "\\Results\\")))
                {
                    if (hasSolution)
                        s.PrintFreeVariables(sw);
                    else
                        sw.WriteLine("NO SOLUTIONS");
                }
            }
        }
    }
}