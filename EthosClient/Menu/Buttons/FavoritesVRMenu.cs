using BestHTTP;
using EthosClient.API;
using EthosClient.Menu;
using EthosClient.Settings;
using EthosClient.Wrappers;
using RubyButtonAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VRC;
using VRC.Core;
using VRC.SDKBase;
using VRC.UI;
using VRCSDK2;

namespace EthosClient.Utils
{
    public class FavoritesVRMenu : QMNestedButton
    {
        private bool DeleteMode = false;
        private int X = 1;
        private int Y = 0;
        public static PageAvatar PAviSaved = new PageAvatar() { avatar = new SimpleAvatarPedestal() };
        public FavoritesVRMenu(QMNestedButton parent, EthosVRButton config) : base(parent, config.X, config.Y, config.Name, config.Tooltip, GeneralUtils.GetColor(config.ColorScheme.Colors[0]), GeneralUtils.GetColor(config.ColorScheme.Colors[1]), GeneralUtils.GetColor(config.ColorScheme.Colors[2]), GeneralUtils.GetColor(config.ColorScheme.Colors[3]))
        {
            new QMSingleButton(this, 0, 0, "Next", delegate
            {
                //to-do
            }, "Go to the next page", Color.red, Color.white);

            new QMSingleButton(this, 0, 1, "Back", delegate
            {
               //to-do
            }, "Go back to the previous page", Color.red, Color.white);

            new QMSingleButton(this, 5, 0, "Add\nCurrent Avatar", delegate
            {
                var currentAvatar = GeneralWrappers.GetPlayerManager().GetCurrentPlayer().GetAPIAvatar();
                Configuration.GetConfig().ExtendedFavoritedAvatars.Add(new FavoritedAvatar(currentAvatar.name, currentAvatar.id, currentAvatar.authorName, currentAvatar.authorId));
                Configuration.SaveConfiguration();
                GeneralWrappers.GetVRCUiPopupManager().AlertPopup("<color=cyan>Success!</color>", "<color=green>Successfully added your current Avatar to extended favorites</color>");
            }, "Adds your current avatar to the extended favorites list", Color.red, Color.white);

            new QMSingleButton(this, 5, 1, "Remove\nCurrent Avatar", delegate
            {
                var currentAvatar = GeneralUtils.GetExtendedFavorite(GeneralWrappers.GetPlayerManager().GetCurrentPlayer().GetAPIAvatar().id);
                Configuration.GetConfig().ExtendedFavoritedAvatars.Remove(currentAvatar);
                Configuration.SaveConfiguration();
                GeneralWrappers.GetVRCUiPopupManager().AlertPopup("<color=cyan>Success!</color>", "<color=green>Successfully removed your current Avatar from extended favorites</color>");
            }, "Removes your current avatar from the extended favorites list", Color.red, Color.white);

            new QMToggleButton(this, 5, -1, "Delete\nMode", delegate
            {
                DeleteMode = true;
            }, "Normal\nMode", delegate
            {
                DeleteMode = false;
            }, "Enable/Disable Delete Mode, with this on, you can remove avatars from this list by just clicking their respective buttons", Color.red, Color.white);

            foreach(var avatar in Configuration.GetConfig().ExtendedFavoritedAvatars)
            {
                if (X == 4)
                {
                    if (Y != 2)
                    {
                        new QMSingleButton(this, X, Y, avatar.Name, delegate
                        {
                            if (DeleteMode)
                            {
                                Configuration.GetConfig().ExtendedFavoritedAvatars.Remove(avatar);
                                Configuration.SaveConfiguration();
                                GeneralWrappers.GetVRCUiPopupManager().AlertPopup("<color=cyan>Success!</color>", "<color=green>Successfully removed this Avatar from extended favorites</color>");
                            }
                            else
                            {
                                new ApiAvatar() { id = avatar.ID }.Get(new Action<ApiContainer>(x =>
                                {
                                    PAviSaved.avatar.field_Internal_ApiAvatar_0 = x.Model.Cast<ApiAvatar>(); // can fix better later.
                                    PAviSaved.ChangeToSelectedAvatar();
                                }), null, null, false);
                            }
                        }, $"by {avatar.Author}\nSwitch into this avatar.", Color.red, Color.white);
                        Y++;
                    }
                }
                else
                {
                    new QMSingleButton(this, X, Y, avatar.Name, delegate
                    {
                        if (DeleteMode)
                        {
                            Configuration.GetConfig().ExtendedFavoritedAvatars.Remove(avatar);
                            Configuration.SaveConfiguration();
                            GeneralWrappers.GetVRCUiPopupManager().AlertPopup("<color=cyan>Success!</color>", "<color=green>Successfully removed this Avatar from extended favorites</color>");
                        }
                        else
                        {
                            new ApiAvatar() { id = avatar.ID }.Get(new Action<ApiContainer>(x =>
                            {
                                PAviSaved.avatar.field_Internal_ApiAvatar_0 = x.Model.Cast<ApiAvatar>(); // can fix better later.
                                PAviSaved.ChangeToSelectedAvatar();
                            }), null, null, false);
                        }
                    }, $"by {avatar.Author}\nSwitch into this avatar.", Color.red, Color.white);
                    X++;
                }
            }
        }
    }
}
