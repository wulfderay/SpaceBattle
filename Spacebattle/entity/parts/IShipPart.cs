namespace Spacebattle.entity
{
    public interface IShipPart: IDamageable, IFlavourTextProvider
    {
        float GetHealth();
        float GetMaxHealth();
        void Repair(float repairAmount);
        float GetMass();
        float GetUpkeepCost();
        Ship Parent { get; set; }
    }
}