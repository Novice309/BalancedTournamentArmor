using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace BalancedTournamentArmor
{
    public class BalancedTournamentArmorModel : DefaultTournamentModel
    {
        // Get troop armors of the current settlement's culture.
        // If basic troops are not found, get the armor of noble troops instead to prevent a crash.
        public override Equipment GetParticipantArmor(CharacterObject participant)
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
            return (troops.Find(troop => troop != null && troop.Tier == tier) ?? eliteTroops.Find(troop => troop != null && troop.Tier == tier)).RandomBattleEquipment;
        }
    }
}
