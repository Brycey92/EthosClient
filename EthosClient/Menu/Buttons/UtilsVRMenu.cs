using BestHTTP;
using EthosClient.Menu;
using EthosClient.Settings;
using EthosClient.Wrappers;
using RubyButtonAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRC;
using VRC.Core;
using VRC.SDKBase;
using VRC.UI;
using VRCSDK2;

namespace EthosClient.Utils
{
    public class UtilsVRMenu : QMNestedButton
    {
        public UtilsVRMenu(QMNestedButton parent, EthosVRButton config) : base(parent, config.X, config.Y, config.Name, config.Tooltip, GeneralUtils.GetColor(config.ColorScheme.Colors[0]), GeneralUtils.GetColor(config.ColorScheme.Colors[1]), GeneralUtils.GetColor(config.ColorScheme.Colors[2]), GeneralUtils.GetColor(config.ColorScheme.Colors[3]))
        {
            new QMToggleButton(this, 1, 0, "Enable\nESP", delegate
            {
                GeneralUtils.ESP = true;
                GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i].transform.Find("SelectRegion"))
                    {
                        array[i].transform.Find("SelectRegion").GetComponent<Renderer>().material.color = Color.green;
                        array[i].transform.Find("SelectRegion").GetComponent<Renderer>().sharedMaterial.color = Color.red;
                        GeneralWrappers.GetHighlightsFX().EnableOutline(array[i].transform.Find("SelectRegion").GetComponent<Renderer>(), GeneralUtils.ESP);
                    }
                }

                foreach (VRCSDK2.VRC_Interactable vrc_Interactable in Resources.FindObjectsOfTypeAll<VRCSDK2.VRC_Interactable>())
                    GeneralWrappers.GetHighlightsFX().EnableOutline(vrc_Interactable.GetComponentInChildren<Renderer>(), GeneralUtils.ESP);

                foreach (VRCSDK2.VRC_Pickup vrc_Pickup in Resources.FindObjectsOfTypeAll<VRCSDK2.VRC_Pickup>())
                    GeneralWrappers.GetHighlightsFX().EnableOutline(vrc_Pickup.GetComponentInChildren<Renderer>(), GeneralUtils.ESP);

                foreach (PortalInternal portalInternal in Resources.FindObjectsOfTypeAll<PortalInternal>())
                    GeneralWrappers.GetHighlightsFX().EnableOutline(portalInternal.GetComponentInChildren<Renderer>(), GeneralUtils.ESP);


            }, "Disable\nESP", delegate
            {
                GeneralUtils.ESP = false;
                GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i].transform.Find("SelectRegion"))
                    {
                        array[i].transform.Find("SelectRegion").GetComponent<Renderer>().material.color = Color.green;
                        array[i].transform.Find("SelectRegion").GetComponent<Renderer>().sharedMaterial.color = Color.red;
                        GeneralWrappers.GetHighlightsFX().EnableOutline(array[i].transform.Find("SelectRegion").GetComponent<Renderer>(), GeneralUtils.ESP);
                    }
                }

                foreach (VRCSDK2.VRC_Interactable vrc_Interactable in Resources.FindObjectsOfTypeAll<VRCSDK2.VRC_Interactable>())
                    GeneralWrappers.GetHighlightsFX().EnableOutline(vrc_Interactable.GetComponentInChildren<Renderer>(), GeneralUtils.ESP);

                foreach (VRCSDK2.VRC_Pickup vrc_Pickup in Resources.FindObjectsOfTypeAll<VRCSDK2.VRC_Pickup>())
                    GeneralWrappers.GetHighlightsFX().EnableOutline(vrc_Pickup.GetComponentInChildren<Renderer>(), GeneralUtils.ESP);

                foreach (PortalInternal portalInternal in Resources.FindObjectsOfTypeAll<PortalInternal>())
                    GeneralWrappers.GetHighlightsFX().EnableOutline(portalInternal.GetComponentInChildren<Renderer>(), GeneralUtils.ESP);

            }, "Decide whether you want the upper game, get an advantage, and see all players anywhere within the world.", Color.red, Color.white).setToggleState(GeneralUtils.ESP);

            new QMSingleButton(this, 2, 0, "Avatar\nBy\nID", delegate
            {
                ConsoleUtil.Info("Enter Avatar ID: ");
                string ID = Console.ReadLine();
                PageAvatar PAviSaved = new PageAvatar() { avatar = new SimpleAvatarPedestal() };
                new ApiAvatar() { id = ID }.Get(new Action<ApiContainer>(x =>
                {
                    PAviSaved.avatar.field_Internal_ApiAvatar_0 = x.Model.Cast<ApiAvatar>();
                    PAviSaved.ChangeToSelectedAvatar();
                }), null, null, false);
                GeneralWrappers.GetVRCUiPopupManager().AlertPopup("<color=cyan>Success!</color>", "<color=green>Successfully cloned that avatar by It's Avatar ID.</color>");
            }, "Sets your current avatar using an avatar ID.", Color.red, Color.white);

            new QMSingleButton(this, 3, 0, "Join\nBy\nID", delegate
            {
                ConsoleUtil.Info("Enter Instance ID: ");
                string ID = Console.ReadLine();
                Networking.GoToRoom(ID);
            }, "Joins an instance by It's ID.", Color.red, Color.white);

            new QMSingleButton(this, 4, 0, "Copy\nRoom\nID", delegate
            {
                Clipboard.SetText($"{RoomManagerBase.field_Internal_Static_ApiWorld_0.id}:{RoomManagerBase.field_Internal_Static_ApiWorld_0.instanceId}");
            }, "Copies the current instance's ID.", Color.red, Color.white);

            new QMToggleButton(this, 1, 1, "Can't Hear\non Non Friends", delegate
            {
                GeneralUtils.CantHearOnNonFriends = true;
                foreach (var player in GeneralWrappers.GetPlayerManager().GetAllPlayers())
                {
                    if (!APIUser.IsFriendsWith(player.GetAPIUser().id))
                    {
                        player.GetVRCPlayer().field_Internal_Boolean_3 = false;
                    }
                }
            }, "Can Hear\non Non Friends", delegate
            {
                GeneralUtils.CantHearOnNonFriends = false;
                foreach (var player in GeneralWrappers.GetPlayerManager().GetAllPlayers())
                {
                    if (!APIUser.IsFriendsWith(player.GetAPIUser().id))
                    {
                        player.GetVRCPlayer().field_Internal_Boolean_3 = true;
                    }
                }
            }, "Decide whether you want your friends to only hear you in game or not.", Color.red, Color.white).setToggleState(GeneralUtils.CantHearOnNonFriends);

            //new QMToggleButton(this, 2, 1, "Invisible\nOn", delegate
            //{
            //    GeneralUtils.Invisible = true;
            //}, "Invisible\nOff", delegate
            //{
            //    GeneralUtils.Invisible = false;
            //}, "Decide whether you want people to see you in game.", Color.red, Color.white).setToggleState(GeneralUtils.Invisible);
        }
    }
}
