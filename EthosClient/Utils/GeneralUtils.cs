using EthosClient.API;
using EthosClient.Menu;
using EthosClient.Modules;
using EthosClient.Settings;
using EthosClient.Wrappers;
using Il2CppSystem.Threading;
using MelonLoader.ICSharpCode.SharpZipLib.Zip;
using RubyButtonAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRCSDK2;

namespace EthosClient.Utils
{
    public static class GeneralUtils
    {
        public static bool
            WorldTriggers = false,
            Flight = false,
            Autism = false,
            ESP = false,
            SpinBot = false,
            ForceClone = false,
            IsDevBranch = false,
            InfiniteJump = false,
            SpeedHax = false,
            CustomSerialization = false,
            AutoDeleteNonFriendsPortals = false,
            CantHearOnNonFriends = false,
            AutoDeleteEveryonesPortals,
            AutoDeleteAllPickups = false,
            VoiceMod = false,
            DestroyUSpeakOnPlayerJoin = false,
            Invisible = false;

        public static string 
            Version = "2.5";

        public static float
            WalkSpeed = 2f,
            RunSpeed = 4f,
            StrafeSpeed = 2f,
            MicrophoneVolume = 1f;

        public static List<string> 
            WhitelistedCanDropPortals = new List<string>(), 
            WhitelistedCanHearUsers = new List<string>();

        public static Vector3 SavedGravity = Physics.gravity;

        public static List<VRCMod> Modules = new List<VRCMod>();

        public static Dictionary<string, string> Authorities = new Dictionary<string, string>();

        public static void InformHudText(Color color, string text)
        {
            if (!Configuration.GetConfig().DefaultLogToConsole)
            {
                var NormalColor = VRCUiManager.prop_VRCUiManager_0.hudMessageText.color;
                VRCUiManager.prop_VRCUiManager_0.hudMessageText.color = color;
                VRCUiManager.prop_VRCUiManager_0.Method_Public_Void_String_0($"[ETHOS] {text}");
                VRCUiManager.prop_VRCUiManager_0.hudMessageText.color = NormalColor;
            }
            else ConsoleUtil.WriteToConsole(ConsoleColor.Yellow, $"[ETHOS] {text}");
        }

        public static void ToggleColliders(bool toggle)
        {
            Collider[] array = UnityEngine.Object.FindObjectsOfType<Collider>();
            Component component = GeneralWrappers.GetPlayerManager().GetCurrentPlayer().GetComponents<Collider>().FirstOrDefault<Component>();

            for (int i = 0; i < array.Length; i++)
            {
                Collider collider = array[i];
                if (collider.GetComponent<PlayerSelector>() != null || collider.GetComponent<VRC.SDKBase.VRC_Pickup>() != null || collider.GetComponent<QuickMenu>() != null || collider.GetComponent<VRC_Station>() != null || collider.GetComponent<VRC.SDKBase.VRC_AvatarPedestal>() != null && collider != component) 
                    collider.enabled = toggle;
            }
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[new System.Random().Next(s.Length)]).ToArray());
        }

        public static FavoritedAvatar GetExtendedFavorite(string ID)
        {
            foreach(var avatar in Configuration.GetConfig().ExtendedFavoritedAvatars) 
                if (avatar.ID == ID) 
                    return avatar;

            return null;
        }

        public static bool SuitableVideoURL(string url)
        {
            if (url.Contains("youtube.com")) 
                return true;
            else if (url.Contains("youtu.be")) 
                return true;
            else if (url.Contains("twitch.tv")) 
                return true;

            return false;
        }

        public static EthosVRButton GetEthosVRButton(string ID)
        {
            foreach(var button in Configuration.GetConfig().Buttons)
            {
                if (button.ID == ID)
                {
                    return button;
                }
            }
            return null;
        }

        public static EthosColor GetEthosColor(Color color) { return new EthosColor(color.r, color.g, color.b); }

        public static Color GetColor(EthosColor color) { return new Color(color.R, color.G, color.B); }

        public static GameObject DropPortal(string ID, int playerCount, UnityEngine.Vector3 pos, UnityEngine.Quaternion rotation)
        {
            GameObject portal = Networking.Instantiate(VRC.SDKBase.VRC_EventHandler.VrcBroadcastType.Always, "Portals/PortalInternalDynamic", pos, rotation);
            Networking.RPC(VRC.SDKBase.RPC.Destination.AllBufferOne, portal, "ConfigurePortal", new Il2CppSystem.Object[]
            {
                (Il2CppSystem.String)ID.Split(':')[0],
                (Il2CppSystem.String)ID.Split(':')[1],
                new Il2CppSystem.Int32
                {
                    m_value = playerCount
                }.BoxIl2CppObject()
            });
            return portal;
        }
    }
}
