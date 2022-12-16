using HarmonyLib;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace BalancedTournamentArmor
{
    // This mod makes tournament participants wear the same basic armor, as well as healing participating heroes to full HP.
    public class BalancedTournamentArmorSubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad() => new Harmony("mod.bannerlord.balancedtournamentarmor").PatchAll();

        protected override void OnGameStart(Game game, IGameStarter gameStarter)
        {
            if (game.GameType is Campaign)
            {
                CampaignGameStarter campaignStarter = (CampaignGameStarter)gameStarter;
                campaignStarter.AddModel(new BalancedTournamentArmorModel((TournamentModel)campaignStarter.Models.ToList().FindLast(model => model is TournamentModel)));
            }
        }
    }
}
