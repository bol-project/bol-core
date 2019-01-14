using Bol.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Neo.Ledger;
using Neo.Network.P2P;
using System.Linq;

namespace Bol.Api.Controllers
{
    [Route("api/nodes")]
    public class NodesController : ControllerBase
    {
        [HttpGet("status")]
        public ActionResult GetStatus()
        {
            uint wh = (Neo.Program.Wallet.WalletHeight > 0) ? Neo.Program.Wallet.WalletHeight - 1 : 0;

            var status = new NodeStatus
            {
                Block = wh,
                Height = Blockchain.Singleton.Height,
                HeaderHeight = Blockchain.Singleton.HeaderHeight,
                ConnectedNodes = LocalNode.Singleton.ConnectedCount,
                DisconnectedNodes = LocalNode.Singleton.UnconnectedCount,
                Nodes = LocalNode.Singleton.GetRemoteNodes()
                    .Select(n => new NodeInfo
                    {
                        IpAddress = n.Remote.Address.ToString(),
                        Port = n.Remote.Port,
                        ListenPort = n.ListenerPort,
                        Height = n.Version?.StartHeight
                    })
            };

            return Ok(status);
        }
    }
}
