using ExitGames.Client.Photon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnhollowerBaseLib;
using UnityEngine;
using VRC;
using VRC.Core;
using VRC.SDKBase;

namespace EthosClient.Wrappers
{
    public static class PlayerWrappers
    {
        public static VRCPlayer GetCurrentPlayer(this PlayerManager instance) { return VRCPlayer.field_Internal_Static_VRCPlayer_0; }

        public static Player[] GetAllPlayers(this PlayerManager instance) { return instance.prop_ArrayOf_Player_0;  }

        public static APIUser GetAPIUser(this Player player) { return player.field_Private_APIUser_0; }

        public static ApiAvatar GetAPIAvatar(this VRCPlayer player) { return player.prop_ApiAvatar_0; }

        public static Player GetVRC_Player(this VRCPlayer player) { return player.field_Private_Player_0; }

        public static VRCPlayer GetVRCPlayer(this Player player) { return player.field_Internal_VRCPlayer_0; }

        public static VRCPlayerApi GetVRCPlayerApi(this Player player) { return player.prop_VRCPlayerApi_0; }

        public static VRCAvatarManager GetVRCAvatarManager(this Player player) { return player.prop_VRCAvatarManager_0; }

        public static USpeaker GetUSpeaker(this VRCPlayer player) { return player.prop_USpeaker_0; }

        public static PlayerNet GetPlayerNet(this Player player) { return player.prop_PlayerNet_0; }

        public static Player GetPlayer(this PlayerManager instance, string UserID)
        {
            var Players = instance.GetAllPlayers();
            for (int i = 0; i < Players.Length; i++)
            {
                var player = Players[i];
                if (player.GetAPIUser().id == UserID)
                {
                    return player;
                }
            }
            return null;
        }
        public static Player GetPlayer(this PlayerManager instance, int Index)
        {
            var Players = instance.GetAllPlayers();
            for (int i = 0; i < Players.Length; i++)
            {
                var player = Players[i];
                if (player.GetVRCPlayerApi().playerId == Index)
                {
                    return player;
                }
            }
            return null;
        }
        public static Player GetPlayer(this PlayerManager instance, VRCPlayerApi api)
        {
            var Players = instance.GetAllPlayers();
            for (int i = 0; i < Players.Length; i++)
            {
                var player = Players[i];
                if (player.GetVRCPlayerApi().playerId == api.playerId)
                {
                    return player;
                }
            }
            return null;
        }
        public static Player GetSelectedPlayer(this QuickMenu instance)
        {
            var APIUser = instance.prop_APIUser_0;
            var playerManager = GeneralWrappers.GetPlayerManager();
            return playerManager.GetPlayer(APIUser.id);
        }

        public static Player GetPlayerByRayCast(this RaycastHit RayCast)
        {
            var gameObject = RayCast.transform.gameObject;
            return GetPlayer(GeneralWrappers.GetPlayerManager(), VRCPlayerApi.GetPlayerByGameObject(gameObject).playerId);
        }

        public static Player GetPlayer(this PortalInternal internalportal) { return internalportal.prop_Player_0; }

        public static ulong GetSteamID(this VRCPlayer player) { return player.field_Private_UInt64_0; }

        public static ObjectPublicObInBoStHaStBoObInHaUnique GetPhotonPlayer(this Player player) { return player.prop_ObjectPublicObInBoStHaStBoObInHaUnique_0; }

        public static Hashtable GetHashtable(this ObjectPublicObInBoStHaStBoObInHaUnique photonPlayer) { return photonPlayer.prop_Hashtable_0; }
    }
}
