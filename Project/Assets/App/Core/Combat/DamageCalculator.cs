using System;
using App.Core.Models;

namespace App.Core.Combat
{
    public static class DamageCalculator
    {
        public static int Calculate(Stats attacker, Stats defender)
        {
            return Math.Max(1, attacker.Attack - defender.Defense / 2);
        }
    }
}
