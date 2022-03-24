using HarmonyLib;
using SandBox.Tournaments.MissionLogics;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;

namespace BalancedTournamentArmor
{
    [HarmonyPatch(typeof(TournamentFightMissionController))]
    public class BalancedTournamentArmorController
    {
        private static bool _isDeReMilitariLoaded;

        // Get troop armors of the current settlement's culture.
        // If the setting is above tier 3 and De Re Militari is loaded, get noble troop armors instead to prevent a crash.
        public static Equipment RandomBattleEquipment
        {
            get
            {
                BalancedTournamentArmorSettings settings = BalancedTournamentArmorSettings.Instance;
                CharacterObject[] characters = new CharacterObject[settings.TroopTierOfArmor];
                for (int i = 0; i < characters.Length; i++)
                {
                    if (i <= 2 || (i > 2 && !_isDeReMilitariLoaded))
                    {
                        characters[i] = i == 0 ? Settlement.CurrentSettlement.Culture.BasicTroop : characters[i - 1].UpgradeTargets.GetRandomElementWithPredicate(character => character.IsInfantry);
                    }
                    else
                    {
                        characters[i] = i == 3 ? Settlement.CurrentSettlement.Culture.EliteBasicTroop : characters[i - 1].UpgradeTargets.GetRandomElement();
                    }
                }
                return characters[settings.TroopTierOfArmor - 1].RandomBattleEquipment;
            }
        }

        // Check whether De Re Militari is loaded.
        [HarmonyPatch(MethodType.Constructor, new Type[] { typeof(CultureObject) })]
        public static void Postfix()
        {
            foreach (string moduleName in Utilities.GetModulesNames())
            {
                if (moduleName == "DeReMilitari")
                {
                    _isDeReMilitariLoaded = true;
                }
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
