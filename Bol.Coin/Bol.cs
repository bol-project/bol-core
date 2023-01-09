using System;
using System.Numerics;
using Bol.Coin;
using Bol.Coin.Models;
using Bol.Coin.Services;
using Neo.SmartContract.Framework.Services.Neo;

namespace Neo.SmartContract
{
    public class Bol : Framework.SmartContract
    {

        public static Object Main(string operation, params object[] args)
        {
            if (Runtime.Trigger == TriggerType.Verification)
            {
                return false; //Transferring from Contract account is forbidden
            }
            else if (Runtime.Trigger == TriggerType.Application)
            {
                if (operation == "deploy") return BolService.Deploy();
                if (operation == "totalSupply") return BolService.CirculatingSupply();
                if (operation == "circulatingSupply") return BolService.CirculatingSupply();
                if (operation == "name") return Constants.Name;
                if (operation == "symbol") return Constants.Symbol;
                if (operation == "globalSupply")
                {
                    if (args.Length != 1)
                    {
                        Runtime.Notify("error", BolResult.BadRequest("Bad number of arguments"));
                        return false;
                    }
                    return BolService.GlobalSupply((BigInteger)args[0]);
                }
                if (operation == "transferClaim")
                {
                    if (args.Length != 3)
                    {
                        Runtime.Notify("error", BolResult.BadRequest("Bad number of arguments"));
                        return false;
                    }
                    var codeName = (byte[])args[0];
                    var address = (byte[])args[1];
                    var value = (BigInteger)args[2];

                    return BolService.TransferClaim(codeName, address, value);
                }
                if (operation == "transfer")
                {
                    if (args.Length != 5)
                    {
                        Runtime.Notify("error", BolResult.BadRequest("Bad number of arguments"));
                        return false;
                    }
                    var from = (byte[])args[0];
                    var senderCodeName = (byte[])args[1]; 
                    var to = (byte[])args[2];
                    var targetCodeName = (byte[])args[3];
                    var value = (BigInteger)args[4];

                    return BolService.Transfer(from, senderCodeName, to, targetCodeName, value);
                }
                if (operation == "balanceOf")
                {
                    Runtime.Notify("error", BolResult.BadRequest("Function not supported"));
                    return false;
                }
                if (operation == "decimals") return Constants.Decimals;
                if (operation == "register")
                {
                    if (args.Length != 7)
                    {
                        Runtime.Notify("error", BolResult.BadRequest("Bad number of arguments"));
                        return false;
                    }
                    var address = (byte[])args[0];
                    var codeName = (byte[])args[1];
                    var edi = (byte[])args[2];
                    var blockChainAddress = (byte[])args[3];
                    var socialAddress = (byte[])args[4];
                    var votingAddress = (byte[])args[5];
                    var commercialAddresses = (byte[])args[6];

                    return BolService.RegisterAccount(address, codeName, edi, blockChainAddress, socialAddress, votingAddress, commercialAddresses);
                }
                if (operation == "registerCertifier")
                {
                    if (args.Length != 3)
                    {
                        Runtime.Notify("error", BolResult.BadRequest("Bad number of arguments"));
                        return false;
                    }
                    var codeName = (byte[])args[0];
                    var countries = (byte[])args[1];
                    var fee = (BigInteger)args[2];

                    return BolService.RegisterAsCertifier(codeName, countries, fee);
                }
                if (operation == "unregisterCertifier")
                {
                    if (args.Length != 1)
                    {
                        Runtime.Notify("error", BolResult.BadRequest("Bad number of arguments"));
                        return false;
                    }
                    var codeName = (byte[])args[0];

                    return BolService.UnregisterAsCertifier(codeName);
                }
                if (operation == "certify")
                {
                    if (args.Length != 2)
                    {
                        Runtime.Notify("error", BolResult.BadRequest("Bad number of arguments"));
                        return false;
                    }
                    var certifier = (byte[])args[0];
                    var receiver = (byte[])args[1];

                    return BolService.Certify(certifier, receiver);
                }
                if (operation == "unCertify")
                {
                    if (args.Length != 2)
                    {
                        Runtime.Notify("error", BolResult.BadRequest("Bad number of arguments"));
                        return false;
                    }
                    var certifier = (byte[])args[0];
                    var receiver = (byte[])args[1];

                    return BolService.UnCertify(certifier, receiver);
                }
                if (operation == "claim")
                {
                    if (args.Length != 1)
                    {
                        Runtime.Notify("error", BolResult.BadRequest("Bad number of arguments"));
                        return false;
                    }
                    var codeName = (byte[])args[0];
                    return BolService.Claim(codeName);
                }
                if (operation == "getAccount")
                {
                    if (args.Length != 1)
                    {
                        Runtime.Notify("error", BolResult.BadRequest("Bad number of arguments"));
                        return false;
                    }
                    var codeName = (byte[])args[0];
                    return BolService.GetAccount(codeName);
                }
                if (operation == "addCommercialAddress")
                {
                    if (args.Length != 2)
                    {
                        Runtime.Notify("error", BolResult.BadRequest("Bad number of arguments"));
                        return false;
                    }
                    var codeName = (byte[])args[0];
                    var commercialAddress = (byte[])args[1];
                    return BolService.AddCommercialAddress(codeName, commercialAddress);
                }
                if (operation == "getCertifiers")
                {
                    if (args.Length != 1)
                    {
                        Runtime.Notify("error", BolResult.BadRequest("Bad number of arguments"));
                        return false;
                    }
                    var countryCode = (byte[])args[0];
                    return BolService.GetCertifiers(countryCode);
                }
                if (operation == "whitelist")
                {
                    if (args.Length != 2)
                    {
                        Runtime.Notify("error", BolResult.BadRequest("Bad number of arguments"));
                        return false;
                    }
                    var codeName = (byte[])args[0];
                    var mainAddress = (byte[])args[1];
                    return BolService.Whitelist(codeName, mainAddress);
                }
                if (operation == "isWhitelisted")
                {
                    if (args.Length != 1)
                    {
                        Runtime.Notify("error", BolResult.BadRequest("Bad number of arguments"));
                        return false;
                    }
                    var mainAddress = (byte[])args[0];
                    return BolService.IsWhitelisted(mainAddress);
                }
                if (operation == "selectMandatoryCertifiers")
                {
                    if (args.Length != 1)
                    {
                        Runtime.Notify("error", BolResult.BadRequest("Bad number of arguments"));
                        return false;
                    }
                    var codeName = (byte[])args[0];
                    return BolService.SelectMandatoryCertifiers(codeName);
                }
            }
            return false;
        }

        internal static byte[] Sha256Hash(byte[] input)
        {
            return Framework.SmartContract.Hash256(input);
        }
    }
}
