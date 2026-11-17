using UnityEngine;

// INHERITANCE + POLYMORPHISM
// ConeZombie mewarisi Enemy, lalu meng-override Serang().
public class ConeZombie : Enemy
{
    public bool cone = true;

    public override void Serang()
    {
        Debug.Log("Cone Gigit");
        base.Serang();   // beri damage ke player
    }
}