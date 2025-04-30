using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Il2Cpp;
using ImprovedSignalVoid;
using ImprovedSignalVoid.Patches.Patches;
using MelonLoader;
using UnityEngine;

namespace ImprovedTales.Patches
{
    internal class ShortwaveSpawnPatches : MonoBehaviour
    {

        [HarmonyPatch(typeof(GearItem), nameof(GearItem.Awake))]

        //Despawns the radio and narrative item spawns if the user already has the radio
        public class DespawnRadios
        {
            public static void Postfix(GearItem __instance)
            {

                if (GameManager.m_ActiveScene.ToLowerInvariant().Contains("menu") || __instance.gameObject.scene.name == "" && !__instance.gameObject.scene.IsValid()) return;

                if (__instance.name.Contains("GEAR_HandheldShortwave") || __instance.name.ToLowerInvariant().Contains("signalvoid") && !__instance.m_InPlayerInventory)
                {

                    if (ShortwavePatches.UserHasShortwave())
                    {
                        __instance.gameObject.SetActive(false);
                        return;
                    }
                    else
                    { //this means the shortwave is still in the environment
                        if (__instance.name.Contains("GEAR_HandheldShortwave"))
                        {
                            BoxCollider bc = __instance.GetComponent<BoxCollider>();
                            if (bc != null) Destroy(bc);
                        }
                    }
                } 
            }
        }
    }
}
