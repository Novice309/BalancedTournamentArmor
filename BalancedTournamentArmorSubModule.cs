using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace BalancedTournamentArmor
{
    // This mod makes tournament participants wear the same armor.
    public class BalancedTournamentArmorSubModule : MBSubModuleBase
    {
        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            if (game.GameType is Campaign)
            {
                gameStarterObject.AddModel(new BalancedTournamentArmorModel((TournamentModel)gameStarterObject.Models.Last(model => model is TournamentModel)));
            }
        }

        public override void OnBeforeMissionBehaviorInitialize(Mission mission) => mission.AddMissionBehavior(new BalancedTournamentArmorMissionBehavior());
    }
}
