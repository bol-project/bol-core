using Bol.Core.Abstractions;
using System;
using System.Linq;

namespace Bol.Core.Encoders
{
    public interface IBase16Encoder : IEncoder { }

    public class Base16Encoder : IBase16Encoder
    {
        public byte[] Decode(string input)
        {
            return ParseHex(input);
        }

        public string Encode(byte[] input)
        {
            return String.Join(String.Empty, input.Select(x => x.ToString("X2")));
        }

        //Jon Skeet code
        private static byte[] ParseHex(string hexString)
        {
            if ((hexString.Length & 1) != 0)
            {
                throw new ArgumentException("Input must have even number of characters");
            }
            int length = hexString.Length / 2;
            byte[] ret = new byte[length];
            for (int i = 0, j = 0; i < length; i++)
            {
                int high = ParseNybble(hexString[j++]);
                int low = ParseNybble(hexString[j++]);
                ret[i] = (byte)((high << 4) | low);
            }

            return ret;
        }

        private static int ParseNybble(char c)
        {
            switch (c)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return c - '0';
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                    return c - ('a' - 10);
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                    return c - ('A' - 10);
                default:
                    throw new ArgumentException("Invalid nybble: " + c);
            }
        }
    }
}
