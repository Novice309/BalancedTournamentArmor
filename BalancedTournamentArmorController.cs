using HarmonyLib;
using SandBox.Tournaments.MissionLogics;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;

namespace BalancedTournamentArmor
{
    [HarmonyPatch(typeof(TournamentFightMissionController))]
    public class BalancedTournamentArmorController
    {
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

        // Heal the hero agent to full HP.
        private static void HealHeroAgent(CharacterObject character, Agent agent)
        {
            if (character.IsHero && BalancedTournamentArmorSettings.Instance.ShouldHealHeroAgents)
            {
                agent.Health = character.HeroObject.MaxHitPoints;
            }
        }
    }
}
