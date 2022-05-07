using HarmonyLib;
using SandBox.Tournaments.MissionLogics;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace BalancedTournamentArmor
{
    [HarmonyPatch(typeof(TournamentFightMissionController))]
    public class BalancedTournamentArmorController
    {
        // Get troop armors of the current settlement's culture.
        // If basic troops are not found, get the armor of noble troops instead to prevent a crash.
        public static Equipment RandomBattleEquipment
        {
            get
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

        // Get the hero agent to heal to full HP.
        [HarmonyTranspiler]
        [HarmonyPatch("SpawnAgentWithRandomItems")]
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

        // Replace the vanilla armor worn by agents.
        [HarmonyTranspiler]
        [HarmonyPatch("AddRandomClothes")]
        private static IEnumerable<CodeInstruction> Transpiler2(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Stloc_0)
                {
                    codes[i - 3].opcode = OpCodes.Nop;
                    codes[i - 2].opcode = OpCodes.Nop;
                    codes[i - 1].opcode = OpCodes.Call;
                    codes[i - 1].operand = AccessTools.Method(typeof(BalancedTournamentArmorController), "get_RandomBattleEquipment");
                }
            }
            return codes;
        }

        // Heal the hero agent to full HP.
        private static void HealHeroAgent(CharacterObject character, Agent agent)
        {
            if (character.IsHero && BalancedTournamentArmorSettings.Instance.ShouldHealHeroAgents)
            {
                agent.Health = character.HeroObject.MaxHitPoints;
            }
        }

        [HarmonyPatch]
        public class BalancedTournamentArmorTeamController
        {
            private static MethodBase TargetMethod() => AccessTools.Method(AccessTools.TypeByName("TeamTournamentMissionController"), "AddRandomClothes");

            // Check whether Arena Overhaul is loaded.
            private static bool Prepare() => TargetMethod() != null;

            // Replace the vanilla armor worn by agents in Arena Overhaul's team tournaments.
            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
                for (int i = 0; i < codes.Count; i++)
                {
                    if (codes[i].opcode == OpCodes.Stloc_0)
                    {
                        codes[i - 3].opcode = OpCodes.Nop;
                        codes[i - 2].opcode = OpCodes.Nop;
                        codes[i - 1].opcode = OpCodes.Call;
                        codes[i - 1].operand = AccessTools.Method(typeof(BalancedTournamentArmorController), "get_RandomBattleEquipment");
                    }
                }
                return codes;
            }
        }
    }
}
