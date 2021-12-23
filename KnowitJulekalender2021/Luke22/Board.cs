using System;
using System.Globalization;
using System.Text;

namespace Luke22
{
    public struct Board : IEquatable<Board>
    {
        uint Pieces;
        public string Debug => ToString();
        public bool IsSolved => this == Solved();

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

        public static Board Solved()
        {
            var b = new Board(0b11111111101010100101010100000000);
            return b;
        }

        private static uint GetPiece(string emoji)
        {
            switch (emoji)
            {
                case "🎅": return 0;
                case "⛄": return 1;
                case "✨": return 2;
                case "🎄": return 3;
            }
            throw new ArgumentException();
        }

        private static string GetString(uint piece)
        {
            switch (piece)
            {
                case 0: return "🎅";
                case 1: return "⛄";
                case 2: return "✨";
                case 3: return "🎄";
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
                return (int)Pieces;
            }
        }

        public static bool operator ==(Board left, Board right) => left.Equals(right);

        public static bool operator !=(Board left, Board right) => !(left == right);
    }

}
