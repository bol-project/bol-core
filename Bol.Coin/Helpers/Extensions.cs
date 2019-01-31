using System;

namespace Bol.Coin
{
    public static class Extensions
    {
        public static double AsDouble(this byte[] source)
        {
            if (source == null) return 0;
            if (source.Length == 0) return 0;

            return BitConverter.ToDouble(source, 0);
        }

        public static int AsInt(this byte[] source)
        {
            if (source == null) return 0;
            if (source.Length == 0) return 0;

            return BitConverter.ToInt32(source, 0);
        }

        public static uint AsUInt(this byte[] source)
        {
            if (source == null) return 0;
            if (source.Length == 0) return 0;

            return BitConverter.ToUInt32(source, 0);
        }

        public static long AsLong(this byte[] source)
        {
            if (source == null) return 0;
            if (source.Length == 0) return 0;

            return BitConverter.ToInt64(source, 0);
        }

        public static bool AsBool(this byte[] source)
        {
            if (source == null) return false;
            if (source.Length == 0) return false;

            return BitConverter.ToBoolean(source, 0);
        }
    }
}
