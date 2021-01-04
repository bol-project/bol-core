using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Bol.Address;
using Bol.Core.Abstractions;
using Bol.Cryptography;
using Bol.Cryptography.Abstractions;
//using Bol.Core.Model.Wallet;
using Neo;
using Neo.Cryptography.ECC;
using Neo.Ledger;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract;
using Neo.VM;
using Neo.Wallets;

namespace Bol.Core.Services
{
    public class ContractService : IContractService
    {
        private readonly ITransactionPublisher _transactionPublisher;
        private readonly IBlockChainService _blockChainService;
        private ISignatureScriptFactory _signatureScriptFactory;
        private IScriptHashFactory _scriptHashFactory;

        public ContractService(
            ITransactionPublisher transactionPublisher,
            IBlockChainService blockChainService,
            ISignatureScriptFactory signatureScriptFactory)
        {
            _transactionPublisher = transactionPublisher ?? throw new ArgumentNullException(nameof(transactionPublisher));
            _blockChainService = blockChainService ?? throw new ArgumentNullException(nameof(blockChainService));
            _signatureScriptFactory = signatureScriptFactory ?? throw new ArgumentNullException(nameof(signatureScriptFactory));
        }

        public InvocationTransaction DeployContract(byte[] script, string name, string version, string author, string email, string description, IEnumerable<Cryptography.IKeyPair> keys, int numberOfSignatures = 0)
        {
            var multiSig = CreateAddressContract(keys, numberOfSignatures);
            var executionScript = CreateContractScript(script, name, version, author, email, description);
            var transaction = CreateTransaction(executionScript, multiSig);
            var transactionSigned = SignTransaction(transaction, multiSig, keys);

            if (!transactionSigned) throw new Exception("Could not sign transaction.");

            SubmitTransaction(transaction);

            return transaction;
        }

        public InvocationTransaction InvokeContract(string contract, string operation, IEnumerable<byte[]> parameters, string description = null, IEnumerable<string> remarks = null, IEnumerable<Cryptography.IKeyPair> keys = null, int numberOfSignatures = 0)
        {
            var transaction = CreateTransaction(contract, operation, parameters, description, remarks, keys, numberOfSignatures);
            InvokeContract(transaction);
            return transaction;
        }

        public void InvokeContract(InvocationTransaction transaction)
        {
            SubmitTransaction(transaction);
        }

        public ContractExecutionResult TestContract(string contract, string operation, IEnumerable<byte[]> parameters, string description = null, IEnumerable<string> remarks = null, IEnumerable<Cryptography.IKeyPair> keys = null, int numberOfSignatures = 0)
        {
            var transaction = CreateTransaction(contract, operation, parameters, description, remarks, keys, numberOfSignatures);

            return TestContract(transaction);
        }

        public ContractExecutionResult TestContract(InvocationTransaction transaction)
        {
            var engine = ApplicationEngine.Run(transaction.Script, transaction);

            if (engine.State.HasFlag(VMState.FAULT)) return ContractExecutionResult.Fail();

            return ContractExecutionResult.Succeed(engine.ResultStack.First().GetByteArray(), engine.GasConsumed);
        }

        //public InvocationTransaction CreateTransaction(string contract, string operation, byte[][] parameters, Cryptography.IKeyPair key, string description = null, IEnumerable<string> remarks = null)
        //{
        //    throw new NotImplementedException();
        //}

        public InvocationTransaction CreateTransaction(string contract, string operation, byte[][] parameters, string description = null, IEnumerable<string> remarks = null, IEnumerable<Cryptography.IKeyPair> keys = null, int numberOfSignatures = 0)
        {
            //TODO: Add validations

            var contractHash = _scriptHashFactory.Create(contract);
            var executionScript = _signatureScriptFactory.CreateContractOperationScript(contractHash, operation, parameters);

            var addressContract = _signatureScriptFactory.Create(keys.Select(key => key.PublicKey), numberOfSignatures);

            var transaction = CreateBolTransaction(executionScript, addressContract.ToScriptHash(), description, remarks);
            var transactionSigned = SignTransaction(transaction, addressContract, keys);

            return transaction;
        }

