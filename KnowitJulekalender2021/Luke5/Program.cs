using System;
using System.Collections.Generic;
using System.IO;

namespace Luke5
{
    record Elf (string Name, Elf Parent1, Elf Parent2);

    class Program
    {
           
        static void Main(string[] args)
        {
            var str = File.ReadAllText("tree.txt");
            //var str = "Aurora(Toralv(Grinch(Kari Robinalv) Alvborg) Grinch(Alva(Alve-Berit Anna) Grete(Ola Hans)))";
            int pos = 0;
            var elf = ParseElf(str, ref pos);
            var count = CountAncestorLevels(elf);
            Console.WriteLine($"{count - 1} nivåer");
        }

        public static int CountAncestorLevels(Elf elf)
        {
            if (elf == null) return 0;
            return elf.Name == "Grinch" ? 0 : 1 + Math.Max(CountAncestorLevels(elf.Parent1), CountAncestorLevels(elf.Parent2));
        }

        public static Elf ParseElf(string str, ref int pos)
        {
            int start = pos;
            while (++pos < str.Length)
            {
                var c = str[pos];
                if (c == ')' || c == ' ')
                {
                    return new Elf(str.Substring(start, pos - start), null, null);
                }
                if (c == '(')
                {
                    var name = str.Substring(start, pos - start);
                    ++pos;
                    Elf parent1 = null;
                    Elf parent2 = null;
                    parent1 = ParseElf(str, ref pos);
                    if (str[pos] == ' ')
                    {
                        pos++;
                        parent2 = ParseElf(str, ref pos);
                    }
                    pos++;
                    return new Elf(name, parent1, parent2);
                }
            }
            return null;
        }

    }
}
