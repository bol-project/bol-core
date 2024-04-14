using System.Numerics;
using Neo.SmartContract.Framework;

namespace Bol.Coin
{
    public static class Constants
    {
        /// <summary>
        /// Multiply integer sums with this factor to add a fixed number of decimal digits.
        /// </summary>
        public static readonly BigInteger Factor = 100000000;

        /// <summary>
        /// Collateral in Bols that a Person or Company needs to reserve in order to become a BoL Certifier.
        /// </summary>
        public static readonly BigInteger CertifierCollateral = 10000000000;
        
        /// <summary>
        /// A fee in Bols that a Person or Company needs to pay in order to be certified in the BoL Blockchain.
        /// </summary>
        public static readonly BigInteger MaxCertificationFee = new BigInteger("0x404B4C".HexToBytes()); //5000000

        /// <summary>
        /// The number of certifications required for an account to become a Bol Certifier.
        /// </summary>
        public static readonly BigInteger CertifierRequiredCertifications = 4;

        /// <summary>
        /// A flag that represents a Person type Account.
        /// </summary>
        public const byte AccountTypeB = 0x01;
        
        /// <summary>
        /// A flag that represents a Company type Account.
        /// </summary>
        public const byte AccountTypeC = 0x02;

        public const byte TransactionTypeClaim = 0x01;
        public const byte TransactionTypeClaimTransfer = 0x02;
        public const byte TransactionTypeTransfer = 0x03;
        public const byte TransactionTypeFees = 0x04;
        public const byte TransactionTypeRegister = 0x05;
        public const byte TransactionTypeWhitelist = 0x06;
        public const byte TransactionTypeCertifierSelection = 0x07;
        public const byte TransactionTypeCertificationRequest = 0x08;
        public const byte TransactionTypeCertify = 0x09;
        public const byte TransactionTypeRegisterCertifier = 0x0B;
        public const byte TransactionTypeUnRegisterCertifier = 0x0C;
        public const byte TransactionTypeAddMultiCitizenship = 0x0D;
        public const byte TransactionTypeSetCertifierFee = 0x0E;
        public const byte TransactionTypeAddCommercialAddress = 0x0F;

        public const int TransactionCountLimit = 10;

        /// <summary>
        /// Start of the range of numbers a Person Address must reside in. Addresses in this range start with BBB.
        /// </summary>
        public static readonly BigInteger BAddressStart = new BigInteger("0xBF4900".HexToBytes());
        
        /// <summary>
        /// End of the range of numbers a Person Address must reside in. Addresses in this range start with BBB.
        /// </summary>
        public static readonly BigInteger BAddressEnd = new BigInteger("0xED4900".HexToBytes());
        
        /// <summary>
        /// Start of the range of numbers a Company Address must reside in. Addresses in this range start with BCC.
        /// </summary>
        public static readonly BigInteger CAddressStart = new BigInteger("0xE75400".HexToBytes());
        
        /// <summary>
        /// End of the range of numbers a Company Address must reside in. Addresses in this range start with BCC.
        /// </summary>
        public static readonly BigInteger CAddressEnd = new BigInteger("0x165500".HexToBytes());

        /// <summary>
        /// A flag that represents an Account that has been certified and enabled for operation.
        /// </summary>
        public const byte AccountStatusOpen = 0x01;
        
        /// <summary>
        /// A flag that represents an Account that needs to be certified in order to be enabled for operation.
        /// </summary>
        public const byte AccountStatusPendingCertifications = 0x02;
        
        /// <summary>
        /// A flag that represents an Account that has been certified but needs to pay certification fees
        /// to be enabled for operation.
        /// </summary>
        public const byte AccountStatusPendingFees = 0x03;
        
        /// <summary>
        /// A flag that represents an Account that has been locked and disabled for operation.
        /// </summary>
        public const byte AccountStatusLocked = 0x04;

        /// <summary>
        /// A code that represents the ability to perform BoL Certifications on all available countries.
        /// </summary>
        public static readonly byte[] AllCountriesCode = "414C4C".HexToBytes();

        /// <summary>
        /// Number of blocks that designates a BoL claim interval.
        /// All registrations that happen inside such an interval are considered equal in claim rights.  
        /// </summary>
        public const uint ClaimInterval = 5000; //  1 Bol Day

        /// <summary>
        /// Name of the Smart Contract.
        /// </summary>
        public static readonly string Name = "Bonus of Life";
        
        /// <summary>
        /// Symbol of the Smart Contract.
        /// </summary>
        public static readonly string Symbol = "BoL";
        
        /// <summary>
        /// Number of fixed point decimal digits supported.
        /// </summary>
        public static readonly byte Decimals = 8;
        
        /// <summary>
        /// Owner of the Smart Contract.
        /// This is the Multisign address that is derived from the Blockchain addresses of the founding members of Bol blockchain.
        /// </summary>
        public static readonly byte[] Owner = "BArFMTf79Mm4ZvTTWEgF4A3mMWENvbFWhK".ToScriptHash();

