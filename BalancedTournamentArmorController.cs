using HarmonyLib;
using SandBox.Tournaments.MissionLogics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;

namespace BalancedTournamentArmor
{
    [HarmonyPatch(typeof(TournamentFightMissionController), "SpawnAgentWithRandomItems")]
    public class BalancedTournamentArmorController
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = instructions.ToList();
            List<CodeInstruction> codesToInsert = new List<CodeInstruction>
            {
                // Get the hero agent to heal to full HP.
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Ldloc_2),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(BalancedTournamentArmorController), "HealHeroAgent", new Type[] { typeof(CharacterObject), typeof(Agent) }))
            };

            codes.InsertRange(codes.Count - 1, codesToInsert);

            return codes;
        }

        private static void HealHeroAgent(CharacterObject character, Agent agent)
        {
            if (character.IsHero && BalancedTournamentArmorSettings.Instance.ShouldHealHeroAgents)
            {
                // Heal the hero agent to full HP.
                agent.Health = character.HeroObject.MaxHitPoints;
            }
        }
    }
}
