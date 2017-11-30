using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Irreducible_Polynom
{
    public class FinderPolynom
    {
        public int Count { get; set; }
        public List<HashSet<int[]>> Polynomials { get; set; }
        public int N { get; set; }
        public int field;
        public int DegreeCalculated { get; set; }
        public PolynomArithmetics pa;
        public FinderPolynom()
        {
            Console.Write("Input field:");
            int field = int.Parse(Console.ReadLine());
            Console.Write("Degree:");
            N = int.Parse(Console.ReadLine());
            if (Directory.Exists(@"Polynomials"))
            {
                Directory.Delete(@"Polynomials", true);
            }

            ///Записываем многочлены первой степени
            ///
            String firstDegree = "";
            for (int i = 0; i < field; ++i)
            {
                firstDegree += i + "1" + Environment.NewLine;
            }
            Directory.CreateDirectory(@"Polynomials");
            File.WriteAllText(@"Polynomials/1.txt", firstDegree);
            Polynomials = new List<HashSet<int[]>>();
            this.field = field;
            pa = new PolynomArithmetics(field);
        }

        public void DoSomething()
        {
            var timer = new Stopwatch();
            timer.Start();
            Polynomials.Add(new HashSet<int[]>());
            for (var i = 1; i < N; i++)
                if (File.Exists(@"Polynomials\" + i + ".txt"))
                {
                    var polynomials = File.ReadAllLines(@"Polynomials\" + i + ".txt");
                    Polynomials.Add(new HashSet<int[]>());
                    foreach (var polynomial in polynomials)
                    {
                        Polynomials[i].Add(polynomial.Select(x => int.Parse(x.ToString())).ToArray());
                    }
                }
                else
                {
                    DegreeCalculated = i;
                    break;
                }
            if (DegreeCalculated == 0) return;

            for (var i = DegreeCalculated > 0 ? DegreeCalculated : 1; i <= N; i++)
            {
                var polynomial = new int[i + 1];

                ///У неприводимого многочлена младший коэффициент
                ///не может быть 0.
                for (int k = 1; k < field; ++k)
                {
                    polynomial[0] = k;
                    polynomial[polynomial.Length - 1] = 1;
                    Polynomials.Add(new HashSet<int[]>());
                    FindPolinomials(polynomial, 1);
                }
                var str = new StringBuilder();
                foreach (var irreducible in Polynomials[polynomial.Length - 1])
                    str.Append(string.Join("", irreducible) + Environment.NewLine);
                File.WriteAllText(@"Polynomials\" + i + ".txt", str.ToString());
                Console.WriteLine("Degree " + i + " calculated at " + DateTime.Now + " in "
                    + timer.Elapsed + ". " + Polynomials[i].Count + " irreducible polynomials was found");
                timer.Restart();
            }
        }

        private void CheckIrreducibility(int[] polynomial)
        {
            for (var i = 1; i <= (polynomial.Length - 1) / 2; i++)
            {
                foreach (var irreducible in Polynomials[i])
                {
                    int[] remainder = PolynomArithmetics.Deconv(polynomial, irreducible);
                    if (remainder.Length == 0) return;
                }
            }
            lock (Polynomials)
                Polynomials[polynomial.Length - 1].Add(polynomial);
        }


        public void FindPolinomials(int[] polynomial, int position)
        {

            Count++;
            if (position == polynomial.Length - 1)
            {
                /// Если сумма коэффициентов кратна порядку поля,
                /// то 1 является корнем этого многочлена
                int coeff = 0;
                for (int i = 0; i < polynomial.Length; ++i)
                {
                    coeff += polynomial[i];
                }
                if (coeff % field == 0) return;


                var a = new Thread(() => CheckIrreducibility((int[])polynomial.Clone())) { IsBackground = false };
                a.Start();
                a.Join();
                return;
            }

            for (var i = 0; i < field; i++)
            {
                polynomial[position] = i;
                FindPolinomials(polynomial, position + 1);
            }

        }
    }
}
