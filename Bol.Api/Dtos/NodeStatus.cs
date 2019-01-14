using System.Collections.Generic;

namespace Bol.Api.Dtos
{
    public class NodeStatus
    {
        public uint Block { get; set; }
        public uint Height { get; set; }
        public uint HeaderHeight { get; set; }
        public int ConnectedNodes { get; set; }
        public int DisconnectedNodes { get; set; }
        public IEnumerable<NodeInfo> Nodes { get; set; }
    }

    public class NodeInfo
    {
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public int ListenPort { get; set; }
        public uint? Height { get; set; }
    }
}
