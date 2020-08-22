using EthosClient.Settings;
using EthosClient.Utils;
using EthosClient.Wrappers;
using Harmony;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using VRC;
using VRC.SDKBase;
using System.Linq;
using static VRC.SDKBase.VRC_EventHandler;
using VRC.Core;
using VRC.Udon.Serialization.OdinSerializer;
using ExitGames.Client.Photon;
using System.ComponentModel;
using System.Security.Cryptography;

namespace EthosClient.Patching
{
    public static class PatchManager
    {
        private static List<string> PlayerCache = new List<string>();

        private static HarmonyMethod GetLocalPatch(string name) { return new HarmonyMethod(typeof(PatchManager).GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic)); }

        private static void RetrievePatches()
        {
            try
            {
                new Patch("Ethos_Moderation", typeof(ModerationManager).GetMethod("KickUserRPC"), GetLocalPatch("AntiKick"), null);
                new Patch("Ethos_Moderation", typeof(ModerationManager).GetMethod("Method_Public_Boolean_String_String_String_1"), GetLocalPatch("CanEnterPublicWorldsPatch"), null);
                new Patch("Ethos_Moderation", typeof(ModerationManager).GetMethod("Method_Public_Boolean_String_String_String_0"), GetLocalPatch("IsKickedFromWorldPatch"), null);
                new Patch("Ethos_Moderation", typeof(ModerationManager).GetMethod("Method_Public_Boolean_String_8"), GetLocalPatch("IsBlockedEitherWayPatch"), null);
                new Patch("Ethos_Moderation", typeof(ModerationManager).GetMethod("BlockStateChangeRPC"), GetLocalPatch("AntiBlock"), null);
                new Patch("Ethos_Moderation", typeof(ModerationManager).GetMethod("ForceLogoutRPC"), GetLocalPatch("AntiLogout"), null);
                new Patch("Ethos_Moderation", typeof(ModerationManager).GetMethod("BanPublicOnlyRPC"), GetLocalPatch("AntiPublicBan"), null);
                new Patch("Ethos_Moderation", typeof(ModerationManager).GetMethod("FriendStateChangeRPC"), GetLocalPatch("FriendPatch"), null);
                new Patch("Ethos_Moderation", typeof(ModerationManager).GetMethod("BanRPC"), GetLocalPatch("BanPatch"), null);
                new Patch("Ethos_Moderation", typeof(ModerationManager).GetMethod("MuteChangeRPC"), GetLocalPatch("MutePatch"), null);
                new Patch("Ethos_Moderation", typeof(ModerationManager).GetMethod("ShowUserAvatarChangedRPC"), GetLocalPatch("AvatarShownPatch"), null);
                new Patch("Ethos_Moderation", typeof(ModerationManager).GetMethod("WarnUserRPC"), GetLocalPatch("WarnPatch"), null);
                new Patch("Ethos_Moderation", typeof(ModerationManager).GetMethod("ModForceOffMicRPC"), GetLocalPatch("ModForceOffMicPatch"), null);
                new Patch("Ethos_Moderation", typeof(VRC_TriggerInternal).GetMethod("OnPlayerJoined"), GetLocalPatch("OnPlayerJoin"), null);
                new Patch("Ethos_Moderation", typeof(VRC_TriggerInternal).GetMethod("OnPlayerLeft"), GetLocalPatch("OnPlayerLeave"), null);
                new Patch("Ethos_Extras", typeof(UserInteractMenu).GetMethod("Update"), GetLocalPatch("CloneAvatarPrefix"), null);
                new Patch("Ethos_Extras", AccessTools.Method(typeof(Il2CppSystem.Console), "WriteLine", new Type[] { typeof(string) }), GetLocalPatch("IL2CPPConsoleWriteLine"), null);
                new Patch("Ethos_Extras", typeof(ImageDownloader).GetMethod("DownloadImage"), GetLocalPatch("AntiIpLogImage"), null);
                new Patch("Ethos_Extras", typeof(VRCSDK2.VRC_SyncVideoPlayer).GetMethod("AddURL"), GetLocalPatch("AntiVideoPlayerHijacking"), null);
                new Patch("Ethos_Extras", typeof(VRCSDK2.VRC_SyncVideoPlayer).GetMethod("Play"), GetLocalPatch("VideoPlayerPatch"), null);
                new Patch("Ethos_Extras", typeof(VRC_EventHandler).GetMethod("InternalTriggerEvent"), GetLocalPatch("TriggerEvent"), null);
                //new Patch("Ethos_Other", typeof(ObjectPublicAbstractSealedStPhStObInSeSiObBoStUnique).GetMethod("ObjectPublicAbstractSealedStPhStObInSeSiObBoStUnique.Method_Private_Static_Boolean_Byte_Object_ObjectPublicObByObInByObObUnique_SendOptions_0"), GetLocalPatch("OpRaiseEventPrefix"), null);
            }
            catch(Exception e) { if (GeneralUtils.IsDevBranch) Console.WriteLine(e.ToString()); }
            finally { ConsoleUtil.Info("All Patches have been applied successfully."); }
        }

