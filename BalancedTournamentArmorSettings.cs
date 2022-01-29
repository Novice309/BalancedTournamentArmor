using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Settings.Base.Global;

namespace BalancedTournamentArmor
{
    public class BalancedTournamentArmorSettings : AttributeGlobalSettings<BalancedTournamentArmorSettings>
    {
        public override string Id => "BalancedTournamentArmor";
        public override string DisplayName => "Balanced Tournament Armor";
        public override string FolderName => "BalancedTournamentArmor";
        public override string FormatType => "json2";
        [SettingPropertyBool("Heal Heroes to Full HP", Order = 0, RequireRestart = false, HintText = "Heal nobles, companions and the player to full HP. HP will revert after the tournament. Enabled by default.")]
        [SettingPropertyGroup("HP", GroupOrder = 0)]
        public bool ShouldHealHeroAgents { get; set; } = true;
        [SettingPropertyInteger("Troop Tier of Armor", 2, 6, "0", Order = 0, RequireRestart = false, HintText = "Troop tier of the armor worn by participants. Default is 2.")]
        [SettingPropertyGroup("Armor", GroupOrder = 1)]
        public int TroopTierOfArmor { get; set; } = 2;
    }
}
