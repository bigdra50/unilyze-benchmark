using System;

namespace App.Core.Models
{
    public abstract class Entity
    {
        public Guid Id { get; }
        public string Name { get; }
        public Position Position { get; set; }
        public Stats Stats { get; }

        protected Entity(string name, Position position, Stats stats)
        {
            Id = Guid.NewGuid();
            Name = name;
            Position = position;
            Stats = stats;
        }
    }
}
