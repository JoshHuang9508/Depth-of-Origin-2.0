using UnityEngine;

public interface IDamageable
{
    public void Damage(AttackerType attackerType, float damage, bool isCrit, Vector2 knockbackForce, float knockbackTime);
}

public interface IInterface
{
    public bool IsActive { get; set; }
    public void Open();
    public void Close();
    public void Toggle();
}