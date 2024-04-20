using System;
using System.IO;
using Neo.Emulation;
using Neo.Emulation.API;
using Neo.VM;

namespace Bol.Coin.Tests.Utils
{
    public static class EmulatorUtils
    {
        public const int ClaimInterval = 5000;
        public static Emulator Create()
        {
            var avmBytes = File.ReadAllBytes("Bol.Coin.avm");
            var chain = new Blockchain();
            var emulator = new Emulator(chain);

            var address = chain.DeployContract("BoL", avmBytes);
            emulator.SetExecutingAccount(address);
            return emulator;
        }

        public static Emulator Create(Action<string> notifyAction)
        {
            var emulator = Create();
            Runtime.OnLogMessage[emulator] = notifyAction;
            return emulator;
        }

        public static bool Execute(this Emulator emulator, byte[] script, CheckWitnessMode mode = CheckWitnessMode.AlwaysTrue)
        {
            emulator.checkWitnessMode = mode;
            emulator.Reset(script, null, "", table: new ScriptTable());
            emulator.Run();

            var result = emulator.GetOutput();
            return result.GetBoolean();
        }

        public static bool Execute(this Emulator emulator, TransactionGrabber grabber, CheckWitnessMode mode = CheckWitnessMode.AlwaysTrue)
        {
            return emulator.Execute(grabber.Transaction.ExecutionScript.GetBytes(), mode);
        }
    }
}
