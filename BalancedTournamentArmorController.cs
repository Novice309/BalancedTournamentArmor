using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using SandBox;

namespace BalancedTournamentArmor
{
    [HarmonyPatch(typeof(TournamentFightMissionController))]
    public class BalancedTournamentArmorController
    {
        [HarmonyPatch(MethodType.Constructor, new Type[] { typeof(CultureObject) })]
        public static void Postfix() => _settings = BalancedTournamentArmorSettings.Instance;
        [HarmonyTranspiler]
        [HarmonyPatch("SpawnAgentWithRandomItems")]
        // Get the hero agent to heal to full HP.
        private static IEnumerable<CodeInstruction> Transpiler1(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            List<CodeInstruction> codesToInsert = new List<CodeInstruction>();
            codesToInsert.Add(new CodeInstruction(OpCodes.Ldloc_0));
            codesToInsert.Add(new CodeInstruction(OpCodes.Ldloc_2));
            codesToInsert.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(BalancedTournamentArmorController), "HealHeroAgent", new Type[] { typeof(CharacterObject), typeof(Agent) })));
            codes.InsertRange(codes.Count - 1, codesToInsert);
            return codes;
        }
        [HarmonyTranspiler]
        [HarmonyPatch("AddRandomClothes")]
        // Replace the vanilla armor worn by agents with either basic troop armor or basic noble troop armor.
        private static IEnumerable<CodeInstruction> Transpiler2(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].operand is MethodInfo method && method == AccessTools.Method(typeof(CharacterObject), "get_RandomBattleEquipment"))
                {
                    codes[i].operand = AccessTools.Method(typeof(BalancedTournamentArmorController), "get_RandomBattleEquipment");
                    codes[i].opcode = OpCodes.Call;
                    codes[i - 1].opcode = OpCodes.Nop;
                    codes[i - 2].opcode = OpCodes.Nop;
                }
            }
            return codes;
        }
        // Heal the hero agent to full HP.
        public static void HealHeroAgent(CharacterObject character, Agent agent)
        {
            if (character.IsHero && _settings.ShouldHealHeroAgents)
            {
                agent.Health = character.HeroObject.MaxHitPoints;
            }
        }
        // Get either the basic troop armor or basic noble troop armor of the current settlement's culture.
        public static Equipment RandomBattleEquipment => !_settings.ShouldWearNobleArmor ? Settlement.CurrentSettlement.Culture.BasicTroop.RandomBattleEquipment : Settlement.CurrentSettlement.Culture.EliteBasicTroop.RandomBattleEquipment;
        private static BalancedTournamentArmorSettings _settings;
    }
}
