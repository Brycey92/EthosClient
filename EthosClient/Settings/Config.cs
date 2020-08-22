using EthosClient.API;
using EthosClient.EthosInput;
using EthosClient.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace EthosClient.Settings
{
    public class Config
    {
        public bool
            CleanConsole = true,
            Optimization = false,
            LogModerations = false,
            AntiPublicBan = true,
            AntiKick = true,
            AntiPhotonBot = true,
            NoUSpeakExploits = true,
            VideoPlayerSafety = true,
            AntiBlock = true,
            PortalSafety = true,
            AntiWorldTriggers = true,
            UseRichPresence = true,
            MenuRGB = false,
            DefaultLogToConsole = true;

        public string ClientVersion = "2.5";

        public List<FavoritedAvatar> ExtendedFavoritedAvatars = new List<FavoritedAvatar>();

        public List<EthosVRButton> Buttons = new List<EthosVRButton>();

        public List<EthosKeybind> Keybinds = new List<EthosKeybind>();
    }
}
