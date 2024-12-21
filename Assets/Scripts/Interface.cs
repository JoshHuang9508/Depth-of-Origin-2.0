using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(AttackerType attackerType, float damage, bool isCrit, Vector2 knockbackForce, float knockbackTime);
}

public interface IInterface
{
    public bool IsActive { get; set; }
    public void Open();
    public void Close();
    public void Toggle();
}
// Make ***able to be an interface
public interface ITalkable
{
    public dialog[] Dialogs { get; set; }
    public void Talk();
}

public interface IInteractable
{
    public bool requireKey { get; set; }
    public void Interact();
}

public interface IPickable
{
    public void Pickup();
}

public interface ITradeable
{
    public void Trade();
}