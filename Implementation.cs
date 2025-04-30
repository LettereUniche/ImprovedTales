using ImprovedSignalVoid;
using MelonLoader;
using ModSettings;

namespace Main
{
	public sealed class Implementation : MelonMod
	{
		public override void OnInitializeMelon()
		{
			MelonLogger.Msg("Improved Tales is online!");
            Settings.OnLoad();
		}

	}
}
