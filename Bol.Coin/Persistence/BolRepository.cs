using Bol.Coin.Models;
using Neo.SmartContract.Framework;
using System.Numerics;

namespace Bol.Coin.Persistence
{
    public static class BolRepository
    {
        /// <summary>
        /// Key for setting or retrieving the BoL coins in circulation.
        /// </summary>
        public const byte CirculatingSupply = 0x00;

        /// <summary>
        /// Prefix for creating the key that sets or retrieves BoL Accounts by CodeName.
        /// </summary>
        public const byte Account = 0x01;

        /// <summary>
        /// Key for setting or retrieving the Map with Whitelisted MainAddresses for registration.
        /// </summary>
        public const byte Whitelist = 0x02;

        /// <summary>
        /// Key for setting or retrieving Multi Citizenship Short Hashes.
        /// </summary>
        public const byte MultiCitizenship = 0x03;

        /// <summary>
        /// Prefix for setting or retrieving Certifier CodeNames by Country Code.
        /// </summary>
        public const byte Certifiers = 0x04;
        
        /// <summary>
        /// Key for setting or retrieving the required fee for Certifications.
        /// </summary>
        public const byte MaxCertificationFee = 0x05;
        
        /// <summary>
        /// Key for setting or retrieving the blockchain validator CodeName list.
        /// </summary>
        public const byte BlockchainValidators = 0x07;

        /// <summary>
        /// Key for setting or retrieving the total number of registered people in BoL.
        /// </summary>
        public const byte TotalRegisteredPersons = 0x08;
        
        /// <summary>
        /// Key for setting or retrieving the total number of registered companies in BoL.
        /// </summary>
        public const byte TotalRegisteredCompanies = 0x09;
        
        /// <summary>
        /// Key for setting of retrieving the value that designates if the smart contract is deployed.
        /// </summary>
        public const byte Deploy = 0xFF;

        /// <summary>
        /// Key for setting or retrieving the year to deaths per second Map.
        /// </summary>
        public const byte DpsYear = 0xB0;
        
        /// <summary>
        /// Key for setting or retrieving the year to population Map.
        /// </summary>
        public const byte PopYear = 0xB1;
        
        /// <summary>
        /// Key for setting or retrieving the year to timestamp Map.
        /// </summary>
        public const byte YearStamp = 0xB2;
        
        /// <summary>
        /// Key for setting or retrieving the value of the BoL claim interval.
        /// </summary>
        public const byte ClaimInterval= 0xB3;
        
        /// <summary>
        /// Key for setting or retrieving the claim distribution per person for a specific interval.
        /// </summary>
        public const byte TotalDistribute = 0xB4;
        
        /// <summary>
        /// Key for setting or retrieving the year to births per second Map.
        /// </summary>
        public const byte BpsYear = 0xB5;
        
        /// <summary>
        /// Key for setting or retrieving earth's population at block.
        /// </summary>
        public const byte Population = 0xB7;
        
        /// <summary>
        /// Key for setting or retrieving the total supply of BoL coins at block.
        /// </summary>
        public const byte TotalSupply = 0xB8;
        
        /// <summary>
        /// Key for setting or retrieving the number of new BoL coins at block.
        /// </summary>
        public const byte NewBol = 0xB9;

        /// <summary>
        /// Key for setting or retrieving the fee to be subtracted from transfer transactions.
        /// </summary>
        public const byte TransferFee = 0xC0;
        
        /// <summary>
        /// Key for setting or retrieving the fee to be subtracted from claim transfer transactions. 
        /// </summary>
        public const byte OperationsFee = 0xC1;
        
        /// <summary>
        /// Key for setting or retrieving the sum of fees that reside in the fee bucket
        /// waiting to be distributed to blockchain validators. 
        /// </summary>
        public const byte FeeBucket = 0xC2;

        /// <summary>
        /// Saves a BoL Account to the contract storage using the CodeName as key, replacing any existing entry.
        /// </summary>
        /// <param name="account"></param>
        public static void SaveAccount(BolAccount account)
        {
            var key = KeyHelper.GenerateKey(Account, account.CodeName);
            var bytes = account.Serialize();
            BolStorage.Put(key, bytes);
        }

