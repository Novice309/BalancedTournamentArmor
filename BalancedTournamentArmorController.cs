using HarmonyLib;
using SandBox;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

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
        // Replace the vanilla armor worn by agents.
        private static IEnumerable<CodeInstruction> Transpiler2(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Stloc_0)
                {
                    codes[i - 1].opcode = OpCodes.Call;
                    codes[i - 1].operand = AccessTools.Method(typeof(BalancedTournamentArmorController), "RandomBattleEquipment", new Type[] { typeof(CharacterObject) });
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
        // Get the armor of the agent's formation group with the current settlement's culture.
        public static Equipment RandomBattleEquipment(CharacterObject character) => CharacterObject.FindAll(c => c.Culture == Settlement.CurrentSettlement.Culture && c.IsSoldier && c.DefaultFormationGroup == character.DefaultFormationGroup && c.Tier == _settings.TroopTierOfArmor).GetRandomElementInefficiently().RandomBattleEquipment;
        private static BalancedTournamentArmorSettings _settings;
    }
}
