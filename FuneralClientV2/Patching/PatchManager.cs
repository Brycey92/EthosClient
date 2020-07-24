﻿using FuneralClientV2.Settings;
using FuneralClientV2.Utils;
using FuneralClientV2.Wrappers;
using Harmony;
using Il2CppSystem.Runtime.Remoting.Messaging;
using Il2CppSystem.Security.Cryptography;
using MelonLoader;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnhollowerBaseLib;
using UnityEngine;
using VRC;
using VRC.UI;
using VRCSDK2;
using static VRC.SDKBase.VRC_EventHandler;

namespace FuneralClientV2.Patching
{
    public static class PatchManager
    {
        private static HarmonyMethod GetLocalPatch(string name) { return new HarmonyMethod(typeof(PatchManager).GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic)); }

        private static List<Patch> RetrievePatches()
        {
            var ConsoleWriteLine = AccessTools.Method(typeof(Il2CppSystem.Console), "WriteLine", new Type[] { typeof(string) });
            //Credit to Dubya for finding out the old way is scuffed and Knah thinking to patch icalls rather than just methods :bigbrain:
            if (Configuration.GetConfig().SpoofHWID)
            {
                unsafe
                {
                    var MainHWID = UnityEngine.SystemInfo.deviceUniqueIdentifier;
                    var mainmethod = IL2CPP.il2cpp_resolve_icall("UnityEngine.SystemInfo::GetDeviceUniqueIdentifier");
                    Imports.Hook((IntPtr)(&mainmethod), AccessTools.Method(typeof(Main), "FakeDeviceID").MethodHandle.GetFunctionPointer());
                    ConsoleUtil.Info($"Old HWID: {MainHWID}\nNew HWID: {UnityEngine.SystemInfo.deviceUniqueIdentifier}");
                }
            }
            List <Patch> patches = new List<Patch>()
            {
                new Patch("WorldTriggers", AccessTools.Method(typeof(VRC_EventHandler), "InternalTriggerEvent", null, null), GetLocalPatch("TriggerEvent"), null),
                //new Patch("HWIDSpoofer", typeof(VRC.Core.API).GetMethod("get_DeviceID"), GetLocalPatch("SpoofDeviceID"), null), // Removed because it actually won't protect you at all.
                new Patch("AntiKick", typeof(ModerationManager).GetMethod("KickUserRPC"), GetLocalPatch("AntiKick"), null),
                new Patch("AntiPublicBan", typeof(ModerationManager).GetMethod("Method_Public_Boolean_String_String_String_1"), GetLocalPatch("CanEnterPublicWorldsPatch"), null),
                new Patch("AntiBlock", typeof(ModerationManager).GetMethod("BlockStateChangeRPC"), GetLocalPatch("AntiBlock"), null),
                new Patch("ForceClone", typeof(UserInteractMenu).GetMethod("Update"), GetLocalPatch("CloneAvatarPrefix"), null),
                new Patch("CleanConsole", ConsoleWriteLine, GetLocalPatch("IL2CPPConsoleWriteLine"), null),
                new Patch("DownloadImage", typeof(ImageDownloader).GetMethod("DownloadImage"), GetLocalPatch("AntiIpLogImage"), null),
                new Patch("PhotonViewSerialisation", typeof(PhotonView).GetMethod("Method_Public_Void_1"), GetLocalPatch("CustomSerialisation"), null),
                new Patch("VideoPlayers", typeof(VRCSDK2.VRC_SyncVideoPlayer).GetMethod("AddURL"), GetLocalPatch("AntiVideoPlayerHijacking"), null),
                new Patch("EmoteMenuFix", typeof(VRCUiCurrentRoom).GetMethod("Method_Private_Void_17"), GetLocalPatch("NonExistentPrefix"), null) //stupid fix to fix emote menu not working :(
            };
            return patches;
        }

        public static void ApplyPatches()
        {
            var patches = RetrievePatches();
            foreach (var patch in patches) patch.ApplyPatch();
            ConsoleUtil.Info("All Patches have been applied successfully.");
        }

