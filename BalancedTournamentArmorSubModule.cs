using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace BalancedTournamentArmor
{
    // This mod makes tournament participants wear the same basic armor, as well as healing participating heroes to full HP.
    public class BalancedTournamentArmorSubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad() => new Harmony("mod.bannerlord.balancedtournamentarmor").PatchAll();
    }
}
