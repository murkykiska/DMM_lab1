using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DMM_lab1
{
    public class Element
    {
        public int Index { get; set; }
        public int Value { get; set; }
        public Element(int index, int value) 
        {
            Index = index;
            Value = value;
        }
    }
    public class SLAE
    {
        public int N { get; set; }
        public int M { get; set; }  
        public int K { get; set; }
        public int[,] A { get; set; }
        public int[] C { get; set; }
        public int[,] B { get; set; }
        public int[,] ResultB { get; set; }
        private List<Element> firstStr;
        public SLAE(string inputPath)
        {
            // Read input params
            using (StreamReader sr = new StreamReader(inputPath))
            {
                string? s;
                string[] words;

                if ((s  = sr.ReadLine()) != null)
                {
                    words = s.Split(' ');
                    N = int.Parse(words[0]);
                    M = int.Parse(words[1]);
                }

                A = new int[N, M];
                C = new int[N];

                for (int i = 0; i < N; i++)
                {
                    s = sr.ReadLine();
                    words = s.Split(" ");

                    for (int j = 0; j < M; j++)
                    {
                        A[i, j] = int.Parse(words[j]);
                    }

                    C[i] = int.Parse(words[M]);
                }  
            }

            // Fill B matrix
            B = new int[N + M, M + 1];
            ResultB = new int[N + M, M + 1];
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    B[i, j] = A[i, j];
                }
                B[i, M] = -C[i];
            }

            for (int i = N; i < N + M; i++) 
            {
                for (int j = 0; j < M; j++)
                {
                    if (i - N == j) B[i, j] = 1;
                }
            }
        }
        private void PrintB(int iter)
        {
            Console.WriteLine($"\n--------------- {iter} ---------------\n"); 
            for (int i = 0; i < M + N; i++)
            {
                for (int j = 0; j < M + 1; j++)
                {
                    Console.Write($"{B[i, j]}\t");
                }
                Console.Write("\n");
            }
        }
        private void ReplaceColumns(int from, int to)
        {
            int t;

            for (int i = 0; i < N + M; i++)
            {
                t = B[i, to];
                B[i, to] = B[i, from];
                B[i, from] = t;
            }
        }
        private int GetGcdByStein(int a, int b)
        {
            if (a == int.MinValue)
                throw new ArgumentOutOfRangeException(nameof(a), int.MinValue, "number is int.MinValue");
            if (b == int.MinValue)
                throw new ArgumentOutOfRangeException(nameof(b), int.MinValue, "number is int.MinValue");

            a = Math.Abs(a);
            b = Math.Abs(b);
            if (a == b) return a;
            if (a == 0) return b;
            if (b == 0) return a;

            if (a % 2 == 0) //четное
            {
                if (b % 2 == 0) return 2 * GetGcdByStein(a / 2, b / 2);
                return GetGcdByStein(a / 2, b);
            }
            if (b % 2 == 0) return GetGcdByStein(a, b / 2);
            if (a > b) return GetGcdByStein((a - b) / 2, b);
            return GetGcdByStein(a, (b - a) / 2);
        }
        private int GetGcdBySteinMultipleValues(int rowIndex)
        {
            int gcd = B[rowIndex, 0];

            for (int i = 1; i < M; i++)
            {
                gcd = GetGcdByStein(gcd, B[rowIndex, i]);
            }

            return gcd;
        }
        private bool FindNotNullElems(int rowNumber)
        {
            firstStr = new List<Element>();

            for (int j = 0; j < M + 1; j++)
            {
                if (B[rowNumber, j] != 0) 
                {
                    firstStr.Add(new Element(j, B[rowNumber, j]));
                }
            }

            firstStr.Sort(delegate (Element a, Element b)
            {
                if (a == null && b == null) return 0;
                else if (a == null) return -1;
                else if (b == null) return 1;
                else
                    return Math.Abs(a.Value).CompareTo(Math.Abs(b.Value));
            });

            return firstStr.Count > rowNumber + 1;
        }
        private bool CheckGcd(int rowNumber)
        {
            var gcd = GetGcdBySteinMultipleValues(rowNumber);
            return (B[rowNumber, M] % gcd == 0);
        }
        public bool ConvertToGeneralSolution()
        {
            Element ai, aj = new Element(-1, 0);
            int q, r;
            int iter = 0;
            int rowNumber = 0;
            bool flag;            

            //PrintB(iter);

            flag = FindNotNullElems(rowNumber);

            for (int i = 0; i < N + M; i++)
            {
                for (int j = 0; j < M + 1; j++)
                {
                    ResultB[i, j] = B[i, j];
                }
            }

            while (flag)
            {
                for (int i = 0; i < N + M; i++)
                {
                    for (int j = 0; j < M + 1; j++)
                    {
                        ResultB[i, j] = B[i, j];
                    }
                }
                
                // Step 1
                ai = firstStr[0];

                if (firstStr.Count <= 1)
                {
                    break;
                }

                // Step 2
                for (int j = rowNumber; j < M + 1; j++)
                {
                    if (j != ai.Index && B[rowNumber, j] != 0)
                    {
                        aj = new Element(j, B[rowNumber, j]);
                        break;
                    }
                }

                // Step 3
                q = aj.Value / ai.Value;
                r = aj.Value % ai.Value;

                // Step 4
                for (int i = 0; i < N + M; i++)
                {
                    B[i, aj.Index] -= B[i, ai.Index] * q;
                }
                iter++;
                if (rowNumber < N)
                {
                    //Console.WriteLine($"rowNumber = {rowNumber}");

                    if (!FindNotNullElems(rowNumber) && firstStr[0].Index != M + 1)
                    {
                        if (!CheckGcd(rowNumber))
                        {
                            //PrintB(iter);
                            Console.WriteLine("NO SOLUTIONS");
                            return false;
                        }                        

                        ReplaceColumns(firstStr[0].Index, 0);
                        rowNumber++;

                    }
                    FindNotNullElems(rowNumber);
                    if (!CheckGcd(rowNumber))
                    {
                        //PrintB(iter);
                        Console.WriteLine("NO SOLUTIONS");
                        return false;

                    }
                    //PrintB(iter);

                }
                else
                {
                    flag = false;
                }               
            }

            // Check last column
            if (N > 1)
            {
                for (int i = 0; i < N; i++)
                {
                    if (ResultB[i, M] != 0)
                    {
                        Console.WriteLine("NO SOLUTIONS");
                        return false;
                    }
                }
            }
            
            return true;
        }
        public void ConvertToTrapezoidalForm()
        {
            int index = 0;

            for (int i = 0; i < N + M; i++)
            {
                for (int j = 0; j < M + 1; j++)
                {
                    B[i, j] = ResultB[i, j];
                }
            }

            for (int i = 0; i < N; i++)
            {
                FindNotNullElems(i);

                index = i;

                for (int j = 0; j < firstStr.Count; j++)
                {                    
                    if (firstStr[j].Index > i)
                    {
                        ReplaceColumns(index, firstStr[j].Index);
                        index++;
                        //PrintB(K);
                    }
                }
            }
        }
        public void PrintFreeVariables(StreamWriter sw)
        {
            ConvertToTrapezoidalForm();

            K = 0;

            while (B[K, K] != 0 && K < N) K++;

            sw.WriteLine($"{M - K}");

            for (int i = N; i < N + M; i++)
            {
                for (int j = K; j < M + 1; j++)
                {
                    sw.Write($"{B[i, j]}\t");
                }
                sw.Write("\n");
            }

            Console.WriteLine($"K = {M - K}");

            for (int i = N; i < N + M; i++)
            {

                for (int j = K; j < M + 1; j++)
                {
                    Console.Write($"{B[i, j]}\t");
                }
                Console.Write("\n");
            }

        }
    }
}