        #region Patches
        private static bool TriggerEvent(ref VrcEvent __0, ref VrcBroadcastType __1, ref int __2, ref float __3)
        {
            if (__1 == VrcBroadcastType.Always || __1 == VrcBroadcastType.AlwaysUnbuffered) if (!GeneralUtils.WorldTriggers) return false;
            if (GeneralUtils.WorldTriggers) __1 = VrcBroadcastType.Always; // really scuffed yaekith we need to fix this. lol - 404
            return true;
        }

        private static bool SpoofDeviceID(ref string __result)
        {
            __result = GeneralUtils.RandomString(50);
            ConsoleUtil.Info($"[HWID Spoofer] New HWID: {__result}"); // removed rn
            return true;
        }

        private static bool AntiKick(ref string __0, ref string __1, ref string __2, ref string __3, ref Player __4)
        {
            //to-do; add support for moderation logging
            return !Configuration.GetConfig().AntiKick;
        }

        private static bool AntiBlockAntiBlock(ref string __0, ref bool __1, ref Player __2)
        {
            //to-do; add support for moderation logging
            var target = GeneralWrappers.GetPlayerManager().GetPlayer(__0);
            var them = __2.GetAPIUser();
            return !Configuration.GetConfig().AntiBlock;
        }

        private static void NonExistentPrefix() { }

        private static bool CloneAvatarPrefix(ref UserInteractMenu __instance)
        {
            bool result = true;
            if (__instance.menuController.activeAvatar.releaseStatus != "private")
            {
                bool flag2 = !__instance.menuController.activeUser.allowAvatarCopying;
                if (flag2)
                {
                    __instance.cloneAvatarButton.gameObject.SetActive(true);
                    __instance.cloneAvatarButton.interactable = true;
                    __instance.cloneAvatarButtonText.color = new Color(0.8117647f, 0f, 0f, 1f);
                    result = false;
                }
                else
                {
                    __instance.cloneAvatarButton.gameObject.SetActive(true);
                    __instance.cloneAvatarButton.interactable = true;
                    __instance.cloneAvatarButtonText.color = new Color(0.470588237f, 0f, 0.8117647f, 1f);
                    result = false;
                }
            }
            return result;
        }

        private static bool IL2CPPConsoleWriteLine(string __0) { return !Configuration.GetConfig().CleanConsole; }

        private static bool AntiIpLogImage(string __0)
        {
            if (__0.StartsWith("https://api.vrchat.cloud/api/1/file/") || __0.StartsWith("https://api.vrchat.cloud/api/1/image/") || __0.StartsWith("https://d348imysud55la.cloudfront.net/thumbnails/") || __0.StartsWith("https://files.vrchat.cloud/thumbnails/")) return true;
            return !Configuration.GetConfig().AntiIpLog;
        }

        private static bool AntiVideoPlayerHijacking(ref string __0)
        {
            if (Configuration.GetConfig().AntiIpLog && GeneralUtils.IsGrabifyLink(__0)) return false;
            return true;
        }

        private static bool CanEnterPublicWorldsPatch(ref bool __result, ref string __0, ref string __1, ref string __2)
        {
            __result = !Configuration.GetConfig().AntiPublicBan;
            return false;
        }

        public static IntPtr FakeDeviceID()
        {
            if (string.IsNullOrEmpty(Configuration.GetConfig().HWID))
            {
                var random = new System.Random();
                Configuration.GetConfig().HWID = KeyedHashAlgorithm.Create().ComputeHash(Encoding.UTF8.GetBytes(string.Format("{0}B-{1}1-C{2}-{3}A-{4}{5}-{6}{7}", new object[]
                {
                    random.Next(1, 9),
                    random.Next(1, 9),
                    random.Next(1, 9),
                    random.Next(1, 9), // this takes literally 3ms but looks like it would take forever XD
                    random.Next(1, 9),
                    random.Next(1, 9),
                    random.Next(1, 9),
                    random.Next(1, 9)
                }))).Select((byte x) =>
                {
                    return x.ToString("x2");
                }).Aggregate((string x, string y) => x + y);
                Configuration.SaveConfiguration();
            }
            if (Configuration.HWIDP == IntPtr.Zero)
                Configuration.HWIDP = new Il2CppSystem.Object(IL2CPP.ManagedStringToIl2Cpp(Configuration.GetConfig().HWID)).Pointer;
            return Configuration.HWIDP;
        }

        private static bool CustomSerialisation()
        {
            return !GeneralUtils.DontSerialise;
        }
    }
    #endregion
}
