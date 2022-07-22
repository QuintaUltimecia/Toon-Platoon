using UnityEngine;

public class Shotgun : Gun
{
    [SerializeField] private int _pelletCount = 5;

    public override void AttackLogic()
    {
        for (int i = 0; i < _pelletCount; i++)
        {
            base.AttackLogic();
        }
    }
}
