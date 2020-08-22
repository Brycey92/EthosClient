using EthosClient.Utils;
using EthosClient.Wrappers;
using RubyButtonAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using VRC.Core;
using VRC.Networking;
using VRC.SDKBase;
using VRC.Udon;
using VRCSDK2;

namespace EthosClient.Menu
{
    public class TargetVRMenu : QMNestedButton
    {
        public TargetVRMenu(EthosVRButton config) : base(config.Menu, config.X, config.Y, config.Name, config.Tooltip, GeneralUtils.GetColor(config.ColorScheme.Colors[0]), GeneralUtils.GetColor(config.ColorScheme.Colors[1]), GeneralUtils.GetColor(config.ColorScheme.Colors[2]), GeneralUtils.GetColor(config.ColorScheme.Colors[3]))
        {
            new QMSingleButton(this, 1, 0, "Teleport", new Action(() =>
            {
                GeneralWrappers.GetPlayerManager().GetCurrentPlayer().transform.position = PlayerWrappers.GetSelectedPlayer(GeneralWrappers.GetQuickMenu()).transform.position;
            }), "Teleports you to the selected player.", Color.red, Color.white);

            new QMToggleButton(this, 2, 0, "Local\nBlock", delegate
            {
                PlayerWrappers.GetSelectedPlayer(GeneralWrappers.GetQuickMenu()).GetVRCPlayer().GetUSpeaker().gameObject.SetActive(false);
                PlayerWrappers.GetSelectedPlayer(GeneralWrappers.GetQuickMenu()).GetVRCAvatarManager().gameObject.SetActive(false);
                PlayerWrappers.GetSelectedPlayer(GeneralWrappers.GetQuickMenu()).GetVRCPlayer().prop_Boolean_0 = false; //disables the nameplate (always the first public boolean)
                PlayerWrappers.GetSelectedPlayer(GeneralWrappers.GetQuickMenu()).GetVRCPlayer().nameTag_old.gameObject.SetActive(false);
                PlayerWrappers.GetSelectedPlayer(GeneralWrappers.GetQuickMenu()).GetVRCPlayer().namePlate.gameObject.SetActive(false);
            }, "Local\nUnblock", delegate
            {
                PlayerWrappers.GetSelectedPlayer(GeneralWrappers.GetQuickMenu()).GetVRCPlayer().GetUSpeaker().gameObject.SetActive(true);
                PlayerWrappers.GetSelectedPlayer(GeneralWrappers.GetQuickMenu()).GetVRCPlayer().prop_Boolean_0 = true; //enables the nameplate (always the first public boolean)
                PlayerWrappers.GetSelectedPlayer(GeneralWrappers.GetQuickMenu()).GetVRCAvatarManager().gameObject.SetActive(true);
            }, "Decide whether you want to block this user locally, meaning, the blocking doesn't effect them but it also makes them disappear to yourself.", Color.red, Color.white);

            new QMToggleButton(this, 3, 0, "Can't\nHear", delegate
            {
                PlayerWrappers.GetSelectedPlayer(GeneralWrappers.GetQuickMenu()).GetVRCPlayer().field_Internal_Boolean_3 = false;
            }, "Can\nHear", delegate
            {
                //canHear is always the second last internal boolean in the VRCPlayer class
                //canSpeak is 1 before it LOL
                PlayerWrappers.GetSelectedPlayer(GeneralWrappers.GetQuickMenu()).GetVRCPlayer().field_Internal_Boolean_3 = true;
            }, "Decide whether you want this user to be able to hear you or not", Color.red, Color.white);

            new QMToggleButton(this, 4, 0, "Can\nHear Whitelist", delegate
            {
                if (GeneralUtils.CantHearOnNonFriends && !GeneralUtils.WhitelistedCanHearUsers.Contains(PlayerWrappers.GetSelectedPlayer(GeneralWrappers.GetQuickMenu()).GetAPIUser().displayName))
                {
                    GeneralUtils.WhitelistedCanHearUsers.Add(PlayerWrappers.GetSelectedPlayer(GeneralWrappers.GetQuickMenu()).GetAPIUser().displayName);
                    PlayerWrappers.GetSelectedPlayer(GeneralWrappers.GetQuickMenu()).GetVRCPlayer().field_Internal_Boolean_3 = true;
                }
                //this is because you can't get the user id of a person who has left through their vrcplayerapi
            }, "Can't\nHear Blacklist", delegate
            {
                if (GeneralUtils.CantHearOnNonFriends && GeneralUtils.WhitelistedCanHearUsers.Contains(PlayerWrappers.GetSelectedPlayer(GeneralWrappers.GetQuickMenu()).GetAPIUser().displayName))
                {
                    GeneralUtils.WhitelistedCanHearUsers.Remove(PlayerWrappers.GetSelectedPlayer(GeneralWrappers.GetQuickMenu()).GetAPIUser().displayName);
                    PlayerWrappers.GetSelectedPlayer(GeneralWrappers.GetQuickMenu()).GetVRCPlayer().field_Internal_Boolean_3 = false;
                }
                //this is because you can't get the user id of a person who has left through their vrcplayerapi
            }, "This is for when you enable can't hear on everyone but friends, but you also want to whitelist/blacklist this user from being able to hear you aswell. When Can't Hear on Non friends is disabled, this won't do anything when toggled.", Color.red, Color.white);
        }
    }
}
