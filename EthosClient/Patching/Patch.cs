using EthosClient.Utils;
using Harmony;
using Il2CppSystem.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MethodBase = System.Reflection.MethodBase;

namespace EthosClient.Patching
{
    public class Patch
    {
        public static Dictionary<string, HarmonyInstance> PatchIDs = new Dictionary<string, HarmonyInstance>();

        public string ID { get; set; }

        public MethodBase TargetMethod { get; set; }

        public HarmonyMethod Prefix { get; set; }

        public HarmonyMethod Postfix { get; set; }

        public Patch(string Identifier, MethodBase Target, HarmonyMethod Before, HarmonyMethod After)
        {
            ID = Identifier;
            TargetMethod = Target;
            Prefix = Before;
            Postfix = After;

            if (!PatchIDs.ContainsKey(ID))
            {
                HarmonyInstance instance = HarmonyInstance.Create(ID);
                PatchIDs.Add(ID, instance);
            }

            if (GeneralUtils.IsDevBranch)
                ConsoleUtil.Info($"Patching {ID}\nTarget Method: {Target.Name}\nPrefix: {Before.methodName}\nAfter: {(After == null ? "None." : After.methodName)}");

            PatchIDs[ID].Patch(TargetMethod, Prefix, Postfix);
        }
    }
}