        private Model.Wallet.Contract CreateAddressContract(IEnumerable<Cryptography.IKeyPair> keys, int numberOfSignatures)
        {
            var parametersList = new List<Model.Wallet.Parameters>();
            var _contract = new Model.Wallet.Contract();
            var publicKeys = keys
                .Select(key => key.PublicKey)
                .ToArray();

            if (publicKeys.Length == 1)
            {

                parametersList = new List<Model.Wallet.Parameters>() {
                    new Model.Wallet.Parameters() { Type = "Signature", Name = "signature"} ,
                 };
                return _contract = new Model.Wallet.Contract
                {
                    Script = _signatureScriptFactory.Create(publicKeys[0]).ToHexString(),
                    Parameters = parametersList,
                    Deployed = false
                };

                //  return Contract.CreateSignatureContract( publicKeys[0]);
            }

            if (numberOfSignatures < 1 || numberOfSignatures > publicKeys.Length)
            {
                throw new Exception("Number of signatures must be in the range of 1 -> Number of Keys");
            }

            parametersList = new List<Model.Wallet.Parameters>() {
                    new Model.Wallet.Parameters() { Type = "Signature", Name = "parameter0"} ,
                     new Model.Wallet.Parameters() { Type = "Signature", Name = "parameter1"} ,
            };
            return _contract = new Model.Wallet.Contract
            {
                Script = _signatureScriptFactory.Create(publicKeys[0]).ToHexString(),
                Parameters = parametersList,
                Deployed = false
            };
            //var multisig = Contract.CreateMultiSigContract(numberOfSignatures, publicKeys);
            // return multisig;
        }

        //TODO: Move to SignatureScriptFactory
        private byte[] CreateContractScript(byte[] script, string name, string version, string author, string email, string description)
        {
            byte[] parameter_list = "0710".HexToBytes();
            var return_type = ContractParameterType.ByteArray;
            var properties = ContractPropertyState.HasStorage;

            using (var sb = new ScriptBuilder())
            {
                sb.EmitSysCall("Neo.Contract.Create", script, parameter_list, return_type, properties, name, version, author, email, description);
                return sb.ToArray();
            }
        }

        private BolTransaction CreateBolTransaction(ISignatureScript executionScript, IScriptHash address, string description = null, IEnumerable<string> remarks = null)
        {
            //var transaction = new BolTransaction
            //{
            //    Version = 1,
            //    Script = executionScript.GetBytes(),
            //    Gas = Fixed8.Zero,
            //    Inputs = new CoinReference[0],
            //    Outputs = new TransactionOutput[0],
            //    Witnesses = new Witness[0]
            //};
            var attributes = new List<BolTransactionAttribute>();

            attributes.Add(
                new BolTransactionAttribute
                {
                    Type = TransactionAttributeType.Script,
                    Value = address.GetBytes()
                });

            if (description != null)
            {
                attributes.Add(
                    new BolTransactionAttribute
                    {
                        Type = TransactionAttributeType.Description,
                        Value = Encoding.UTF8.GetBytes(description),
                    });
            }

            if (remarks != null && remarks.Any())
            {
                var remarkAttributes = Enumerable
                    .Range(0xf0, 0xff)
                    .Zip(remarks, (index, remark) => new BolTransactionAttribute
                    {
                        Type = (TransactionAttributeType)index,
                        Value = Encoding.UTF8.GetBytes(remark),
                    });

                attributes.AddRange(remarkAttributes);
            }

            return new BolTransaction
            {
                ExecutionScript = executionScript,
                Attributes = attributes
            };
        }

        private bool SignTransaction(InvocationTransaction transaction, Model.Wallet.Contract multiSig, IEnumerable<Cryptography.IKeyPair> keys)
        {
            var context = new ContractParametersContext(transaction);

            foreach (var key in keys)
            {

                context.AddSignature(multiSig, key.PublicKey, transaction.Sign(key));
            }

            if (!context.Completed) return false;

            transaction.Witnesses = context.GetWitnesses();
            return true;
        }

        private void SubmitTransaction(InvocationTransaction transaction)
        {
            _transactionPublisher.Publish(transaction);
        }

        public Contract CreateMultiSigContract(int requiredSignatures, params ECPoint[] publicKeys)
        {
            return Contract.CreateMultiSigContract(requiredSignatures, publicKeys);
        }

        public Contract CreateSignatureContract(ECPoint publicKey)
        {
            return Contract.CreateSignatureContract(publicKey);
        }
    }

    public interface ITransactionNotarizer
    {
        BolTransaction Notarize(BolTransaction transaction, IEnumerable<IKeyPair> keys);
    }

    public class TransactionNotarizer : ITransactionNotarizer
    {
        private readonly ITransactionSigner _signer;
        private readonly ISignatureScriptFactory _signatureScriptFactory;

