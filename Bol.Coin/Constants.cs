using System.Numerics;
using Neo.SmartContract.Framework;

namespace Bol.Coin
{
    public class Constants
    {
        private static readonly BigInteger Factor = 100000000;

        public static readonly BigInteger COLLATERAL_BOL = 1000 * Factor;
        public static readonly BigInteger CERTIFIER_FEE = 1 / 1000 * Factor;

        public const byte B_ACCOUNT_TYPE = 0x01;
        public const byte C_ACCOUNT_TYPE = 0x02;

        public static readonly BigInteger B_ADDRESS_START = new BigInteger("0xB9C64900".HexToBytes());
        public static readonly BigInteger B_ADDRESS_END = new BigInteger("0x8CC74900".HexToBytes());
        public static readonly BigInteger C_ADDRESS_START = new BigInteger("0x1AF05400".HexToBytes());
        public static readonly BigInteger C_ADDRESS_END = new BigInteger("0xEDF05400".HexToBytes());

        public static readonly BigInteger DPS = 184200000;

        public const byte ACCOUNT_STATUS_OPEN = 0x01;
        public const byte ACCOUNT_STATUS_PENDING_CERTIFICATIONS = 0x02;
        public const byte ACCOUNT_STATUS_PENDING_FEES = 0x03;
        public const byte ACCOUNT_STATUS_LOCKED = 0x04;        
    }
}