        /// <summary>
        /// Retrieves a BoL Account from the contract storage using the CodeName as key.
        /// TotalBalance is calculated upon retrieval by the sum of all commercial and claim balances.
        /// </summary>
        /// <param name="codeName"></param>
        /// <returns></returns>
        public static BolAccount GetAccount(byte[] codeName)
        {
            var key = KeyHelper.GenerateKey(Account, codeName);
            var bytes = BolStorage.Get(key);
            if (bytes == null || bytes.Length == 0) return new BolAccount();

            var account = (BolAccount)bytes.Deserialize();

            account.TotalBalance = 0;
            var balances = account.CommercialAddresses.Values;
            foreach (var balance in balances)
            {
                account.TotalBalance += balance;
            }
            account.TotalBalance += account.ClaimBalance;

            return account;
        }

        /// <summary>
        /// Retrieves the number of registered BoL Accounts of type Person.
        /// </summary>
        /// <returns></returns>
        public static BigInteger GetTotalRegisteredPersons()
        {
            var key = KeyHelper.GenerateKey(TotalRegisteredPersons);
            return BolStorage.GetAsBigInteger(key);
        }

        /// <summary>
        /// Increases by one the number of registered BoL Accounts of type Person.
        /// </summary>
        public static void AddRegisteredPerson()
        {
            var key = KeyHelper.GenerateKey(TotalRegisteredPersons);
            var currentTotal = BolStorage.GetAsBigInteger(key);
            BolStorage.Put(key, currentTotal + 1);
        }

        /// <summary>
        /// Retrieves the number of registered BoL Accounts of type Company.
        /// </summary>
        /// <returns></returns>
        public static BigInteger GetTotalRegisteredCompanies()
        {
            var key = KeyHelper.GenerateKey(TotalRegisteredCompanies);
            return BolStorage.GetAsBigInteger(key);
        }

        /// <summary>
        /// Increases by one the number of registered BoL Accounts of type Company.
        /// </summary>
        public static void AddRegisteredCompany()
        {
            var key = KeyHelper.GenerateKey(TotalRegisteredCompanies);
            var currentTotal = BolStorage.GetAsBigInteger(key);
            BolStorage.Put(key, currentTotal + 1);
        }

        /// <summary>
        /// Sets the number of BoL coins in circulation.
        /// New BoL are added in circulation when a Claim transaction is executed in a new interval.
        /// </summary>
        /// <param name="amount"></param>
        public static void SetCirculatingSupply(BigInteger amount)
        {
            var key = KeyHelper.GenerateKey(CirculatingSupply);
            BolStorage.Put(key, amount);
        }
        
        /// <summary>
        /// Retrieves the number of BoL coins in circulation.
        /// New BoL are added in circulation when a Claim transaction is executed in a new interval.
        /// </summary>
        /// <returns></returns>
        public static BigInteger GetCirculatingSupply()
        {
            var key = KeyHelper.GenerateKey(CirculatingSupply);
            return BolStorage.GetAsBigInteger(key);
        }

        /// <summary>
        /// Sets the list of Blockchain Validator Codenames
        /// </summary>
        /// <param name="validators"></param>
        public static void SetBlockchainValidators(Map<byte[],int> validators)
        {
            var key = KeyHelper.GenerateKey(BlockchainValidators);
            BolStorage.Put(key, validators.Serialize());
        }
        
        /// <summary>
        /// Retrieves the number of BoL coins in circulation.
        /// New BoL are added in circulation when a Claim transaction is executed in a new interval.
        /// </summary>
        /// <returns></returns>
        public static Map<byte[],int> GetBlockchainValidators()
        {
            var key = KeyHelper.GenerateKey(BlockchainValidators);
            return BolStorage.Get(key).Deserialize() as Map<byte[],int>;
        }

