using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irreducible_Polynom
{
    public class PolynomArithmetics
    {
        static int field;

        public PolynomArithmetics(int field)
        {
            PolynomArithmetics.field = field;
        }
        ///

        /// Метод деления многочлена на многочлен
        /// 
        public static int[] Deconv(int[] dividend, int[] divisor)
        {
            int[] remainder;
            if (dividend.Length < divisor.Length)
            {
                remainder = (int[])dividend.Clone();
                return remainder;
            }
            if (dividend[dividend.Length - 1] == 0)
                throw new ArithmeticException("Старший член многочлена делимого не может быть 0");

            if (divisor[divisor.Length - 1] == 0)
                throw new ArithmeticException("Старший член многочлена делителя не может быть 0");


            var thisRemainder = (int[])dividend.Clone();
            for (var i = 0; i < dividend.Length - divisor.Length + 1; i++)
            {
                var coeff = thisRemainder[thisRemainder.Length - i - 1] / divisor[divisor.Length - 1] % field;
                for (var j = 0; j < divisor.Length; j++)
                {
                    thisRemainder[thisRemainder.Length - i - j - 1] -= coeff * divisor[divisor.Length - j - 1];
                }
            }
            remainder = thisRemainder.Reverse().SkipWhile(x => x == 0).Reverse().ToArray();
            return remainder;

        }
    }
}
