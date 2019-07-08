using Spacebattle.entity;
using Spacebattle.orders;

namespace Spacebattle.Entity
{
    public interface IControllableEntity: IDamageableEntity, IUpdateable
    {
        void AllStop();
        float GetMaxAcceleration(); // is this really needed?
       
        void SetCourse(float angle);
        void SetThrottle(float percent);
        float GetThrottle();

        void DoOrder(Order order);
        
    }
}