        /// <summary>
        /// Sets the number of registered people at a specific block.
        /// Only exists at Interval blocks.
        /// </summary>
        /// <param name="blockHeight"></param>
        /// <param name="total"></param>
        public static void SetRegisteredAtBlock(BigInteger blockHeight, BigInteger total)
        {
            var key = KeyHelper.GenerateKey(TotalRegisteredPersons, blockHeight);
            BolStorage.Put(key, total);
        }

        /// <summary>
        /// Retrieves the total number of registered people at a specific block.
        /// Only exists at Interval blocks.
        /// </summary>
        /// <param name="blockHeight"></param>
        /// <returns></returns>
        public static BigInteger GetRegisteredAtBlock(BigInteger blockHeight)
        {
            var key = KeyHelper.GenerateKey(TotalRegisteredPersons, blockHeight);
            return BolStorage.GetAsBigInteger(key);
        }

        /// <summary>
        /// Sets the number of new Bol coins produced at a specific block.
        /// Only exists at Interval blocks.
        /// </summary>
        /// <param name="blockHeight"></param>
        /// <param name="bol"></param>
        public static void SetNewBolAtBlock(BigInteger blockHeight, BigInteger bol)
        {
            var key = KeyHelper.GenerateKey(NewBol, blockHeight);
            BolStorage.Put(key, bol);
        }

        /// <summary>
        /// Retrieves the total number of new Bol coins produced at a specific block.
        /// Only exists at Interval blocks.
        /// </summary>
        /// <param name="blockHeight"></param>
        /// <returns></returns>
        public static BigInteger GetNewBolAtBlock(BigInteger blockHeight)
        {
            var key = KeyHelper.GenerateKey(NewBol, blockHeight);
            return BolStorage.GetAsBigInteger(key);
        }

        /// <summary>
        /// Returns true if the Smart Contract's Deploy function has been executed.
        /// </summary>
        /// <returns></returns>
        public static bool IsContractDeployed()
        {
            var key = KeyHelper.GenerateKey(Deploy);
            var deployed = BolStorage.GetAsBigInteger(key);
            return deployed == 1;
        }

        /// <summary>
        /// Sets the Deploy flag to true to indicate that the Contract's Deploy function has been executed.
        /// </summary>
        public static void SetContractDeployed()
        {
            var key = KeyHelper.GenerateKey(Deploy);
            BolStorage.Put(key, 1);
        }

        /// <summary>
        /// Retrieves the required fee for BoL certifications.
        /// </summary>
        /// <returns></returns>
        public static BigInteger GetMaxCertificationFee()
        {
            var key = KeyHelper.GenerateKey(MaxCertificationFee);
            return BolStorage.GetAsBigInteger(key);
        }

        /// <summary>
        /// Sets the required fee for BoL certifications.
        /// </summary>
        /// <param name="fee"></param>
        public static void SetMaxCertificationFee(BigInteger fee)
        {
            var key = KeyHelper.GenerateKey(MaxCertificationFee);
            BolStorage.Put(key, fee);
        }

        /// <summary>
        /// Retrieves a dictionary of Certifiers by CountryCode.
        /// Each key in the dictionary is a Certifier CodeName, and each value is the required certification fee.
        /// </summary>
        /// <param name="countryCode"></param>
        /// <returns></returns>
        public static Map<byte[], BigInteger> GetCertifiers(byte[] countryCode)
        {
            var key = KeyHelper.GenerateKey(Certifiers, countryCode);
            var result = BolStorage.Get(key);
            if (result == null || result.Length == 0)
            {
                return new Map<byte[], BigInteger>();
            }
            var certifiers = (Map<byte[], BigInteger>)result.Deserialize();
            return certifiers;
        }

        /// <summary>
        /// Sets a dictionary of Certifiers by CountryCode.
        /// Each key in the dictionary is a Certifier CodeName, and each value is the required certification fee.
        /// </summary>
        /// <param name="countryCode"></param>
        /// <param name="certifiers"></param>
        public static void SetCertifiers(byte[] countryCode, Map<byte[], BigInteger> certifiers)
        {
            var key = KeyHelper.GenerateKey(Certifiers, countryCode);
            BolStorage.Put(key, certifiers.Serialize());
        }