        public static void ApplyPatches() => RetrievePatches();

        #region Patches

        private static bool OpRaiseEventPrefix(ref byte __0, ref Il2CppSystem.Object __1, ref ObjectPublicObByObInByObObUnique __2, ref SendOptions __3)
        {
            try
            {
                if (__0 == 7)
                    return !GeneralUtils.CustomSerialization;

            }
            catch { }
            return true;
        }


        private static bool TriggerEvent(ref VrcEvent __0, ref VrcBroadcastType __1, ref int __2, ref float __3)
        {
            try
            {
                Player Sender = GeneralWrappers.GetPlayerManager().GetPlayer(__2);
                if (__1 == VrcBroadcastType.Always || __1 == VrcBroadcastType.AlwaysUnbuffered || __1 == VrcBroadcastType.AlwaysBufferOne)
                {
                    if (Sender != null)
                    {
                        if (Sender.GetVRCPlayerApi().playerId != GeneralWrappers.GetPlayerManager().GetCurrentPlayer().GetVRC_Player().GetVRCPlayerApi().playerId)
                        {
                            if (Configuration.GetConfig().AntiWorldTriggers)
                                return false;
                        }
                    }
                }

                if (GeneralUtils.WorldTriggers && (__1 != VrcBroadcastType.Always || __1 != VrcBroadcastType.AlwaysBufferOne || __1 != VrcBroadcastType.AlwaysUnbuffered))
                    __1 = VrcBroadcastType.Always;
                
            }
            catch { }
            return true;
        }

        private static bool SendOperationPatch(ref byte __0, ref Dictionary<byte, Il2CppSystem.Object> __1, ref SendOptions __2)
        {
            try
            {
                if (__0 == 7)
                    return !GeneralUtils.CustomSerialization;

                if (__0 == 202)
                    return !GeneralUtils.Invisible;
            }
            catch { }
            return true;
        }

        private static bool IsBlockedEitherWayPatch(ref bool __result)
        {
            if (Configuration.GetConfig().AntiBlock)
                __result = false;

            return true;
        }

        private static bool IsKickedFromWorldPatch(ref bool __result)
        {
            if (Configuration.GetConfig().AntiKick)
                __result = false;

            return true;
        }

        private static bool VideoPlayerPatch(ref VRCSDK2.VRC_SyncVideoPlayer __instance)
        {
            if (__instance.Videos.Count() > 0)
            {
                if (!GeneralUtils.SuitableVideoURL(__instance.Videos.First().URL))
                    return !Configuration.GetConfig().VideoPlayerSafety;
            }

            return true;
        }

        private static bool EnterPortalPatch(PortalInternal __instance)
        {
            try
            {
                if (Configuration.GetConfig().PortalSafety)
                {
                    GeneralWrappers.AlertV2($"Portal: {__instance.field_Private_ApiWorld_0.name}", $"Instance: {(__instance.field_Private_ApiWorld_0.instanceId == null ? "Random Public Instance" : __instance.field_Private_ApiWorld_0.instanceId)}\nWorld Creator: {__instance.field_Private_ApiWorld_0.authorName}\nWho Dropped: {__instance.GetPlayer().GetAPIUser().displayName}",
                    "Enter", new Action(() =>
                    {
                        Networking.GoToRoom($"{__instance.field_Private_ApiWorld_0.id}{__instance.field_Private_ApiWorld_0.instanceId}");
                    }), "Don't Enter", new Action(() =>
                    {
                        GeneralWrappers.ClosePopup();
                    }));
                    return false;
                }
            }
            catch(Exception) { }
            return true;
        }