        public BolTransaction Notarize(BolTransaction transaction, IEnumerable<IKeyPair> keys)
        {
            var witnesses = keys.OrderBy(key => key.PublicKey)
                .Select(key =>
                {

                    var signature = _signer.GenerateSignature(transaction, key);
                    var witness = _signatureScriptFactory.Create(key.PublicKey);
                    return new BolTransactionWitness
                    {
                        InvocationScript = signature,
                        VerificationScript = witness.GetBytes()
                    };
                });
            transaction.Witnesses = witnesses;
            return transaction;
        }
    }

    public interface ITransactionSigner
    {
        byte[] GenerateSignature(BolTransaction transaction, IKeyPair key);
    }

    public class TransactionSigner : ITransactionSigner
    {
        private readonly ITransactionHasher _transactionHasher;
        private readonly IECCurveSigner _signer;

        public TransactionSigner(ITransactionHasher transactionHasher, IECCurveSigner signer)
        {
            _transactionHasher = transactionHasher ?? throw new ArgumentNullException(nameof(transactionHasher));
            _signer = signer ?? throw new ArgumentNullException(nameof(signer));
        }

        public byte[] GenerateSignature(BolTransaction transaction, IKeyPair key)
        {
            var transactionHash = _transactionHasher.Hash(transaction);
            return _signer.Sign(transactionHash, key.PrivateKey, key.PublicKey.ToRawValue());
        }
    }

    public interface ITransactionHasher
    {
        byte[] Hash(BolTransaction transaction);
    }

    public class TransactionHasher : ITransactionHasher
    {
        public byte[] Hash(BolTransaction transaction)
        {
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms);

            writer.Write((byte)0xd1); //Writes 0xd1 as InvocationTransaction Type
            writer.Write(1); //Writes 1 as Transaction Version

            writer.WriteVarBytes(transaction.ExecutionScript.GetBytes());

            writer.Write(0); //Writes 0 for GAS for Transaction Version >= 1

            //Writes Attributes
            writer.WriteVarInt(transaction.Attributes.Count());
            foreach(var attr in transaction.Attributes)
            {
                writer.Write((byte)attr.Type);
                if (attr.Type == TransactionAttributeType.DescriptionUrl)
                    writer.Write((byte)attr.Value.Length);
                else if (attr.Type == TransactionAttributeType.Description || attr.Type >= TransactionAttributeType.Remark)
                    writer.WriteVarInt(attr.Value.Length);
                if (attr.Type == TransactionAttributeType.ECDH02 || attr.Type == TransactionAttributeType.ECDH03)
                    writer.Write(attr.Value, 1, 32);
                else
                    writer.Write(attr.Value);
            }

            writer.Write(0); //Writes 0 as empty Inputs
            writer.Write(0); //Writes 0 as empty Outputs

            writer.Flush();
            return ms.ToArray();
        }
    }

    public class BolTransaction
    {
        public ISignatureScript ExecutionScript { get; set; }
        public IEnumerable<BolTransactionAttribute> Attributes { get; set; }
        public IEnumerable<BolTransactionWitness> Witnesses { get; set; } = new List<BolTransactionWitness>();
    }

    public class BolTransactionWitness
    {
        public byte[] InvocationScript { get; set; }
        public byte[] VerificationScript { get; set; }
    }

    public class BolTransactionAttribute
    {
        public TransactionAttributeType Type { get; set; }
        public byte[] Value { get; set; }
    }
    public enum TransactionAttributeType : byte
    {
        ContractHash = 0x00,

        ECDH02 = 0x02,
        ECDH03 = 0x03,

        Script = 0x20,

        Vote = 0x30,

        DescriptionUrl = 0x81,
        Description = 0x90,

        Hash1 = 0xa1,
        Hash2 = 0xa2,
        Hash3 = 0xa3,
        Hash4 = 0xa4,
        Hash5 = 0xa5,
        Hash6 = 0xa6,
        Hash7 = 0xa7,
        Hash8 = 0xa8,
        Hash9 = 0xa9,
        Hash10 = 0xaa,
        Hash11 = 0xab,
        Hash12 = 0xac,
        Hash13 = 0xad,
        Hash14 = 0xae,
        Hash15 = 0xaf,

        Remark = 0xf0,
        Remark1 = 0xf1,
        Remark2 = 0xf2,
        Remark3 = 0xf3,
        Remark4 = 0xf4,
        Remark5 = 0xf5,
        Remark6 = 0xf6,
        Remark7 = 0xf7,
        Remark8 = 0xf8,
        Remark9 = 0xf9,
        Remark10 = 0xfa,
        Remark11 = 0xfb,
        Remark12 = 0xfc,
        Remark13 = 0xfd,
        Remark14 = 0xfe,
        Remark15 = 0xff
    }
}