        /// <summary>
        /// Increases the Total Certifiers counter by 1.
        /// </summary>
        public static void AddRegisteredCertifier()
        {
            var key = KeyHelper.GenerateKey(Certifiers);
            var currentTotal = BolStorage.GetAsBigInteger(key);
            BolStorage.Put(key, currentTotal + 1);
        }

        /// <summary>
        /// Decreases the Total Certifiers counter by 1.
        /// </summary>
        public static void RemoveRegisteredCertifier()
        {
            var key = KeyHelper.GenerateKey(Certifiers);
            var currentTotal = BolStorage.GetAsBigInteger(key);
            BolStorage.Put(key, currentTotal - 1);
        }

        /// <summary>
        /// Sets a dictionary that maps a year to the value of Births Per Second for that year. 
        /// </summary>
        /// <param name="bpsYear"></param>
        public static void SetBpsYear(Map<uint, BigInteger> bpsYear)
        {
            var key = KeyHelper.GenerateKey(BpsYear);
            BolStorage.Put(key, bpsYear.Serialize());
        }

        /// <summary>
        /// Retrieves a Dictionary that maps a year to the value of Births Per Second for that year.
        /// </summary>
        /// <returns></returns>
        public static Map<uint, BigInteger> GetBpsYear()
        {
            var key = KeyHelper.GenerateKey(BpsYear);
            var result = BolStorage.Get(key);
            return (Map<uint, BigInteger>)result.Deserialize();
        }

        /// <summary>
        /// Sets a dictionary that maps a year to the value of Deaths Per Second for that year. 
        /// </summary>
        /// <param name="dpsYear"></param>
        public static void SetDpsYear(Map<uint, BigInteger> dpsYear)
        {
            var key = KeyHelper.GenerateKey(DpsYear);
            BolStorage.Put(key, dpsYear.Serialize());
        }

        /// <summary>
        /// Retrieves a Dictionary that maps a year to the value of Deaths Per Second for that year.
        /// </summary>
        /// <returns></returns>
        public static Map<uint, BigInteger> GetDpsYear()
        {
            var key = KeyHelper.GenerateKey(DpsYear);
            var result = BolStorage.Get(key);
            return (Map<uint, BigInteger>)result.Deserialize();
        }

        /// <summary>
        /// Sets a dictionary that maps a year to the value of Population for that year. 
        /// </summary>
        /// <param name="popYear"></param>
        public static void SetPopYear(Map<uint, BigInteger> popYear)
        {
            var key = KeyHelper.GenerateKey(PopYear);
            BolStorage.Put(key, popYear.Serialize());
        }

        /// <summary>
        /// Retrieves a Dictionary that maps a year to the value of Population for that year.
        /// </summary>
        /// <returns></returns>
        public static Map<uint, BigInteger> GetPopYear()
        {
            var key = KeyHelper.GenerateKey(PopYear);
            var result = BolStorage.Get(key);
            return (Map<uint, BigInteger>)result.Deserialize();
        }

        /// <summary>
        /// Sets a dictionary that maps a year to the timestamp that this year starts.
        /// </summary>
        /// <param name="yearStamp"></param>
        public static void SetYearStamp(Map<uint, BigInteger> yearStamp)
        {
            var key = KeyHelper.GenerateKey(YearStamp);
            BolStorage.Put(key, yearStamp.Serialize());
        }

        /// <summary>
        /// Retrieves a dictionary that maps a year to the timestamp that this year starts.
        /// </summary>
        /// <returns></returns>
        public static Map<uint, BigInteger> GetYearStamp()
        {
            var key = KeyHelper.GenerateKey(YearStamp);
            var result = BolStorage.Get(key);
            return (Map<uint, BigInteger>)result.Deserialize();
        }

