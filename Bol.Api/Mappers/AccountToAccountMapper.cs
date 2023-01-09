using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using Bol.Address;
using Bol.Core.Model;
using Bol.Cryptography;

namespace Bol.Api.Mappers
{
    public class AccountToAccountMapper : IAccountToAccountMapper
    {
        private readonly IBase16Encoder _hex;
        private readonly IScriptHashFactory _hashFactory;
        private readonly IAddressTransformer _addressTransformer;

        public AccountToAccountMapper(IBase16Encoder hex, IScriptHashFactory hashFactory, IAddressTransformer addressTransformer)
        {
            _hex = hex ?? throw new ArgumentNullException(nameof(hex));
            _hashFactory = hashFactory ?? throw new ArgumentNullException(nameof(hashFactory));
            _addressTransformer = addressTransformer ?? throw new ArgumentNullException(nameof(addressTransformer));
        }

        public BolAccount Map(Bol.Api.Model.BolAccount account)
        {
            var bolAccount = new BolAccount
            {
                AccountStatus = (AccountStatus)int.Parse(account.AccountStatus),
                AccountType = (AccountType)int.Parse(account.AccountType),
                CodeName = Encoding.ASCII.GetString(_hex.Decode(account.CodeName)),
                Edi = account.Edi,
                MainAddress = ConvertToAddress(account.MainAddress),
                BlockChainAddress = ConvertToAddress(account.BlockChainAddress),
                SocialAddress = ConvertToAddress(account.SocialAddress),
                VotingAddress = ConvertToAddress(account.VotingAddress),
                CommercialAddresses = new HashSet<string>(account.CommercialAddresses.Keys.Select(ConvertToAddress)),
                CommercialBalances = account.CommercialAddresses.ToDictionary(pair => ConvertToAddress(pair.Key), pair => ConvertToDecimal(pair.Value)),
                ClaimBalance = ConvertToDecimal(account.ClaimBalance),
                RegistrationHeight = int.Parse(account.RegistrationHeight),
                LastClaimHeight = int.Parse(account.LastClaimHeight),
                IsCertifier = account.IsCertifier == "1",
                Collateral = ConvertToDecimal(account.Collateral),
                CertificationFee = ConvertToDecimal(account.CertificationFee),
                Countries = account.Countries,
                Certifications = string.IsNullOrWhiteSpace(account.Certifications) ? 0 : int.Parse(account.Certifications),
                Certifiers = account.Certifiers,
                MandatoryCertifiers = account.MandatoryCertifiers,
                LastCertificationHeight = int.Parse(account.LastCertificationHeight),
                LastCertifierSelectionHeight = int.Parse(account.LastCertifierSelectionHeight)
            };
            bolAccount.TotalBalance = ConvertToDecimal(account.TotalBalance);
            return bolAccount;
        }

        private string ConvertToAddress(string scriptHex)
        {
            return _addressTransformer.ToAddress(_hashFactory.Create(scriptHex));
        }

        private string HexToNumber(string hex)
        {
            if (string.IsNullOrEmpty(hex)) return hex;
            
            var bytes = _hex.Decode(hex);
            var result = new BigInteger(bytes).ToString();
            return result;
        }

        private string ConvertToDecimal(string number)
        {
            if (string.IsNullOrWhiteSpace(number))
                return "0,00000000";
            else if (number.Length > 8)
                return number.Insert(number.Length - 8, ",");
            else
                return "0,".PadRight(10-number.Length, '0') + number;
        }
    }

    public interface IAccountToAccountMapper
    {
        BolAccount Map(Bol.Api.Model.BolAccount account);
    }
}
