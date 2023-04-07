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
        public override Equipment GetParticipantArmor(CharacterObject participant)
        {
            Equipment equipment = null;
            if (Mission.Current.Mode == MissionMode.Tournament)
            {
                equipment = CharacterObject.FindAll(character => character.Culture == Settlement.CurrentSettlement.Culture && character.Tier == BalancedTournamentArmorSettings.Instance.TroopTier && character.IsSoldier && !character.HiddenInEncylopedia && !character.StringId.Contains("conspiracy") && !character.StringId.Contains("tutorial") && !character.StringId.Contains("canticles")).GetRandomElementInefficiently()?.RandomBattleEquipment;
                if (equipment == null)
                {
                    InformationManager.DisplayMessage(new InformationMessage("Unable to change armor of " + participant.Name + "!"));
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