        private static bool OnPlayerJoin(ref VRCPlayerApi __0)
        {
            try
            {
                if (__0 != null)
                {
                    if (GeneralUtils.WhitelistedCanHearUsers.Contains(__0.displayName))
                        GeneralUtils.WhitelistedCanHearUsers.Remove(__0.displayName); //just to be sure they dont already exist (i could do this in the playereventshandler but this one is better and is called more)

                    if (!PlayerCache.Contains(__0.displayName))
                    {
                        for (var i = 0; i < GeneralUtils.Modules.Count; i++)
                            GeneralUtils.Modules[i].OnPlayerJoin(__0);

                        PlayerCache.Add(__0.displayName);
                    }
                }
            }
            catch { }
            return true;
        }

        private static bool OnPlayerLeave(ref VRCPlayerApi __0)
        {
            try
            {
                if (__0 != null)
                {
                    if (PlayerCache.Contains(__0.displayName))
                    {
                        for (var i = 0; i < GeneralUtils.Modules.Count; i++)
                            GeneralUtils.Modules[i].OnPlayerLeft(__0);

                        PlayerCache.Remove(__0.displayName);
                    }
                }
            }
            catch { }
            return true;
        }

        private static bool AntiKick(ref string __0, ref string __1, ref string __2, ref string __3, ref Player __4)
        {
            try
            {
                if (GeneralWrappers.GetPlayerManager().GetPlayer(__0) != null)
                {
                    for (var i = 0; i < GeneralUtils.Modules.Count; i++)
                        GeneralUtils.Modules[i].OnPlayerKicked(GeneralWrappers.GetPlayerManager().GetPlayer(__0), __4);
                }

                if (__4.GetAPIUser().id == APIUser.CurrentUser.id)
                    return !Configuration.GetConfig().AntiKick;
            }
            catch { }
            return true;
        }

        private static bool AntiBlock(ref string __0, bool __1, ref Player __2)
        {
            try
            {
                if (GeneralWrappers.GetPlayerManager().GetPlayer(__0) != null)
                {
                    for (var i = 0; i < GeneralUtils.Modules.Count; i++)
                        GeneralUtils.Modules[i].OnPlayerBlocked(GeneralWrappers.GetPlayerManager().GetPlayer(__0), __2, __1);
                }
            }
            catch { }
            return true;
        }

        private static bool CloneAvatarPrefix(ref UserInteractMenu __instance)
        {
            try
            {
                if (GeneralUtils.ForceClone)
                {
                    if (__instance.menuController.activeAvatar.releaseStatus != "private")
                    {
                        bool flag2 = !__instance.menuController.activeUser.allowAvatarCopying;
                        if (flag2)
                        {
                            __instance.cloneAvatarButton.gameObject.SetActive(true);
                            __instance.cloneAvatarButton.interactable = true;
                            __instance.cloneAvatarButtonText.color = new Color(0.8117647f, 0f, 0f, 1f);
                            return false;
                        }
                        else
                        {
                            __instance.cloneAvatarButton.gameObject.SetActive(true);
                            __instance.cloneAvatarButton.interactable = true;
                            __instance.cloneAvatarButtonText.color = new Color(0.470588237f, 0f, 0.8117647f, 1f);
                            return false;
                        }
                    }
                }
            }
            catch { }
            return true;
        }

        private static bool IL2CPPConsoleWriteLine(ref string __0) { return !Configuration.GetConfig().CleanConsole; }

        private static bool AntiIpLogImage(ref string __0)
        {
            try
            {
                if (!__0.Contains("vrchat.cloud"))
                    return !Configuration.GetConfig().PortalSafety;
            }
            catch { }
            return true;
        }

        private static bool AntiVideoPlayerHijacking(ref string __0)
        {
            try 
            { 
                if (Configuration.GetConfig().VideoPlayerSafety && !GeneralUtils.SuitableVideoURL(__0)) 
                    return false; 
            }
            catch { }
            return true;
        }

