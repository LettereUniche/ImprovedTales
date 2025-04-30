using Il2Cpp;
using MelonLoader;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using HarmonyLib;
using System.Collections;
using Il2CppNodeCanvas.Tasks.Conditions;
using Il2CppParadoxNotion.Services;
using Main;
using SceneManager = UnityEngine.SceneManagement.SceneManager;
using System.Security.AccessControl;

namespace ImprovedSignalVoid.Patches.Patches
{
    internal class ShortwavePatches : MonoBehaviour
    {

        [HarmonyPatch(typeof(PlayerManager), nameof(PlayerManager.Update))]

        internal class ShortwaveFPHActivator
        {
            private static void Postfix(PlayerManager __instance)
            {

                GameObject rig = GameObject.Find("CHARACTER_FPSPlayer/NEW_FPHand_Rig/GAME_DATA/Origin/HipJoint/Chest_Joint/Camera_Weapon_Offset/Shoulder_Joint/Shoulder_Joint_Offset/Right_Shoulder_Joint_Offset/RightClavJoint/RightShoulderJoint/RightElbowJoint/RightWristJoint/RightPalm/right_prop_point");

                if(rig == null)
                {
                    return;
                }

                GameObject shortwaveFPH = rig.transform.GetChild(18).gameObject;

                if (__instance == null || __instance.m_Gear == null) return;

                if (__instance.m_Gear.name.Contains("GEAR_SignalVoid"))
                {
                    if (__instance.m_InspectModeActive)
                    {
                        SetTriggerItem(__instance.m_Gear.name);
                        shortwaveFPH.SetActive(true);
                    }
                }

            }

            private static void SetTriggerItem(string gearItem)
            {

                if (gearItem.Contains("Tale1ChiefNote1")) return;

                if (gearItem.Contains("GEAR_SignalVoid"))
                {
                    GameObject sideTale1 = GameObject.Find("sideTale1");

                    if (sideTale1 != null)
                    {
                        MessageRouter msgRouter = sideTale1.GetComponent<MessageRouter>();

                        if (msgRouter == null)
                        {
                            MelonLogger.Msg("Message Router is null");
                            return;
                        }

                        if (msgRouter.listeners.ContainsKey("OnCustomEvent"))
                        {
                            var list = msgRouter.listeners["OnCustomEvent"];

                            Condition_PlayerHasInventoryItems condition = list[0].Cast<Condition_PlayerHasInventoryItems>();

                            condition.requirementsDict["_std"][0].name = gearItem;
                        }  
                    }
                }
            }

        }

        [HarmonyPatch(typeof(PlayerManager), nameof(PlayerManager.ProcessPickupItemInteraction))]

        internal class ShortwaveFPHDeactivator
        {

            private static void Postfix(PlayerManager __instance)
            {

                string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

                GameObject rig = GameObject.Find("CHARACTER_FPSPlayer/NEW_FPHand_Rig/GAME_DATA/Origin/HipJoint/Chest_Joint/Camera_Weapon_Offset/Shoulder_Joint/Shoulder_Joint_Offset/Right_Shoulder_Joint_Offset/RightClavJoint/RightShoulderJoint/RightElbowJoint/RightWristJoint/RightPalm/right_prop_point");
                GameObject shortwaveFPH = rig.transform.GetChild(18).gameObject;

                if (__instance.m_Gear == null) return;

                if (__instance.m_Gear.name.Contains("GEAR_SignalVoid"))
                {

                    DisableShortwaveInScene();

                    //wait 5 seconds
                    MelonCoroutines.Start(DisableShortwaveFPH(shortwaveFPH));
                }
            }

            private static void DisableShortwaveInScene()
            {

                Scene mainScene = SceneManager.GetSceneByName(GameManager.m_ActiveScene);
                GameObject[] rootObjects = mainScene.GetRootGameObjects();
                foreach (var root in rootObjects)
                {
                    GameObject result = FindInHierarchy(root, "GEAR_HandheldShortwave");
                    if (result != null)
                    {
                        result.SetActive(false);
                    }
                }
            }

            private static GameObject FindInHierarchy(GameObject parent, string targetName)
            {
                if (parent.name == targetName)
                    return parent;

                var parentTransform = parent.transform;
                int childCount = parentTransform.childCount;

                for (int i = 0; i < childCount; i++)
                {
                    var child = parentTransform.GetChild(i).gameObject;
                    var found = FindInHierarchy(child, targetName);
                    if (found != null)
                        return found;
                }

                return null;
            }

            private static IEnumerator DisableShortwaveFPH(GameObject shortwaveFPH)
            {

                if (shortwaveFPH.active)
                {
                    float waitSeconds = 5f;
                    for (float t = 0f; t < waitSeconds; t += Time.deltaTime) yield return null;
                    shortwaveFPH.SetActive(false);
                }
            }

        }

        [HarmonyPatch(typeof(QualitySettingsManager), nameof(QualitySettingsManager.ApplyCurrentQualitySettings))]

        //Disables the vanilla item spawns for the Shortwave Radio if the player already has it in the inventory
        internal class ShortwaveInAirfieldSceneManager
        {
            private static void Postfix()
            {

                string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

                if (currentScene.Contains("MainMenu") || currentScene == "" || currentScene == null)
                {
                    return;
                }

                for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
                {
                    UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);

                    if (scene.name == "AirfieldRegion_SANDBOX")
                    {

                        GameObject[] rootObjects = scene.GetRootGameObjects();
                        GameObject parent = null;

                        foreach (var obj in rootObjects)
                        {

                            if (obj.name == "Design")
                            {
                                parent = obj;
                                break;
                            }
                        }

                        if (parent != null)
                        {

                            GameObject tales = null;
                            GameObject trackables = null;

                            for (int j = 0; j < parent.transform.childCount; j++)
                            {
                                GameObject child = parent.transform.GetChild(j).gameObject;

                                if (child.name == "Tales")
                                {
                                    tales = child;
                                }
                                else if (child.name == "TrackableHiddenCaches")
                                {
                                    trackables = child;
                                }
                            }

                            if (tales != null)
                            {
                                if (UserHasShortwave())
                                {
                                    tales.transform.GetChild(0).gameObject.SetActive(false);
                                }
                            }
                            else
                            {
                                MelonLogger.Msg("Unable to find tales object in scene");
                            }
                        }

                    }
                }
            }
        }

        public static bool UserHasShortwave() => GameManager.GetInventoryComponent().GetBestGearItemWithName("GEAR_HandheldShortwave") != null ? true : false;
      
    }
}
