using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace BalancedTournamentArmor
{
    public class BalancedTournamentArmorSubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad() => new Harmony("mod.bannerlord.balancedtournamentarmor").PatchAll();
    }
}