        /// <summary>
        /// Sets the number of blocks that are part of a BoL claim interval.
        /// </summary>
        /// <param name="interval"></param>
        public static void SetClaimInterval(BigInteger interval)
        {
            var key = KeyHelper.GenerateKey(ClaimInterval);
            BolStorage.Put(key, interval);
        }

        /// <summary>
        /// Retrieves the number of blocks that are part of a BoL claim interval.
        /// </summary>
        /// <returns></returns>
        public static BigInteger GetClaimInterval()
        {
            var key = KeyHelper.GenerateKey(ClaimInterval);
            return BolStorage.GetAsBigInteger(key);
        }

        /// <summary>
        /// Sets the claim distribution per person that corresponds to a specific BoL claim interval.
        /// </summary>
        /// <param name="blockHeight"></param>
        /// <param name="total"></param>
        public static void SetDistributeAtBlock(BigInteger blockHeight, BigInteger total)
        {
            var key = KeyHelper.GenerateKey(TotalDistribute, blockHeight);
            BolStorage.Put(key, total);
        }

        /// <summary>
        /// Retrieves the claim distribution per person that corresponds to a specific BoL claim interval.
        /// </summary>
        /// <param name="blockHeight"></param>
        /// <returns></returns>
        public static BigInteger GetDistributeAtBlock(BigInteger blockHeight)
        {
            var key = KeyHelper.GenerateKey(TotalDistribute, blockHeight);
            return BolStorage.GetAsBigInteger(key);
        }

        /// <summary>
        /// Sets the earth population at a specific block's timestamp.
        /// </summary>
        /// <param name="blockHeight"></param>
        /// <param name="total"></param>
        public static void SetPopulationAtBlock(BigInteger blockHeight, BigInteger total)
        {
            var key = KeyHelper.GenerateKey(Population, blockHeight);
            BolStorage.Put(key, total);
        }

        /// <summary>
        /// Retrieves the earth's population at a specific block's timestamp.
        /// </summary>
        /// <param name="blockHeight"></param>
        /// <returns></returns>
        public static BigInteger GetPopulationAtBlock(BigInteger blockHeight)
        {
            var key = KeyHelper.GenerateKey(Population, blockHeight);
            return BolStorage.GetAsBigInteger(key);
        }

        /// <summary>
        /// Sets the total number of BoL coins that can be claimed or that are already in circulation,
        /// at a specific block's timestamp.
        /// </summary>
        /// <param name="blockHeight"></param>
        /// <param name="total"></param>
        public static void SetTotalSupplyAtBlock(BigInteger blockHeight, BigInteger total)
        {
            var key = KeyHelper.GenerateKey(TotalSupply, blockHeight);
            BolStorage.Put(key, total);
        }

        /// <summary>
        /// Retrieves the total number of BoL coins that can be claimed or that are already in circulation,
        /// at a specific block's timestamp.
        /// </summary>
        /// <param name="blockHeight"></param>
        /// <returns></returns>
        public static BigInteger GetTotalSupplyAtBlock(BigInteger blockHeight)
        {
            var key = KeyHelper.GenerateKey(TotalSupply, blockHeight);
            return BolStorage.GetAsBigInteger(key);
        }

        /// <summary>
        /// Retrieves the required fee for Transfer transactions.
        /// </summary>
        /// <returns></returns>
        public static BigInteger GetTransferFee()
        {
            var key = KeyHelper.GenerateKey(TransferFee);
            return BolStorage.GetAsBigInteger(key);
        }

        /// <summary>
        /// Sets the required fee for Transfer transactions.
        /// </summary>
        /// <param name="fee"></param>
        public static void SetTransferFee(BigInteger fee)
        {
            var key = KeyHelper.GenerateKey(TransferFee);
            BolStorage.Put(key, fee);
        }

        /// <summary>
        /// Retrieves the required fee for Operational transactions.
        /// </summary>
        /// <returns></returns>
        public static BigInteger GetOperationsFee()
        {
            var key = KeyHelper.GenerateKey(OperationsFee);
            return BolStorage.GetAsBigInteger(key);
        }

