using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Luke22
{
    public struct Board : IEquatable<Board>
    {
        public uint Pieces;
        public static Board Solved;
        public static Board Invalid;

        public string Debug => ToString();
        public bool IsSolved => this == Solved;

        public Board(string pieces)
        {
            Pieces = 0;
            int i = 0;
            var enumerator = StringInfo.GetTextElementEnumerator(pieces);
            while (enumerator.MoveNext())
            {
                var emoji = enumerator.GetTextElement();
                var piece = GetPiece(emoji);
                SetPiece(i++, piece);
            }
        }

        public Board(uint pieces)
        {
            Pieces = pieces;
        }

        private static ushort[] SantaLookup = new ushort[13 * 13 * 13 * 13];
        private const uint s0 = 13 * 13 * 13;
        private const uint s1 = 13 * 13;
        private const uint s2 = 13;
        private static uint[] SantaFactors = new uint[] { s0, s1, s2, 1 };

        private static ushort[] FrostyLookup = new ushort[9 * 9 * 9 * 9];
        private const uint f0 = 9 * 9 * 9;
        private const uint f1 = 9 * 9;
        private const uint f2 = 9;
        private static uint[] FrostyFactors = new uint[] { f0, f1, f2, 1 };

        private static ushort[] PolarisLookup = new ushort[5 * 5 * 5 * 5];
        private const uint p0 = 5 * 5 * 5;
        private const uint p1 = 5 * 5;
        private const uint p2 = 5;
        private static uint[] PolarisFactors = new uint[] { p0, p1, p2, 1 };

        static Board()
        {
            ushort n = 0;
            for (int i = 0; i <= 12; i++)
            for (int j = i; j <= 12; j++)
            for (int k = j; k <= 12; k++)
            for (int l = k; l <= 12; l++)
            {
                SantaLookup[i * s0 + j * s1 + k * s2 + l] = n;
                n++;
            }

            n = 0;
            for (int i = 0; i <= 8; i++)
            for (int j = i; j <= 8; j++)
            for (int k = j; k <= 8; k++)
            for (int l = k; l <= 8; l++)
            {
                FrostyLookup[i * f0 + j * f1 + k * f2 + l] = n;
                n++;
            }

            n = 0;
            for (int i = 0; i <= 4; i++)
            for (int j = i; j <= 4; j++)
            for (int k = j; k <= 4; k++)
            for (int l = k; l <= 4; l++)
            {
                PolarisLookup[i * p0 + j * p1 + k * p2 + l] = n;
                n++;
            }
            Solved = new Board("🎅🎅🎅🎅⛄⛄⛄⛄✨✨✨✨🎄🎄🎄🎄");
            Invalid = new Board("🎄🎄🎄🎄🎄🎄🎄🎄🎄🎄🎄🎄🎄🎄🎄🎄");
        }

        public int GetBoardNo()
        {
            //Idea for numbering boards uniquely from 0 to 63M:
            //First part stores which arrangement the 4 santas have:
            //16 over 4 :1820 possibilities -> 11 bit
            //Second part stores which arrangement the 4 snowmen have:
            //12 over 4: 495 possibilities -> 9 bit
            //Last part stores which arrangement the 4 stars/trees have:
            //8 over 4: 70 possibilities -> 7 bit

            //santas:
            //[SSSSxxxxxxxxxxxx] = 0
            //[SSSxSxxxxxxxxxxx] = 1
            //[SSSxxxxxxxxxxxxS] = 12
            //[SSxSSxxxxxxxxxxx] = 13
            //[SSxSxSxxxxxxxxxx] = 14
            //[SSxSxxxxxxxxxxxS] = 24
            //[SSxxSSxxxxxxxxxx] = 25
            //...
            //[xxxxxxxxxxxxSSSS] = 1819

            //frosty: (only considering remaining spaces)
            //[FFFFxxxxxxxx] = 0
            //[FFFxFxxxxxxx] = 1
            //[xxxxxxxxFFFF] = 494

            //polaris: (only considering remaining spaces)
            //[PPPPxxxx] = 0
            //[PPPxPxxx] = 1
            //[xxxxPPPP] = 69 (nice!)

            Span<int> santaOffsets = stackalloc int[4];
            int s = 0;
            Span<int> frostyOffsets = stackalloc int[4];
            int f = 0;
            Span<int> polarisOffsets = stackalloc int[4];
            int p = 0;

            uint santa = 3;
            uint frosty = 2;
            uint polaris = 1;
            for (int i = 0; i < 16; i++)
            {
                var masked = (Pieces & santa);
                if (masked == santa)
                {
                    santaOffsets[s] = i - s;
                    s++;
                }
                else if (masked == frosty)
                {
                    frostyOffsets[f] = i - (s + f);
                    f++;
                }
                else if (masked == polaris)
                {
                    polarisOffsets[p] = i - (s + f + p);
                    p++;
                }
                santa <<= 2;
                frosty <<= 2;
                polaris <<= 2;
            }

            var santaNumber = SantaLookup[santaOffsets[0] * s0 + santaOffsets[1] * s1 + santaOffsets[2] * s2 + santaOffsets[3]];
            var frostyNumber = FrostyLookup[frostyOffsets[0] * f0 + frostyOffsets[1] * f1 + frostyOffsets[2] * f2 + frostyOffsets[3]];
            var polarisNumber = PolarisLookup[polarisOffsets[0] * p0 + polarisOffsets[1] * p1 + polarisOffsets[2] * p2 + polarisOffsets[3]];

            return santaNumber * 499 * 71 + frostyNumber * 71 + polarisNumber;
        }

        public static Board FromBoardNumbers(int santaNumber, int frostyNumber, int polarisNumber)
        {
            var santaIndexSigned = Array.IndexOf(SantaLookup, (ushort)santaNumber);
            if (santaIndexSigned < 0) return Board.Invalid;

            var frostyIndexSigned = Array.IndexOf(FrostyLookup, (ushort)frostyNumber);
            if (frostyIndexSigned < 0) return Board.Invalid;

            var polarisIndexSigned = Array.IndexOf(PolarisLookup, (ushort)polarisNumber);
            if (polarisIndexSigned < 0) return Board.Invalid;

            var polarisOffsets = new uint[4];
            var santaOffsets = new uint[4];
            var frostyOffsets = new uint[4];
            var santaIndex = (uint)santaIndexSigned;
            var frostyIndex = (uint)frostyIndexSigned;
            var polarisIndex = (uint)polarisIndexSigned;

            uint q = 0;
            for (uint i = 0; i < 4; i++)
            {
                q = santaIndex / SantaFactors[i];
                santaOffsets[i] = q + i;
                santaIndex -= q * SantaFactors[i];

                q = frostyIndex / FrostyFactors[i];
                frostyOffsets[i] = q + i;
                frostyIndex -= q * FrostyFactors[i];

                q = polarisIndex / PolarisFactors[i];
                polarisOffsets[i] = q + i;
                polarisIndex -= q * PolarisFactors[i];
            }

            uint pieces = 0;
            int s = 0, f = 0, p = 0;
            for (int i = 0; i < 16; i++)
            {
                if (s < 4 && i == santaOffsets[s])
                {
                    pieces += (uint)3 << i * 2;
                    s++;
                }
                else if (f < 4 && i == frostyOffsets[f] + s)
                {
                    pieces += (uint)2 << i * 2;
                    f++;
                }
                else if (p < 4 && i == polarisOffsets[p] + s + f)
                {
                    pieces += (uint)1 << i * 2;
                    p++;
                }
            }
            return new Board(pieces);
        }

        private static uint GetPiece(string emoji)
        {
            switch (emoji)
            {
                case "🎅": return 3;
                case "⛄": return 2;
                case "✨": return 1;
                case "🎄": return 0;
            }
            throw new ArgumentException();
        }

        private static string GetString(uint piece)
        {
            switch (piece)
            {
                case 3: return "🎅";
                case 2: return "⛄";
                case 1: return "✨";
                case 0: return "🎄";
            }
            throw new ArgumentException();
        }
        public uint GetPiece(int i)
        {
            int shift = i * 2;
            uint mask = 0b11;
            mask = (mask << shift);
            var masked = (Pieces & mask);
            return masked >> shift;
        }

        public void SetPiece(int i, uint piece)
        {
            var shift = i * 2;
            uint mask = 0b11;
            mask = (mask << shift);
            Pieces &= ~mask;
            Pieces |= (piece << shift);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 16; i++)
            {
                sb.Append(GetString(GetPiece(i)));
            }
            return sb.ToString();
        }

        public void Move(bool row, int no, bool forward)
        {
            int start, end, step;
            if (row)
            {
                start = no * 4 + (forward ? 0 : 3);
                end = no * 4 + (forward ? 3 : 0);
                step = forward ? 1 : -1;
            }
            else
            {
                start = (forward ? 0 : 3) * 4 + no;
                end = (forward ? 3 : 0) * 4 + no;
                step = forward ? 4 : -4;
            }
            var tmp = GetPiece(end);
            for (int i = 0; i < 3; i++)
            {
                int j = i + 1;
                SetPiece(end - step * i, GetPiece(end - step * j));
            }
            SetPiece(start, tmp);
        }

        public void Move(byte move)
        {
            //0bRFNN where R = row/column, F = forward/backward, NN = piece number
            int no = move & 0b0011;
            bool row = (move & 0b1000) != 0;
            bool forward = (move & 0b0100) != 0;
            Move(row, no, forward);
        }

        public Board WithMove(byte move)
        {
            var other = new Board(Pieces);
            other.Move(move);
            return other;
        }

        public static byte InverseMove(byte move)
        {
            bool forward = (move & 0b0100) != 0;
            if (forward) return (byte)(move & 0b1011);
            else return (byte)(move | 0b0100);
        }

        public override bool Equals(object obj) => obj is Board other && this.Equals(other);

        public bool Equals(Board other) => other.Pieces == Pieces;

        public override int GetHashCode()
        {
            unchecked
            {
                return (int)(Pieces);
            }
        }

        public static bool operator ==(Board left, Board right) => left.Equals(right);

        public static bool operator !=(Board left, Board right) => !(left == right);
    }

}
