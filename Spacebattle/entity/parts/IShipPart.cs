namespace Spacebattle.entity
{
    public interface IShipPart
    {
        float GetHealth();
        float GetMaxHealth();
        void Repair(float repairAmount);
        float GetMass();
        float GetUpkeepCost();
    }
}