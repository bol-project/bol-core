﻿using Bol.Core.Services;
using Neo.Network.P2P.Payloads;
using Neo.Wallets;
using System.Collections.Generic;

namespace Bol.Core.Abstractions
{
    public interface IContractService
    {
        InvocationTransaction DeployContract(byte[] script, string name, string version, string author, string email, string description, IEnumerable<KeyPair> keys);
        InvocationTransaction InvokeContract(string contract, string operation, IEnumerable<byte[]> parameters, IEnumerable<KeyPair> keys);
        ContractExecutionResult TestContract(string contract, string operation, IEnumerable<byte[]> parameters, IEnumerable<KeyPair> keys);
        ContractExecutionResult TestContract(string contract, string operation, IEnumerable<byte[]> parameters);
    }
}