        /// <summary>
        /// The required fee for Transfer transactions.
        /// </summary>
        public static readonly BigInteger TransferFee = new BigInteger("0xE803".HexToBytes()); //1000

        /// <summary>
        /// The required fee for operational transactions except Transfer.
        /// </summary>
        public static readonly BigInteger OperationsFee = new BigInteger("0xF401".HexToBytes()); //500

        /// <summary>
        /// The max number of commercial addresses that can be added to a Bol Account.
        /// </summary>
        public static readonly int MaxCommercialAddresses = 50;

        /// <summary>
        /// Earth population at the time of the Genesis block.
        /// </summary>
        public static readonly BigInteger PopulationAtGenesis = 810408072900000000; // World Population at genesis block

        /// <summary>
        /// Births per Second by year.
        /// The following  data was obtained from : United Nations, Department of Economic and Social Affairs, Population Division (2022).
        /// World Population Prospects 2022, Online Edition. File GEN/01/REV1
        /// All data correspond to July 1 of each year
        /// </summary>
        /// <returns></returns>
        public static Map<uint, BigInteger> BpsPerYear()
        {
            var bpsYear = new Map<uint, BigInteger>();

            bpsYear[2022] = 424881402;
            bpsYear[2023] = 425797856;
            bpsYear[2024] = 425282581;
            bpsYear[2025] = 427169356;
            bpsYear[2026] = 427787475;
            bpsYear[2027] = 428711254;
            bpsYear[2028] = 428353344;
            bpsYear[2029] = 430450574;
            bpsYear[2030] = 431281573;
            bpsYear[2031] = 432057271;
            bpsYear[2032] = 432126853;
            bpsYear[2033] = 434142174;

            return bpsYear;
        }

        /// <summary>
        /// Deaths per Second by year.
        /// The following  data was obtained from : United Nations, Department of Economic and Social Affairs, Population Division (2022).
        /// World Population Prospects 2022, Online Edition. File GEN/01/REV1
        /// All data correspond to July 1 of each year
        /// </summary>
        /// <returns></returns>
        public static Map<uint, BigInteger> DpsPerYear()
        {
            var dpsYear = new Map<uint, BigInteger>();

            dpsYear[2022] = 212761818;
            dpsYear[2023] = 192669172;
            dpsYear[2024] = 192755468;
            dpsYear[2025] = 196400336;
            dpsYear[2026] = 199546369;
            dpsYear[2027] = 202780876;
            dpsYear[2028] = 205555783;
            dpsYear[2029] = 209533711;
            dpsYear[2030] = 213002762;
            dpsYear[2031] = 216597726;
            dpsYear[2032] = 219707426;
            dpsYear[2033] = 224090401;

            return dpsYear;
        }

        /// <summary>
        /// Population by year.
        /// The following  data was obtained from : United Nations, Department of Economic and Social Affairs, Population Division (2022).
        /// World Population Prospects 2022, Online Edition. File GEN/01/REV1
        /// All data correspond to July 1 of each year
        /// </summary>
        /// <returns></returns>
        public static Map<uint, BigInteger> PopulationPerYear()
        {
            var popYear = new Map<uint, BigInteger>();

            popYear[2022] = 797510515600000000;
            popYear[2023] = 804531144700000000;
            popYear[2024] = 811883599900000000;
            popYear[2025] = 819198845300000000;
            popYear[2026] = 826436450900000000;
            popYear[2027] = 833597767100000000;
            popYear[2028] = 840682879200000000;
            popYear[2029] = 847688939100000000;
            popYear[2030] = 854614132700000000;
            popYear[2031] = 861453274500000000;
            popYear[2032] = 868209198400000000;
            popYear[2033] = 874879854200000000;

            return popYear;
        }

        /// <summary>
        /// Timestamp by year, corresponds to July 1 (00:00:00 GMT+0000) of each year.
        /// The following  data was obtained from : United Nations, Department of Economic and Social Affairs, Population Division (2022).
        /// World Population Prospects 2022, Online Edition. File GEN/01/REV1
        /// All data correspond to July 1 of each year
        /// </summary>
        /// <returns></returns>
        public static Map<uint, BigInteger> TimestampPerYear()
        {
            var yearStamp = new Map<uint, BigInteger>();

            yearStamp[2022] = 1656633600;
            yearStamp[2023] = 1688169600;
            yearStamp[2024] = 1719792000;
            yearStamp[2025] = 1751328000;
            yearStamp[2026] = 1782864000;
            yearStamp[2027] = 1814400000;
            yearStamp[2028] = 1846022400;
            yearStamp[2029] = 1877558400;
            yearStamp[2030] = 1909094400;
            yearStamp[2031] = 1940630400;
            yearStamp[2032] = 1972252800;
            yearStamp[2033] = 2003788800;

            return yearStamp;
        }
    }
}
