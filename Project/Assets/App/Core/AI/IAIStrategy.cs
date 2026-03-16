using App.Core.Models;
using App.Core.Turn;

namespace App.Core.AI
{
    public interface IAIStrategy
    {
        IAction DecideAction(Enemy enemy, GameState state);
    }
}
