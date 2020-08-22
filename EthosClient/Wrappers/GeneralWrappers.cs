using BestHTTP.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using VRC;
using VRC.Core;

namespace EthosClient.Wrappers
{
    public static class GeneralWrappers
    {
        public static PlayerManager GetPlayerManager() { return PlayerManager.field_Private_Static_PlayerManager_0; }

        public static QuickMenu GetQuickMenu() { return QuickMenu.prop_QuickMenu_0; }

        public static VRCUiManager GetVRCUiPageManager() { return VRCUiManager.field_Protected_Static_VRCUiManager_0; }

        public static UserInteractMenu GetUserInteractMenu() { return Resources.FindObjectsOfTypeAll<UserInteractMenu>()[0]; }

        public static GameObject GetPlayerCamera() { return GameObject.Find("Camera (eye)"); }

        public static VRCVrCamera GetVRCVrCamera() { return VRCVrCamera.field_Private_Static_VRCVrCamera_0; }

        public static string GetRoomId() { return APIUser.CurrentUser.location; }

        public static VRCUiManager GetVRCUiManager() { return VRCUiManager.prop_VRCUiManager_0; }

        public static HighlightsFX GetHighlightsFX() { return HighlightsFX.prop_HighlightsFX_0; }

        public static void EnableOutline(this HighlightsFX instance, Renderer renderer, bool state) => instance.Method_Public_Void_Renderer_Boolean_0(renderer, state); //First method to take renderer, bool parameters

        public static VRCUiPopupManager GetVRCUiPopupManager() { return VRCUiPopupManager.prop_VRCUiPopupManager_0; }

        public static void AlertPopup(this VRCUiPopupManager manager, string title, string text) => manager.Method_Public_Void_String_String_Single_0(title, text, 10f);

        public static void SelectPlayer(this QuickMenu instance, VRCPlayer player) => instance.Method_Public_Void_VRCPlayer_0(player);

        public static void AlertV2(string title, string Content, string buttonname, Action action, string button2, Action action2) => VRCUiPopupManager.field_Private_Static_VRCUiPopupManager_0.Method_Public_Void_String_String_String_Action_String_Action_Action_1_VRCUiPopup_0(title, Content, buttonname, action, button2, action2, null);

        public static bool IsInVr() { return XRDevice.isPresent; }

        public static void ShowInputKeyBoard(Il2CppSystem.Action<string, Il2CppSystem.Collections.Generic.List<KeyCode>, Text> InputAction)
        {
            VRCUiPopupManager vrcpopup = GetVRCUiPopupManager();
            vrcpopup.inputPopup.gameObject.SetActive(true);
            vrcpopup.inputPopup.Method_Public_Void_String_InputType_String_Action_3_String_List_1_KeyCode_Text_Boolean_0("Enter Input", InputField.InputType.Standard, "Enter text", InputAction, true);
            GameObject.Find("UserInterface/MenuContent/Popups/InputKeypadPopup").SetActive(true);
        }

        public static void ClosePopup()
        {
            try
            {
                VRCUiPopupManager vrcpopup = GetVRCUiPopupManager();
                vrcpopup.Method_Private_Void_0();
            }
            catch { }
        }
    }
}
