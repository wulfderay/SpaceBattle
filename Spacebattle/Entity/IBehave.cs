using Spacebattle.Behaviours;
using System;

namespace Spacebattle.entity
{
    public interface IBehave
    {
        void AddBehaviour(IBehaviour behaviour);
        void RemoveBehaviour(IBehaviour behaviour);
        void ExecuteBehaviours();
    }
}