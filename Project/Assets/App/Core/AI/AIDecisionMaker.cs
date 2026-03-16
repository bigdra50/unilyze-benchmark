using System;
using App.Core.Models;
using App.Core.Turn;

namespace App.Core.AI
{
    public class AIDecisionMaker
    {
        private readonly ChaseStrategy _chase = new();
        private readonly PatrolStrategy _patrol = new();
        private readonly FleeStrategy _flee = new();

        public IAction DecideAction(Enemy enemy, GameState state)
        {
            var hpRatio = (double)enemy.Stats.CurrentHP / enemy.Stats.MaxHP;
            var distance = Math.Abs(enemy.Position.X - state.Player.Position.X)
                         + Math.Abs(enemy.Position.Y - state.Player.Position.Y);

            if (hpRatio > 0.5)
            {
                return distance <= 8
                    ? _chase.DecideAction(enemy, state)
                    : _patrol.DecideAction(enemy, state);
            }

            if (hpRatio <= 0.3)
            {
                return _flee.DecideAction(enemy, state);
            }

            // HP 30-50%: attack if adjacent, otherwise flee
            if (distance == 1)
                return new AttackAction(enemy, state.Player);

            return _flee.DecideAction(enemy, state);
        }
    }
}
