using LunarLabs.Parser;

namespace Bol.Coin.Tests.Helper
{
    public class DebugParam
    {

        public   DebugParam() { }
        public DataNode CreateDebugParam(string method, object[] param)
        {
            var inputs = DataNode.CreateArray();
            inputs.AddValue(method);

            if (param != null && param.Length > 0)
            {
                var _param = DataNode.CreateArray();
                foreach (var o in param)
                {
                    _param.AddValue(o);
                }
                inputs.AddNode(_param);
            }
            else
            {
                inputs.AddValue(null);
            }
            return inputs;
        }
    }
}
