using System.Numerics;
using Neo.SmartContract.Framework;

namespace Bol.Coin
{
    public class Constants
    {
        public static readonly BigInteger FACTOR = 100000000;

        public static readonly BigInteger COLLATERAL_BOL = 1000 * FACTOR;
        public static readonly BigInteger CERTIFIER_FEE = FACTOR / 1000;

        public const byte B_ACCOUNT_TYPE = 0x01;
        public const byte C_ACCOUNT_TYPE = 0x02;

        public static readonly BigInteger B_ADDRESS_START = new BigInteger("0xB9C64900".HexToBytes());
        public static readonly BigInteger B_ADDRESS_END = new BigInteger("0x8CC74900".HexToBytes());
        public static readonly BigInteger C_ADDRESS_START = new BigInteger("0x1AF05400".HexToBytes());
        public static readonly BigInteger C_ADDRESS_END = new BigInteger("0xEDF05400".HexToBytes());


        public static readonly BigInteger[] DpsYear = { 184661436, 187819619, 190637490, 193569717, 196034672, 199614745, 202682166, 205775244, 208348971, 212137640, 215434148, 218821347 };
        public static readonly BigInteger[] PopYear = { 7713468205, 7794798729, 7874965732, 7953952577, 8031800338, 8108605255, 8184437453, 8259276651, 8333078318, 8405863301, 8477660723, 8548487371 };
        public static readonly BigInteger[] yearStampSeconds = { 1561939200, 1593561600, 1625097600, 1656633600, 1688169600, 1719792000, 1751328000, 1782864000, 1814400000, 1846022400, 1877558400, 1909094400 };
        public static readonly BigInteger[] yearStampMiliSeconds = { 1561939200000, 1593561600000, 1625097600000, 1656633600000, 1688169600000, 1719792000000, 1751328000000, 1782864000000, 1814400000000, 1846022400000, 1877558400000, 1909094400000 };
        public static readonly BigInteger SecOfYear = 31536000;
        public static readonly BigInteger SecOfLeapYear = 31622400;

        public const byte ACCOUNT_STATUS_OPEN = 0x01;
        public const byte ACCOUNT_STATUS_PENDING_CERTIFICATIONS = 0x02;
        public const byte ACCOUNT_STATUS_PENDING_FEES = 0x03;
        public const byte ACCOUNT_STATUS_LOCKED = 0x04;

        public static readonly byte[] ALL_COUNTRIES = "414C4C".HexToBytes();
    }
}
