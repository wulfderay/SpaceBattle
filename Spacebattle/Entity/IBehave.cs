using Spacebattle.Behaviours;
using Spacebattle.orders;

namespace Spacebattle.entity
{
    public interface IBehave
    {
        void AddBehaviour(IBehaviour behaviour);
        void RemoveBehaviour(IBehaviour behaviour); // don't really like this.
        Order ExecuteBehaviours();
    }
}