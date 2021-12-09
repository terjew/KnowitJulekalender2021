using System;
using System.Numerics;

namespace Utilities
{
    public class ChineseRemainderTheorem
    {
        // Returns modulo inverse of 'a' with respect to 'm' using extended Euclid Algorithm.
        // Refer below post for details:
        // https://www.geeksforgeeks.org/multiplicative-inverse-under-modulo-m/
        public static BigInteger ModuloInverse(BigInteger a, BigInteger m)
        {
            BigInteger m0 = m, t, q;
            BigInteger x0 = 0, x1 = 1;

            if (m == 1)
                return 0;

            // Apply extended Euclid Algorithm
            while (a > 1)
            {
                // q is quotient
                q = a / m;
                t = m;

                // m is remainder now, process same as euclid's algo
                m = a % m;
                a = t;
                t = x0;
                x0 = x1 - q * x0;
                x1 = t;
            }

            // Make x1 positive
            if (x1 < 0)
            {
                x1 += m0;
            }

            return x1;
        }

        // Returns the smallest number x such that:
        // x % num[0] = rem[0],
        // x % num[1] = rem[1],
        // ..................
        // x % num[k-2] = rem[k-1]
        // Assumption: Numbers in num[] are pairwise coprime (gcd for every pair is 1)
        public static BigInteger Solve(BigInteger[] num, BigInteger[] rem)
        {
            // Compute product of all numbers
            BigInteger prod = num[0];
            for (int i = 1; i < num.Length; i++)
            {
                prod *= num[i];
            }

            BigInteger result = 0;
            for (int i = 0; i < num.Length; i++)
            {
                var pp = prod / num[i];
                result += rem[i] * ModuloInverse(pp, num[i]) * pp;
            }

            return result % prod;
        }
    }
}
