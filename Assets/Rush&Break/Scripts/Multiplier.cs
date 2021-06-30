
using UnityEngine;


public class Multiplier : Operator
{

    [SerializeField] private int multiplier = 1;

    protected override void Operate()
    {
        Troop troop = levelManager.Troop;
        int number = troop.Size * (multiplier - 1);
        troop.PushAgents(number);
        DoHaptic();
    }

    protected override void SetText()
    {
        text.text = "x" + multiplier;
    }

}
