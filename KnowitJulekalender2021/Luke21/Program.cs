using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Luke21
{
    public class Trie
    {
        public const string ALPHABET = "abcdefghijklmnopqrstuvwxyzæøå";

        public class Node
        {
            public string Word;
            public bool IsTerminal { get { return Word != null; } }
            public Node[] Edges = new Node[29];
        }

        public Node Root = new Node();

        public Trie(IEnumerable<string> words)
        {
            foreach (var word in words)
            {
                var node = Root;
                for (int len = 1; len <= word.Length; len++)
                {
                    var letter = ALPHABET.IndexOf(word[len - 1]);
                    Node next = node.Edges[letter];
                    if (next == null)
                    {
                        next = new Node();
                        if (len == word.Length)
                        {
                            next.Word = word;
                        }
                        node.Edges[letter] = next;
                    }
                    node = next;
                }
            }
        }
    }

    public class Program
    {
        string[] Words;
        Trie Dictionary;
        int[] MessageDigits;

        static void Main(string[] args)
        {
            var p = new Program();
            Console.WriteLine(p.Solve());
            Console.WriteLine(p.SolveLarge());
            BenchmarkRunner.Run<Program>();
        }

        [Benchmark]
        public string SolveLarge()
        {
            var large = File.ReadAllText("large.txt");            
            MessageDigits = large.Select(c => c - '0').ToArray();
            var (success, words) = ParseSentence();
            return success ? string.Join(" ", words.Reverse()) : "No valid solution found";
        }

        [Benchmark]
        public string SolveRandom()
        {
            var random = GenerateEncrypted();
            Console.WriteLine(random);
            MessageDigits = random.Select(c => c - '0').ToArray();
            var (success, words) = ParseSentence();
            return success ? string.Join(" ", words.Reverse()) : "No valid solution found";
        }

        public string GenerateEncrypted()
        {
            var rnd = new Random();
            int len = 0;
            StringBuilder sb = new StringBuilder();
            while (len < 8000)
            {
                var index = rnd.Next(0, Words.Length - 1);
                sb.Append(Words[index]);
                len += Words[index].Length;
            }
            return string.Join("",sb.ToString().Select(c => Trie.ALPHABET.IndexOf(c) + 1).Select(i => i.ToString()));
        }

        [Benchmark]
        public string SolveWithIO()
        {
            Words = File.ReadAllLines("wordlist.txt");
            BuildTrie();
            return Solve();
        }

        [Benchmark]
        public string Solve()
        {
            var encrypted = "45205145192051057281419115181357" + 
                            "20912102112518120151616191125209" +
                            "14751412210113519235227291821812" +
                            "22718192919149121210211251491919" + 
                            "514";
            MessageDigits = encrypted.Select(c => c - '0').ToArray();
            var (success, words) = ParseSentence();
            return success ? string.Join(" ", words.Reverse()) : "No valid solution found";
        }

        [Benchmark]
        public void BuildTrie() => Dictionary = new Trie(Words);

        [Benchmark]
        public void ReadFile() => Words = File.ReadAllLines("wordlist.txt");

        public Program()
        {
            ReadFile();
            BuildTrie();
        }

        public (bool, Stack<string>) ParseSentence()
        {
            var words = new Stack<string>();
            if (ParseSentence(Dictionary.Root, 0, words)) return (true, words);
            return (false, null);
        }

        public bool ParseSentence(Trie.Node node, int pos, Stack<string> words)
        {
            if (pos == MessageDigits.Length) return true;

            if (pos < MessageDigits.Length - 1 && (MessageDigits[pos] == 1 || MessageDigits[pos] == 2))
            {
                var letter = MessageDigits[pos] * 10 + MessageDigits[pos + 1] - 1;
                if (node.Edges[letter] != null)
                {
                    var next = node.Edges[letter];
                    if (ParseSentence(next, pos + 2, words)) return true;
                }
            }
            if (MessageDigits[pos] > 0)
            {
                var letter = MessageDigits[pos] - 1;
                if (node.Edges[letter] != null)
                {
                    var next = node.Edges[letter];
                    if (ParseSentence(next, pos + 1, words)) return true;
                }
            }

            if (node.IsTerminal)
            {
                words.Push(node.Word);
                if (ParseSentence(Dictionary.Root, pos, words)) return true;
                words.Pop();
            }

            return false;
        }
    }
}
