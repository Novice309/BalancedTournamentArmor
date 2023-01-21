using SandBox.Tournaments.MissionLogics;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace BalancedTournamentArmor
{
    public class BalancedTournamentArmorModel : TournamentModel
    {
        private readonly TournamentModel _model;

        public BalancedTournamentArmorModel(TournamentModel model) => _model = model;

        // Get troop armors of the current settlement's culture.
        // If basic troops are not found, get the armor of noble troops instead.
        public override Equipment GetParticipantArmor(CharacterObject participant)
        {
            Equipment equipment = null;
            if (Mission.Current.HasMissionBehavior<TournamentBehavior>())
            {
                int tier = BalancedTournamentArmorSettings.Instance.TroopTierOfArmor;
                List<CharacterObject> troops = new List<CharacterObject>();
                List<CharacterObject> eliteTroops = new List<CharacterObject>();
                CultureObject culture = Settlement.CurrentSettlement.Culture;
                for (int i = 0; i < tier; i++)
                {
                    troops.Add(i == 0 ? culture.BasicTroop : troops[i - 1]?.UpgradeTargets.GetRandomElementWithPredicate(character => character.IsInfantry));
                    eliteTroops.Add(i == 0 ? culture.EliteBasicTroop : eliteTroops[i - 1]?.UpgradeTargets.GetRandomElement());
                }
                equipment = (troops.Find(troop => troop != null && troop.Tier == tier) ?? eliteTroops.Find(troop => troop != null && troop.Tier == tier))?.RandomBattleEquipment;
                if (equipment == null)
                {
                    InformationManager.DisplayMessage(new InformationMessage("Unable to change armor of tournament participants!"));
                }
            }
            return equipment ?? _model.GetParticipantArmor(participant);
        }

        public override TournamentGame CreateTournament(Town town) => _model.CreateTournament(town);

        public override int GetInfluenceReward(Hero winner, Town town) => _model.GetInfluenceReward(winner, town);

        public override int GetNumLeaderboardVictoriesAtGameStart() => _model.GetNumLeaderboardVictoriesAtGameStart();

        public override int GetRenownReward(Hero winner, Town town) => _model.GetRenownReward(winner, town);

        public override (SkillObject skill, int xp) GetSkillXpGainFromTournament(Town town) => _model.GetSkillXpGainFromTournament(town);

        public override float GetTournamentEndChance(TournamentGame tournament) => _model.GetTournamentEndChance(tournament);

        public override float GetTournamentSimulationScore(CharacterObject character) => _model.GetTournamentSimulationScore(character);

        public override float GetTournamentStartChance(Town town) => _model.GetTournamentStartChance(town);
    }
}