        /// <summary>
        /// Sets the required fee for Operational transactions.
        /// </summary>
        /// <param name="fee"></param>
        public static void SetOperationsFee(BigInteger fee)
        {
            var key = KeyHelper.GenerateKey(OperationsFee);
            BolStorage.Put(key, fee);
        }

        /// <summary>
        /// Returns the sum of fees that reside in the Fee Bucket waiting to be distributed.
        /// </summary>
        /// <returns></returns>
        public static BigInteger GetFeeBucket()
        {
            var key = KeyHelper.GenerateKey(FeeBucket);
            return BolStorage.GetAsBigInteger(key);
        }

        /// <summary>
        /// Sets the sum of fees that reside in the Fee Bucket waiting to be distributed.
        /// </summary>
        /// <param name="fee"></param>
        public static void SetFeeBucket(BigInteger fee)
        {
            var key = KeyHelper.GenerateKey(FeeBucket);
            BolStorage.Put(key, fee);
        }

        /// <summary>
        /// Initialise an empty whitelist map
        /// </summary>
        public static void InitWhitelist()
        {
            var key = KeyHelper.GenerateKey(Whitelist);
            var whitelist = new Map<byte[], BigInteger>();
            BolStorage.Put(key, whitelist.Serialize());
        }
        
        /// <summary>
        /// Returns true if the provided address is one of the whitelisted main addresses.
        /// </summary>
        /// <param name="mainAddress"></param>
        /// <returns></returns>
        public static bool IsWhitelisted(byte[] mainAddress)
        {
            var key = KeyHelper.GenerateKey(Whitelist);
            var result = BolStorage.Get(key);
            var whitelist = (Map<byte[], BigInteger>)result.Deserialize();
            return whitelist.HasKey(mainAddress);
        }

        /// <summary>
        /// Adds a provided address to the whitelisted main addresses for registration.
        /// </summary>
        /// <param name="whitelist"></param>
        public static void AddToWhitelist(byte[] mainAddress)
        {
            var key = KeyHelper.GenerateKey(Whitelist);
            var result = BolStorage.Get(key);
            var whitelist = (Map<byte[], BigInteger>)result.Deserialize();
            whitelist[mainAddress] = 1;
            BolStorage.Put(key, whitelist.Serialize());
        }

        /// <summary>
        /// Removes a provided address from the whitelisted main addresses for registration.
        /// </summary>
        /// <param name="whitelist"></param>
        public static void RemoveFromWhitelist(byte[] mainAddress)
        {
            var key = KeyHelper.GenerateKey(Whitelist);
            var result = BolStorage.Get(key);
            var whitelist = (Map<byte[], BigInteger>)result.Deserialize();
            whitelist.Remove(mainAddress);
            BolStorage.Put(key, whitelist.Serialize());
        }

        /// <summary>
        /// Adds a provided shortHash to the MultiCitizenship List, along with the certifier and block height of the action.
        /// </summary>
        /// <param name="shortHash"></param>
        /// <param name="codeName"></param>
        /// <param name="height"></param>
        public static void AddToMultiCitizenshipList(byte[] shortHash, byte[] codeName, uint height)
        {
            var key = KeyHelper.GenerateKey(MultiCitizenship, shortHash);
            var result = BolStorage.Get(key);
            
            Map<byte[], uint> multiCitizenship;
            if (result == null || result.Length == 0)
            {
                multiCitizenship = new Map<byte[], uint>();
            }
            else
            {
                multiCitizenship = (Map<byte[], uint>)result.Deserialize();
            }
            multiCitizenship[codeName] = height;
            
            BolStorage.Put(key, multiCitizenship.Serialize());
        }

        /// <summary>
        /// Returns true if the provided input has been registered as a Multi Citizenship shortHash. 
        /// </summary>
        /// <param name="shortHash"></param>
        /// <returns></returns>
        public static bool IsMultiCitizenship(byte[] shortHash)
        {
            var key = KeyHelper.GenerateKey(MultiCitizenship, shortHash);
            
            return BolStorage.KeyExists(key);
        }
    }
}
