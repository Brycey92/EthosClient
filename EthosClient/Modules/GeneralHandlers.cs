using EthosClient.EthosInput;
using EthosClient.Settings;
using EthosClient.Utils;
using EthosClient.Wrappers;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VRC;
using VRC.Core;
using VRC.SDKBase;

namespace EthosClient.Modules
{
    public class GeneralHandlers : VRCMod
    {
        public override string Name => "General Handlers";

        public override string Description => "Handlers for flight, input, etc";

        public override string Author => "404#0004 and Yaekith#1337";

        public override void OnStart() { }

        private List<EthosKeybind> StoredKeybinds = new List<EthosKeybind>();

        public override void OnUpdate()
        {
            try
            {
                if (StoredKeybinds.Count() == 0) 
                    StoredKeybinds.AddRange(Configuration.GetConfig().Keybinds);

                foreach (var keybind in StoredKeybinds)
                {
                    if (Input.GetKey(keybind.FirstKey) && Input.GetKeyDown(keybind.SecondKey))
                    {
                        switch (keybind.Target)
                        {
                            default:
                                break;
                            case EthosFeature.Flight:
                                GeneralUtils.Flight = !GeneralUtils.Flight;
                                Physics.gravity = GeneralUtils.Flight ? Vector3.zero : GeneralUtils.SavedGravity;
                                GeneralUtils.ToggleColliders(!GeneralUtils.Flight);
                                break;
                            case EthosFeature.ESP:
                                GeneralUtils.ESP = !GeneralUtils.ESP;
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
                                break;
                            case EthosFeature.Autism:
                                GeneralUtils.Autism = !GeneralUtils.Autism;
                                break;
                            case EthosFeature.SpinBot:
                                GeneralUtils.SpinBot = !GeneralUtils.SpinBot;
                                break;
                            case EthosFeature.WorldTriggers:
                                GeneralUtils.WorldTriggers = !GeneralUtils.WorldTriggers;
                                break;
                            case EthosFeature.ToggleAllTriggers:
                                foreach (VRC_Trigger trigger in Resources.FindObjectsOfTypeAll<VRC_Trigger>())
                                    if (!trigger.name.Contains("Avatar") && !trigger.name.Contains("Chair"))
                                        trigger.Interact();
                                break;
                            case EthosFeature.AntiWorldTriggers:
                                Configuration.GetConfig().AntiWorldTriggers = !Configuration.GetConfig().AntiWorldTriggers;
                                Configuration.SaveConfiguration();
                                break;
                        }
                    }
                }

                if (GeneralUtils.AutoDeleteEveryonesPortals)
                {
                    if (Resources.FindObjectsOfTypeAll<PortalInternal>().Count() > 0)
                    {
                        foreach (var portal in Resources.FindObjectsOfTypeAll<PortalInternal>())
                            UnityEngine.Object.Destroy(portal.gameObject);
                    }
                }

                if (GeneralUtils.AutoDeleteNonFriendsPortals)
                {
                    if (Resources.FindObjectsOfTypeAll<PortalInternal>().Count() > 0)
                    {
                        foreach (var portal in Resources.FindObjectsOfTypeAll<PortalInternal>())
                        {
                            var player = portal.GetPlayer();
                            if (player.GetAPIUser() != null)
                            {
                                if (!APIUser.IsFriendsWith(player.GetAPIUser().id))
                                    UnityEngine.Object.Destroy(portal.gameObject);
                            }
                        }
                    }
                }

                if (GeneralUtils.AutoDeleteAllPickups)
                {
                    if (Resources.FindObjectsOfTypeAll<VRC_Pickup>().Count() > 0)
                    {
                        foreach (var pickup in Resources.FindObjectsOfTypeAll<VRC_Pickup>())
                            UnityEngine.Object.Destroy(pickup.gameObject);
                    }
                }

                if (GeneralUtils.SpinBot)
                    PlayerWrappers.GetVRC_Player(GeneralWrappers.GetPlayerManager().GetCurrentPlayer()).gameObject.transform.Rotate(0f, 20f, 0f);

                if (GeneralUtils.InfiniteJump)
                {
                    if (VRCInputManager.Method_Public_Static_ObjectPublicStSiBoSiObBoSiObStSiUnique_String_0("Jump").prop_Boolean_0)
                        GeneralWrappers.GetPlayerManager().GetCurrentPlayer().GetComponent<VRCMotionState>().field_Private_Boolean_0 = true;
                }

                if (GeneralUtils.Autism)
                {
                    var randomPlr = PlayerWrappers.GetAllPlayers(GeneralWrappers.GetPlayerManager())[new System.Random().Next(0, PlayerWrappers.GetAllPlayers(GeneralWrappers.GetPlayerManager()).Count())];
                    PlayerWrappers.GetVRC_Player(GeneralWrappers.GetPlayerManager().GetCurrentPlayer()).gameObject.transform.position = randomPlr.transform.position;
                }

                if (GeneralUtils.Flight)
                {
                    GameObject gameObject = GeneralWrappers.GetPlayerCamera();
                    var currentSpeed = (Input.GetKey(KeyCode.LeftShift) ? 16f : 8f);
                    var player = GeneralWrappers.GetPlayerManager().GetCurrentPlayer();

                    if (Input.GetKey(KeyCode.W))  
                        player.transform.position += gameObject.transform.forward * currentSpeed * Time.deltaTime;

                    if (Input.GetKey(KeyCode.A)) 
                        player.transform.position += player.transform.right * -1f * currentSpeed * Time.deltaTime;

                    if (Input.GetKey(KeyCode.S)) 
                        player.transform.position += gameObject.transform.forward * -1f * currentSpeed * Time.deltaTime;

                    if (Input.GetKey(KeyCode.D)) 
                        player.transform.position += player.transform.right * currentSpeed * Time.deltaTime;

                    if (Input.GetKey(KeyCode.Space)) 
                        player.transform.position += player.transform.up * currentSpeed * Time.deltaTime;

                    if (Math.Abs(Input.GetAxis("Joy1 Axis 2")) > 0f) 
                        player.transform.position += gameObject.transform.forward * currentSpeed * Time.deltaTime * (Input.GetAxis("Joy1 Axis 2") * -1f);

                    if (Math.Abs(Input.GetAxis("Joy1 Axis 1")) > 0f) 
                        player.transform.position += gameObject.transform.right * currentSpeed * Time.deltaTime * Input.GetAxis("Joy1 Axis 1");
                }
            }
            catch (Exception) { }
        }
    }
}
