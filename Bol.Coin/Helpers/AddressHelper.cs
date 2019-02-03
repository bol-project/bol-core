using Neo.SmartContract.Framework;
using System;
using System.Numerics;

namespace Bol.Coin.Helpers
{
    public class AddressHelper
    {
        public const byte AddressVersion = 25;

        public const string Alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

        /// <summary>
        /// DO NOT USE - Not functional
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static byte[] ToScriptHash(string address)
        {
            byte[] data = Base58CheckDecode(address);
            if (data.Length != 21)
                throw new FormatException();
            if (data[0] != AddressVersion)
                throw new FormatException();
            var scriptHash = new byte[20];
            Buffer.BlockCopy(data, 1, scriptHash, 0, 20);
            return scriptHash;
        }

        public static string ToAddress(byte[] scriptHash)
        {
            byte[] data = AddressVersion
                .AsByteArray()
                .Concat(scriptHash);
            return Base58CheckEncode(data);
        }

        public static byte[] Base58CheckDecode(string input)
        {
            byte[] buffer = Decode(input);
            if (buffer.Length < 4) throw new FormatException();

            byte[] checksum = SHA256Hash(buffer.Take(buffer.Length - 4));
            byte[] originalChecksum = buffer.Range(buffer.Length - 4, 4);

            if (!ArraysHelper.ArraysEqual(originalChecksum, checksum))
            {
                throw new FormatException();
            }

            byte[] result = buffer.Range(0, buffer.Length - 4);
            return result;
        }

        public static string Base58CheckEncode(byte[] data)
        {
            byte[] checksum = SHA256Hash(data);
            byte[] buffer = data.Concat(checksum.Take(4));

            return Encode(buffer);
        }

        public static byte[] Decode(string source)
        {
            BigInteger bi = 0;
            byte[] input = source.AsByteArray();
            byte[] alphabet = Alphabet.AsByteArray();

            for (int i = input.Length - 1; i >= 0; i--)
            {
                int index = IndexOf(alphabet, input[i]);
                if (index == -1)
                    throw new Exception();
                bi += index * Pow(58, input.Length - 1 - i);
            }
            byte[] bibytes = bi.AsByteArray();
            byte[] bytes = bibytes.Reverse();
            bool stripSignByte = bytes.Length > 1 && bytes[0] == 0 && bytes[1] >= 0x80;
            int leadingZeros = 0;
            for (int i = 0; i < input.Length && input[i] == alphabet[0]; i++)
            {
                leadingZeros++;
            }

            byte[] tmp = new byte[2048];
            var length = bytes.Length - (stripSignByte ? 1 : 0) + leadingZeros;
            Buffer.BlockCopy(bytes, stripSignByte ? 1 : 0, tmp, leadingZeros, length - leadingZeros);
            return tmp.Take(length);
        }

        public static string Encode(byte[] input)
        {
            BigInteger value = new byte[1].Concat(input).Reverse().AsBigInteger();

            string result = "";

            while (value >= 58)
            {
                BigInteger mod = value % 58;
                result = Alphabet[(int)mod] + result;
                value /= 58;
            }
            result = Alphabet[(int)value] + result;
            foreach (byte b in input)
            {
                if (b == 0)
                    result = Alphabet[0] + result;
                else
                    break;
            }

            return result;
        }

        public static byte[] SHA256Hash(byte[] input)
        {
            return Neo.SmartContract.Bol.Sha256Hash(input);
        }

        public static int IndexOf(byte[] input, byte c)
        {
            int result = -1;

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == c)
                {
                    result = i;
                    break;
                }
            }

            return result;
        }

        public static BigInteger Pow(BigInteger input, int times)
        {
            BigInteger result = 1;
            for (int i = 0; i < times; i++)
            {
                result = result * result;
            }
            return result;
        }
    }
}
