using Bol.Coin.Helpers;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.ComponentModel;

namespace Neo.SmartContract
{
    public class Bol : Framework.SmartContract
    {
        [DisplayName("transfer")]
        public static event Action<byte[], byte[], double> Transferred;

        public static Object Main(string operation, params object[] args)
        {
            try
            {
                var service = BolFactory.CreateBolService(Transferred);

                if (Runtime.Trigger == TriggerType.Verification)
                {
                    return false; //Transferring from Contract account is forbidden
                }
                else if (Runtime.Trigger == TriggerType.Application)
                {
                    if (operation == "deploy") return service.Deploy();
                    if (operation == "totalSupply") return service.TotalSupply();
                    if (operation == "name") return service.Name;
                    if (operation == "symbol") return service.Symbol;
                    if (operation == "transfer")
                    {
                        if (args.Length != 3) return false;
                        byte[] from = (byte[])args[0];
                        byte[] to = (byte[])args[1];
                        double value = (double)args[2];
                        return service.Transfer(from, to, value);
                    }
                    if (operation == "balanceOf")
                    {
                        if (args.Length != 1) return 0;
                        byte[] account = (byte[])args[0];
                        return service.GetBalance(account);
                    }
                    if (operation == "decimals") return service.Decimals;
                    if (operation == "register")
                    {
                        if (args.Length != 3) return false;
                        byte[] address = (byte[])args[0];
                        byte[] codeName = (byte[])args[1];
                        byte[] edi = (byte[])args[2];
                        return service.Register(address, codeName, edi);
                    }
                    if (operation == "registerCertifier")
                    {
                        if (args.Length != 1) return false;
                        byte[] address = (byte[])args[0];
                        service.RegisterAsCertifier(address);
                    }
                    if (operation == "unregisterCertifier")
                    {
                        if (args.Length != 1) return false;
                        byte[] address = (byte[])args[0];
                        service.UnregisterAsCertifier(address);
                    }
                    if (operation == "certify")
                    {
                        if (args.Length != 2) return false;
                        byte[] certifier = (byte[])args[0];
                        byte[] address = (byte[])args[1];
                        service.Certify(certifier, address);
                    }
                    if (operation == "uncertify")
                    {
                        if (args.Length != 2) return false;
                        byte[] certifier = (byte[])args[0];
                        byte[] address = (byte[])args[1];
                        service.UnCertify(certifier, address);
                    }
                    if (operation == "claim")
                    {
                        if (args.Length != 1) return false;
                        byte[] address = (byte[])args[0];
                        service.Claim(address);
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}