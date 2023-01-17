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

        public BolAccount Map(BolAccount account)
        {
            var bolAccount = new BolAccount
            {
                AccountStatus = account.AccountStatus,
                AccountType = account.AccountType,
                CodeName = Encoding.ASCII.GetString(_hex.Decode(account.CodeName)),
                Edi = account.Edi,
                MainAddress = ConvertToAddress(account.MainAddress),
                BlockChainAddress = ConvertToAddress(account.BlockChainAddress),
                SocialAddress = ConvertToAddress(account.SocialAddress),
                VotingAddress = ConvertToAddress(account.VotingAddress),
                CommercialAddresses = new HashSet<string>(account.CommercialAddresses.Select(ConvertToAddress)),
                CommercialBalances = account.CommercialBalances.ToDictionary(pair => ConvertToAddress(pair.Key), pair => ConvertToDecimal(pair.Value)),
                ClaimBalance = ConvertToDecimal(account.ClaimBalance),
                RegistrationHeight = account.RegistrationHeight,
                LastClaimHeight = account.LastClaimHeight,
                IsCertifier = account.IsCertifier,
                Collateral = ConvertToDecimal(account.Collateral),
                CertificationFee = ConvertToDecimal(HexToNumber(account.CertificationFee)),
                Countries = Encoding.ASCII.GetString(_hex.Decode(account.Countries)),
                Certifications = account.Certifications,
                Certifiers = account.Certifiers.ToDictionary(pair => Encoding.ASCII.GetString(_hex.Decode(pair.Key)), pair => pair.Value),
                MandatoryCertifiers = account.MandatoryCertifiers.ToDictionary(pair => Encoding.ASCII.GetString(_hex.Decode(pair.Key)), pair => pair.Value),
                CertificationRequests = account.CertificationRequests.ToDictionary(pair => Encoding.ASCII.GetString(_hex.Decode(pair.Key)), pair => pair.Value),
                LastCertificationHeight = account.LastCertificationHeight,
                LastCertifierSelectionHeight = account.LastCertifierSelectionHeight,
                LastClaim = ConvertToDecimal(account.LastClaim),
                TransactionsCount = account.TransactionsCount,
                Transactions = account.Transactions.ToDictionary(pair => pair.Key, pair => new BolTransactionEntry
                {
                    TransactionHash = pair.Value.TransactionHash,
                    TransactionType = pair.Value.TransactionType,
                    SenderCodeName = !string.IsNullOrWhiteSpace(pair.Value.SenderCodeName) 
                        ? Encoding.ASCII.GetString(_hex.Decode(pair.Value.SenderCodeName))
                        : null,
                    SenderAddress = !string.IsNullOrWhiteSpace(pair.Value.SenderAddress)
                        ? ConvertToAddress(pair.Value.SenderAddress)
                        : null,
                    ReceiverCodeName = !string.IsNullOrWhiteSpace(pair.Value.ReceiverCodeName)
                        ? Encoding.ASCII.GetString(_hex.Decode(pair.Value.ReceiverCodeName))
                        : null,
                    ReceiverAddress = !string.IsNullOrWhiteSpace(pair.Value.ReceiverAddress)
                        ? ConvertToAddress(pair.Value.ReceiverAddress)
                        : null,
                    Amount = ConvertToDecimal(pair.Value.Amount)
                })
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
        BolAccount Map(BolAccount account);
    }
}
