
using UnityEngine;


public class Divider : Operator
{

    [SerializeField] private int divider = 1;

    protected override void Operate()
    {
        Troop troop = levelManager.Troop;
        int number = Mathf.CeilToInt(troop.Size * (1 - (1f / divider)));
        troop.DequeueAgents(number);
        DoHaptic();
    }

    protected override void SetText()
    {
        text.text = "÷" + divider;
    }

}
