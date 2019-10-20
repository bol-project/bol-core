using System;
using System.Numerics;
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
                if (operation == "totalSupply") return BolService.TotalSupply();
                if (operation == "name") return BolService.Name();
                if (operation == "symbol") return BolService.Symbol();
                if (operation == "transfer")
                {
                    if (args.Length != 3)
                    {
                        Runtime.Notify("error", BolResult.BadRequest("Bad number of arguments"));
                        return false;
                    }
                    var from = (byte[])args[0];
                    var to = (byte[])args[1];
                    var value = (BigInteger)args[2];

                    return BolService.Transfer(from, to, value);
                }
                if (operation == "balanceOf")
                {
                    if (args.Length != 1)
                    {
                        Runtime.Notify("error", BolResult.BadRequest("Bad number of arguments"));
                        return false;
                    }
                    var account = (byte[])args[0];

                    return BolService.GetBalance(account);
                }
                if (operation == "decimals") return BolService.Decimals();
                if (operation == "register")
                {
                    if (args.Length != 3)
                    {
                        Runtime.Notify("error", BolResult.BadRequest("Bad number of arguments"));
                        return false;
                    }
                    var address = (byte[])args[0];
                    var codeName = (byte[])args[1];
                    var edi = (byte[])args[2];

                    return BolService.Register(address, codeName, edi);
                }
                if (operation == "registerCertifier")
                {
                    if (args.Length != 1)
                    {
                        Runtime.Notify("error", BolResult.BadRequest("Bad number of arguments"));
                        return false;
                    }
                    var address = (byte[])args[0];

                    return BolService.RegisterAsCertifier(address);
                }
                if (operation == "unregisterCertifier")
                {
                    if (args.Length != 1)
                    {
                        Runtime.Notify("error", BolResult.BadRequest("Bad number of arguments"));
                        return false;
                    }
                    var address = (byte[])args[0];

                    return BolService.UnregisterAsCertifier(address);
                }
                if (operation == "certify")
                {
                    if (args.Length != 2)
                    {
                        Runtime.Notify("error", BolResult.BadRequest("Bad number of arguments"));
                        return false;
                    }
                    var certifier = (byte[])args[0];
                    var address = (byte[])args[1];

                    return BolService.Certify(certifier, address);
                }
                if (operation == "claim")
                {
                    if (args.Length != 1)
                    {
                        Runtime.Notify("error", BolResult.BadRequest("Bad number of arguments"));
                        return false;
                    }
                    var address = (byte[])args[0];
                    return BolService.Claim(address);
                }
                if (operation == "getAccount")
                {
                    if (args.Length != 1)
                    {
                        Runtime.Notify("error", BolResult.BadRequest("Bad number of arguments"));
                        return false;
                    }
                    var mainAddress = (byte[])args[0];
                    return BolService.GetAccount(mainAddress);
                }
                if (operation == "addCommercialAddress")
                {
                    if (args.Length != 2)
                    {
                        Runtime.Notify("error", BolResult.BadRequest("Bad number of arguments"));
                        return false;
                    }
                    var mainAddress = (byte[])args[0];
                    var commercialAddress = (byte[])args[1];
                    return BolService.AddCommercialAddress(mainAddress, commercialAddress);
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
