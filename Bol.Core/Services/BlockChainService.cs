using System;
using System.Collections.Generic;
using System.Linq;
using Bol.Core.Abstractions;
using Bol.Core.Abstractions.Mappers;
using Bol.Core.Dtos;
using Bol.Core.Model.Internal;
using Neo;
using Neo.Ledger;
using Neo.Network.P2P.Payloads;

namespace Bol.Core.Services
{
    public class BlockChainService : IBlockChainService
    {
        private readonly IMapper<Block, BlockDto> _blockDtoMapper;
        private readonly IMapper<TrimmedBlock, BaseBlockDto> _baseBlockDtoMapper;

        public BlockChainService(
            IMapper<Block, BlockDto> blockDtoMapper,
            IMapper<TrimmedBlock, BaseBlockDto> baseBlockDtoMapper)
        {
            _blockDtoMapper = blockDtoMapper ?? throw new ArgumentNullException(nameof(blockDtoMapper));
            _baseBlockDtoMapper = baseBlockDtoMapper ?? throw new ArgumentNullException(nameof(baseBlockDtoMapper));
        }

        public GetContractResult GetContract(string contract)
        {
            var scriptHash = UInt160.Parse(contract);

            var contractState = Blockchain.Singleton.Store.GetContracts().TryGet(scriptHash);

            if (contractState == null)
            {
                return new GetContractResult
                {
                    ContractExists = false,
                    ScriptHash = null
                };
            }
            else
            {
                return new GetContractResult
                {
                    ContractExists = true,
                    ScriptHash = scriptHash
                };
            }
        }

        public BlockDto GetCurrentBlock()
        {
            var currentBlockHash = Blockchain.Singleton.CurrentBlockHash;

            var currentBlock = Blockchain.Singleton.GetBlock(currentBlockHash);

            return _blockDtoMapper.Map(currentBlock);
        }

        public BlockDto GetBlock(string id)
        {
            var hash = UInt256.Parse(id);

            var block = Blockchain.Singleton.GetBlock(hash);

            return _blockDtoMapper.Map(block);
        }

        public IEnumerable<BaseBlockDto> GetBlocks()
        {
            var blocks = Blockchain.Singleton.Store.GetBlocks().Find();

            var trimmedBlocks = blocks.Select(b => b.Value.TrimmedBlock);

            return trimmedBlocks.Select(b => _baseBlockDtoMapper.Map(b));
        }
    }
}
