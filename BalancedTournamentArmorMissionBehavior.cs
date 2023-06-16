using SandBox.Missions.MissionLogics.Arena;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.MountAndBlade;

namespace BalancedTournamentArmor
{
    public class BalancedTournamentArmorMissionBehavior : MissionBehavior
    {
        private bool _canHealHeroAgents;

        public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;

        public override void OnAddTeam(Team team) => _canHealHeroAgents = true;

        public override void OnMissionTick(float dt)
        {
            if (Mission.Current.HasMissionBehavior<ArenaAgentStateDeciderLogic>() && BalancedTournamentArmorSettings.Instance.ShouldHealHeroAgents && _canHealHeroAgents)
            {
                IEnumerable<Agent> agentsToHeal = Mission.Agents.Where(a => a.IsHero && a.Health < a.Character.MaxHitPoints());

                foreach (Agent agent in agentsToHeal)
                {
                    // Heal the hero agent to full HP.
                    agent.Health = agent.Character.MaxHitPoints();
                }

                _canHealHeroAgents = agentsToHeal.Any();
            }
        }
    }
}
