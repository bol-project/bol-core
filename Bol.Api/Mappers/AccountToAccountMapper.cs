using System;
using System.Collections.Generic;
using System.Linq;
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
                CommercialAddresses = new HashSet<string>(account.CommercialAddresses.Keys.Select(ConvertToAddress)),
                CommercialBalances = account.CommercialAddresses.ToDictionary(pair => ConvertToAddress(pair.Key), pair => ConvertToDouble(pair.Value)),
                ClaimBalance = ConvertToDouble(account.ClaimBalance),
                RegistrationHeight = int.Parse(account.RegistrationHeight),
                LastClaimHeight = int.Parse(account.LastClaimHeight),
                IsCertifier = account.IsCertifier == "1",
                Collateral = ConvertToDouble(account.Collateral),
                Certifications = string.IsNullOrWhiteSpace(account.Certifications) ? 0 : int.Parse(account.Certifications),
                MandatoryCertifier = Encoding.ASCII.GetString(_hex.Decode(account.MandatoryCertifier))
            };
            bolAccount.TotalBalance = bolAccount.CommercialBalances.Values.Sum();
            return bolAccount;
        }

        private string ConvertToAddress(string scriptHex)
        {
            return _addressTransformer.ToAddress(_hashFactory.Create(scriptHex));
        }

        private double ConvertToDouble(string number)
        {
            if (string.IsNullOrWhiteSpace(number))
                return 0;
            else if (number.Length > 8)
                return double.Parse(number.Insert(number.Length - 8, ","));
            else
                return double.Parse("0," + number);
        }
    }

    public interface IAccountToAccountMapper
    {
        BolAccount Map(Bol.Api.Model.BolAccount account);
    }
}
