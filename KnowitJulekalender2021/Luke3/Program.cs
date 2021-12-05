using System;
using System.IO;
using System.Text;

namespace Luke3
{
    class Program
    {
        static void Main(string[] args)
        {
            var bytes = File.ReadAllBytes("input.txt");
            //bytes = Encoding.ASCII.GetBytes("JJJJJNNJJNNJJJJJ");

            int max = 0;
            int pos = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                var size = LargestNeutralFrom(bytes, i);
                if (size > max)
                {
                    max = size;
                    pos = i;
                }
            }
            Console.WriteLine($"{max}, {pos}");
        }

        static int LargestNeutralFrom(byte[] bytes, int start)
        {
            int j = 0;
            int n = 0;
            int size = 0;
            for (int i = start; i < bytes.Length; i++)
            {
                var c = bytes[i];
                if (c == 'J') j++;
                if (c == 'N') n++;
                if (j == n) size = i;
            }
            return size + 1 - start;
        }
    }
}
