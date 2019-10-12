using Bol.Coin.Helpers;
using Bol.Coin.Services;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.Numerics;

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
                    if (args.Length != 3) return false;
                    byte[] from = (byte[])args[0];
                    byte[] to = (byte[])args[1];
                    BigInteger value = (BigInteger)args[2];
                    //Runtime.Log(value.AsByteArray().AsString());
                    return JsonHelper.AsJson(BolService.Transfer(from, to, value));
                }
                if (operation == "balanceOf")
                {
                    if (args.Length != 1) return 0;
                    byte[] account = (byte[])args[0];
                    return BolService.GetBalance(account);
                }
                if (operation == "decimals") return BolService.Decimals();
                if (operation == "register")
                {
                    if (args.Length != 3) return false;
                    byte[] address = (byte[])args[0];
                    byte[] codeName = (byte[])args[1];
                    byte[] edi = (byte[])args[2];

                    return JsonHelper.AsJson(BolService.Register(address, codeName, edi));
                }
                if (operation == "registerCertifier")
                {
                    if (args.Length != 1) return false;
                    byte[] address = (byte[])args[0];
                    return JsonHelper.AsJson(BolService.RegisterAsCertifier(address));
                }
                if (operation == "unregisterCertifier")
                {
                    if (args.Length != 1) return false;
                    byte[] address = (byte[])args[0];
                    return JsonHelper.AsJson(BolService.UnregisterAsCertifier(address));
                }
                if (operation == "certify")
                {
                    if (args.Length != 2) return false;
                    byte[] certifier = (byte[])args[0];
                    byte[] address = (byte[])args[1];
                    return JsonHelper.AsJson(BolService.Certify(certifier, address));
                }
                if (operation == "claim")
                {
                    if (args.Length != 1) return false;
                    byte[] address = (byte[])args[0];
                    return JsonHelper.AsJson(BolService.Claim(address));
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
