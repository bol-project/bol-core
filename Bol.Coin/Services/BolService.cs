using Bol.Coin.Abstractions;
using Neo.SmartContract.Framework;
using System;

namespace Bol.Coin.Services
{
    public class BolService
    {
        public string Name => "Bonus of Life";
        public string Symbol => "BoL";
        public byte[] Owner => "ATrzHaicmhRj15C3Vv6e6gLfLqhSD2PtTr".ToScriptHash();
        public byte Decimals => 8;

        private const ulong starting_amount = 7660500000; //Earth population

        private readonly IBolRepository _repository;
        private readonly IRuntimeService _runtimeService;
        private readonly IBlockChainService _blockChainService;
        private readonly Action<byte[], byte[], double> _transferred;

        public BolService(
            IBolRepository repository,
            IRuntimeService runtimeService,
            IBlockChainService blockChainService,
            Action<byte[], byte[], double> transferred)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _runtimeService = runtimeService ?? throw new ArgumentNullException(nameof(runtimeService));
            _blockChainService = blockChainService ?? throw new ArgumentNullException(nameof(blockChainService));
            _transferred = transferred ?? throw new ArgumentNullException(nameof(transferred));
        }

        public bool Register(byte[] address, byte[] codeName, byte[] edi)
        {
            ThrowOnBadAddress(address);
            ThrowIfNotAddressOwner(address);

            if (codeName == null || codeName.Length == 0) throw new ArgumentNullException("CodeName cannot be empty");
            if (edi == null || edi.Length == 0) throw new ArgumentNullException("Edi cannot be empty");
            if (edi.Length != 32) throw new ArgumentException("EDI length must be 32 bytes");

            //TODO: Add Validations for CodeName

            var currentHeight = _blockChainService.GetCurrentHeight();

            _repository.SetCodeName(address, codeName);
            _repository.SetEdi(address, edi);
            _repository.SetRegistrationHeight(address, currentHeight);
            _repository.SetLastClaimHeight(address, currentHeight);
            _repository.SetBols(address, 1); // Claims 1st Bol
            _repository.RemoveBols(1); //Remove 1 from central wallet
            _repository.AddRegisteredPerson();

            _transferred(Owner, address, 1);

            return true;
        }

        public bool Deploy()
        {
            var totalSupply = _repository.GetBols();

            if (totalSupply > 0) return false;

            _repository.SetBols(starting_amount);
            _transferred(null, Owner, starting_amount);
            return true;
        }

        public double TotalSupply()
        {
            return _repository.GetBols();
        }

        public bool Transfer(byte[] from, byte[] to, double value)
        {
            ThrowOnBadAddress(from);
            ThrowOnBadAddress(to);
            ThrowIfNotAddressOwner(from);

            if (value <= 0) throw new ArgumentException("Cannot transfer negative value");

            var fromBalance = _repository.GetBols(from);

            if (fromBalance < value) throw new Exception("Cannot transfer more Bols that account balance");

            var certifications = _repository.GetCertifications(from);

            if (certifications == 0) throw new Exception("Cannot transfer Bols unless certified by valid certifier.");

            //TODO: Validation needs rework because one can make many small transfers
            if (value > 10 && certifications < 3) throw new Exception("Cannot transfer more than 10 Bols unless certifiex by 3 valid certifiers.");

            _repository.RemoveBols(from, value);
            _repository.AddBols(to, value);

            _transferred(from, to, value);
            return true;
        }

        public double GetBalance(byte[] address)
        {
            ThrowOnBadAddress(address);

            return _repository.GetBols(address);
        }

        public void RegisterAsCertifier(byte[] address)
        {
            ThrowOnBadAddress(address);
            ThrowIfNotAddressOwner(address);

            if (_repository.IsCertifier(address)) throw new Exception("Already a certifier.");

            _repository.RegisterAsCertifier(address);
            _repository.BindCollateral(address);
        }

        public void UnregisterAsCertifier(byte[] address)
        {
            ThrowOnBadAddress(address);
            ThrowIfNotAddressOwner(address);

            if (!_repository.IsCertifier(address)) throw new Exception("Not a certifier.");

            _repository.UnregisterAsCertifier(address);
            _repository.ReleaseCollateral(address);
        }

        public void Certify(byte[] certifier, byte[] address)
        {
            ThrowOnBadAddress(certifier);
            ThrowOnBadAddress(address);
            ThrowIfNotAddressOwner(certifier);

            if (!_repository.IsCertifier(certifier)) throw new Exception("Not a certifier.");

            _repository.AddCertification(address, certifier);
        }

        public void UnCertify(byte[] certifier, byte[] address)
        {
            ThrowOnBadAddress(certifier);
            ThrowOnBadAddress(address);
            ThrowIfNotAddressOwner(certifier);

            if (!_repository.IsCertifier(certifier)) throw new Exception("Not a certifier.");

            _repository.RemoveCertification(address, certifier);
        }

        public void Claim(byte[] address)
        {
            ThrowOnBadAddress(address);
            ThrowIfNotAddressOwner(address);

            var codeName = _repository.GetCodeName(address);

            if (codeName.Length == 0) throw new ArgumentException("Not a registered address.");

            var previousHeight = _repository.GetLastClaimHeight(address);
            var currentHeight = _blockChainService.GetCurrentHeight();

            var diff = currentHeight - previousHeight;

            var bonus = diff * 100; //TODO: Fix the calculation according to the algorithm

            _repository.AddBols(address, bonus);
            _repository.RemoveBols(bonus);

            _repository.SetLastClaimHeight(address, currentHeight);

            _transferred(Owner, address, bonus);
        }

        internal void ThrowOnBadAddress(byte[] address)
        {
            if (address == null || address.Length == 0) throw new ArgumentNullException("Address cannot be empty");
            if (address.Length != 20) throw new ArgumentException("Address length must be 20 bytes");
        }

        internal void ThrowIfNotAddressOwner(byte[] address)
        {
            if (!_runtimeService.ValidateCallerAddress(address)) throw new ArgumentException("Only the Address owner can perform this action.");
        }
    }
}
