using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using SandBox;

namespace BalancedTournamentArmor
{
    [HarmonyPatch(typeof(TournamentFightMissionController), "AddRandomClothes")]
    public class BalancedTournamentArmorController
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].operand is MethodInfo method && method == AccessTools.Method(typeof(CharacterObject), "get_RandomBattleEquipment"))
                {
                    codes[i].operand = AccessTools.Method(typeof(BalancedTournamentArmorController), "RandomBattleEquipment");
                    codes[i].opcode = OpCodes.Call;
                    codes[i - 1].opcode = OpCodes.Nop;
                    codes[i - 2].opcode = OpCodes.Nop;
                }
            }
            return codes;
        }
        public static Equipment RandomBattleEquipment() => Settlement.CurrentSettlement.Culture.BasicTroop.RandomBattleEquipment;
    }
}
