using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using MelonLoader;
using Il2Cpp;
using System.Reflection;
using ModSettings;
using UnityEngine;
using Il2CppParadoxNotion.Services;
using Il2CppNodeCanvas.Tasks.Conditions;
using static Il2Cpp.Panel_Log;

namespace ImprovedSignalVoid.Patches.Patches
{
    internal class MissionPatches : MonoBehaviour
    {

        
        [HarmonyPatch(typeof(Il2CppTLD.UI.MiniTopNav), nameof(Il2CppTLD.UI.MiniTopNav.Update))]

        internal class MissionTabDisplay
        {

            public static void Prefix(Il2CppTLD.UI.MiniTopNav __instance)
            {

                if (!Settings.settings.enabledMissionTab)
                {
                    foreach (var element in __instance.m_NavElements)
                    {
                        if (element.name == "SpriteMissions")
                        {
                            __instance.m_NavElements.Remove(element);
                            GameObject child = __instance.gameObject.transform.Find("SpriteMissions").gameObject;
                            child.SetActive(false);
                            return;
                        } 
                    }
                }
            }

        } 

       
        [HarmonyPatch(typeof(Panel_HUD), nameof(Panel_HUD.Update))]
        internal class HUDMissionPopupDisplay
        {
            private static void Postfix(Panel_HUD __instance)
            {
                GameObject EssentialHUD = __instance.gameObject.transform.GetChild(3).gameObject;
                GameObject MissionPopup = EssentialHUD.transform.GetChild(11).gameObject;

                if (!Settings.settings.enabledMissionPopups)
                {
                    MissionPopup.SetActive(false);
                }
            }
        }


        [HarmonyPatch(typeof(Panel_Log), nameof(Panel_Log.EnableFromRadial))]
        internal class MissionTabDeactivateFromJournalOpen
        {
            private static void Prefix(Panel_Log __instance, ref PanelLogState state)
            {

                if (!Settings.settings.enabledMissionTab)
                {
                    state = PanelLogState.DayListStats;
                }
            }

        }

        [HarmonyPatch(typeof(Panel_Log), nameof(Panel_Log.EnableFromObjective))]

        internal class MissionTabDeactivateFromPopup
        {
            private static bool Prefix(Panel_Log __instance)
            {
                return !Settings.settings.enabledMissionTab ? false : true;
            }

            private static void Postfix(Panel_Log __instance)
            {

                if (!Settings.settings.enabledMissionTab)
                {

                    __instance.m_NavigationTabState = NavigationTabState.Journal;
                    __instance.m_Log = GameManager.GetLogComponent();
                    __instance.EnterState(PanelLogState.DayListStats);
                    __instance.Enable(enable: true);
                    __instance.Refresh();
                    __instance.UpdateNavigationButtons();
                    if (__instance.ShouldEnterSectionNav())
                    {
                        __instance.EnterSectionNav();
                    }
                }

            }

        }
    }
}