        private static bool CanEnterPublicWorldsPatch(ref bool __result, ref string __0, ref string __1, ref string __2)
        {
            if (Configuration.GetConfig().AntiPublicBan)
                __result = true;

            return true;
        }

        private static bool AntiLogout(ref string __0, ref Player __1)
        {
            try
            {
                if (GeneralWrappers.GetPlayerManager().GetPlayer(__0) != null)
                {
                    for (var i = 0; i < GeneralUtils.Modules.Count; i++)
                        GeneralUtils.Modules[i].OnPlayerLoggedOut(GeneralWrappers.GetPlayerManager().GetPlayer(__0), __1);
                }

                if (__1.GetAPIUser().id == APIUser.CurrentUser.id)
                    return false;
            }
            catch { }
            return true;
        }

        private static bool AntiPublicBan(ref string __0, ref int __1, ref Player __2)
        {
            try
            {
                if (GeneralWrappers.GetPlayerManager().GetPlayer(__0) != null)
                {
                    for (var i = 0; i < GeneralUtils.Modules.Count; i++)
                        GeneralUtils.Modules[i].OnPlayerPublicBanned(GeneralWrappers.GetPlayerManager().GetPlayer(__0), __2);
                }

                if (__2.GetAPIUser().id == APIUser.CurrentUser.id)
                    return !Configuration.GetConfig().AntiPublicBan;
            }
            catch { }
            return true;
        }

        private static bool BanPatch(ref string __0, ref int __1, ref Player __2)
        {
            try
            {
                if (GeneralWrappers.GetPlayerManager().GetPlayer(__0) != null)
                {
                    for (var i = 0; i < GeneralUtils.Modules.Count; i++)
                        GeneralUtils.Modules[i].OnPlayerBanned(GeneralWrappers.GetPlayerManager().GetPlayer(__0), __2);
                }
            }
            catch { }
            return true;
        }

        private static bool FriendPatch(ref string __0, ref Player __1)
        {
            try
            {
                if (GeneralWrappers.GetPlayerManager().GetPlayer(__0) != null)
                {
                    for (var i = 0; i < GeneralUtils.Modules.Count; i++)
                        GeneralUtils.Modules[i].OnPlayerFriended(GeneralWrappers.GetPlayerManager().GetPlayer(__0), __1);
                }
            }
            catch { }
            return true;
        }

        private static bool MutePatch(ref string __0, bool __1, ref Player __2)
        {
            try
            {
                if (GeneralWrappers.GetPlayerManager().GetPlayer(__0) != null)
                {
                    for (var i = 0; i < GeneralUtils.Modules.Count; i++)
                        GeneralUtils.Modules[i].OnPlayerMuted(GeneralWrappers.GetPlayerManager().GetPlayer(__0), __2, __1);
                }
            }
            catch { }
            return true;
        }

        private static bool AvatarShownPatch(ref string __0, bool __1, ref Player __2)
        {
            try
            {
                if (GeneralWrappers.GetPlayerManager().GetPlayer(__0) != null)
                {
                    for (var i = 0; i < GeneralUtils.Modules.Count; i++)
                        GeneralUtils.Modules[i].OnPlayerShown(GeneralWrappers.GetPlayerManager().GetPlayer(__0), __2, __1);
                }
            }
            catch { }
            return true;
        }

        private static bool WarnPatch(ref string __0, ref string __1, ref Player __2)
        {
            try
            {
                if (GeneralWrappers.GetPlayerManager().GetPlayer(__0) != null)
                {
                    for (var i = 0; i < GeneralUtils.Modules.Count; i++)
                        GeneralUtils.Modules[i].OnPlayerWarned(GeneralWrappers.GetPlayerManager().GetPlayer(__0), __2);
                }
            }
            catch { }
            return true;
        }

        private static bool ModForceOffMicPatch(ref string __0, ref Player __1)
        {
            try
            {
                if (GeneralWrappers.GetPlayerManager().GetPlayer(__0) != null)
                {
                    for (var i = 0; i < GeneralUtils.Modules.Count; i++)
                        GeneralUtils.Modules[i].OnPlayerWarned(GeneralWrappers.GetPlayerManager().GetPlayer(__0), __1);
                }
            }
            catch { }
            return true;
        }
    }
    #endregion
}
