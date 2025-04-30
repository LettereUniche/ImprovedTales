using ModSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Unity.Burst.CompilerServices;

namespace ImprovedSignalVoid
{

    public enum Active
    {
        Disabled, Enabled
    }

    internal class CustomSettings : JsonModSettings
    {

        [Section("Tale Settings")]

        [Name("Journal Missions")]
        [Description("Enables or disables the Tale missions in the journal page.")]
        [Choice("Disabled", "Enabled")]
        public bool enabledMissionTab = true;

        [Name("Mission Popups")]
        [Description("Enables or disables the mission popups.")]
        [Choice("Disabled", "Enabled")]
        public bool enabledMissionPopups = true;

        
    }

    static class Settings
    {
        internal static CustomSettings settings;
        internal static void OnLoad()
        {
            settings = new CustomSettings();
            settings.AddToModSettings("Improved Tales", MenuType.Both);
        }
    }
}
