using Harmony;
using System.Reflection;

namespace CustomNotes.HarmonyPatches
{
    /// <summary>
    /// Apply and remove all of our Harmony patches through this class
    /// </summary>
    public class CustomNotesPatches
    {
        private static HarmonyInstance instance;

        public static bool IsPatched { get; private set; }
        public const string InstanceId = "com.legoandmars.beatsaber.customnotes";

        internal static void ApplyHarmonyPatches()
        {
            if (!IsPatched)
            {
                if (instance == null)
                {
                    instance = HarmonyInstance.Create(InstanceId);
                }

                instance.PatchAll(Assembly.GetExecutingAssembly());
                IsPatched = true;
            }
        }

        internal static void RemoveHarmonyPatches()
        {
            if (instance != null && IsPatched)
            {
                instance.UnpatchAll(InstanceId);
                IsPatched = false;
            }
        }
    }
}
