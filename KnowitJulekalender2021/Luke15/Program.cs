using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Luke15
{
    class Program
    {
        public static Dictionary<char, int> charValues = new Dictionary<char, int>();
        public static char[] chars = new char[29];
        static Program()
        {
            int i = 0;
            for (char c = 'a'; c <= 'z'; c++)
            {
                chars[i++] = c;
            }
            chars[26] = 'æ';
            chars[27] = 'ø';
            chars[28] = 'å';
            for (i = 0; i < 29; i++)
            {
                charValues[chars[i]] = i + 1;
            }
        }

        public static char GetChar(int i)
        {
            i = (i - 1) % 29;
            while (i < 0) i += 29;
            return chars[i];
        }

        public static string GetString(IEnumerable<int> en)
        {
            return new string(en.Select(i => GetChar(i)).ToArray());
        }

        static void Main(string[] args)
        {
            var test = Encrypt("abc", "godjul");
            var test2 = Decrypt(test, "godjul");

            var encrypted = new[]
{
                "wawwgjlmwkafeosjoæiralop",
                "jagwfjsuokosjpzæynzxtxfnbæjkæalektfamxæø",
                "wawwgjlmwkoåeosaæeoltååøbupscpfzqehkgdhkjdoqqkuuakvwogjkpøjsbmpq",
                "vttyøyønøbjåiåzpejsimøldajjecnbplåkyrsliænhbgkvbecvdscxømrvåmagdioftvivwøkvbnyøå"
            };

            var cipher = Solve(encrypted);
            Console.WriteLine(cipher);
            var decrypted = encrypted.Select(s => Decrypt(s, cipher));
            foreach (var d in decrypted) Console.WriteLine(d);
        }

        static string Solve(IEnumerable<string> encrypted)
        {
            for (int i = 0; i <= 8; i++)
            {
                var cipher = new string('x', i);
                var decrypted = encrypted.Select(s => Decrypt(s, cipher));
                foreach (var d in decrypted) Console.WriteLine(d);
            }
            Console.WriteLine();
            Console.WriteLine();
            return "alvalv";
        }

        public static string Encrypt(string str, string cipher) => Crypt(str, cipher, EncryptBlock);
        public static string Decrypt(string str, string cipher) => Crypt(str, cipher, DecryptBlock);

        private delegate string CryptFunc(string str, int[] cipherArr, int cipherLength, int blockIndex);
        private static string Crypt(string str, string cipher, CryptFunc func)
        {

            string cipherPad = cipher.PadRight(8, 'x');
            int cipherLength = cipher.Length;
            int[] cipherArr = cipherPad.Select(c => charValues[c]).ToArray();
            int block = 0;
            StringBuilder sb = new StringBuilder();
            while (block * 8 < str.Length)
            {
                var remaining = str.Length - block * 8;
                sb.Append(func(str.Substring(block * 8, Math.Min(8, remaining)), cipherArr, cipherLength, block));
                block++;
            }
            return sb.ToString();
        }

        private static string DecryptBlock(string encrypted, int[] cipherArr, int cipherLength, int blockindex)
        {
            var encryptedArr = encrypted.Select(c => charValues[c]);
            int offset = 1 + cipherLength * (blockindex + 1);
            return GetString(encryptedArr.Select((c,i) => c - offset - i - cipherArr[i]));
        }

        private static string EncryptBlock(string plaintext, int[] cipherArr, int cipherLength, int blockindex)
        {
            var plainArr = plaintext.Select(c => charValues[c]);
            int offset = 1 + cipherLength * (blockindex + 1);
            return GetString(plainArr.Select((c, i) => c + cipherArr[i] + i + offset));
        }
    }
}
