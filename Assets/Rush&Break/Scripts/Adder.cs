
using UnityEngine;


public class Adder : Operator
{

    [SerializeField] private int number = 1;

    protected override void Operate()
    {
        Troop troop = levelManager.Troop;
        troop.PushAgents(number);
        DoHaptic();
        //ObjectPooler.Instance.SpawnFromPool("Operator Effect", transform.position, Quaternion.identity);
    }

    protected override void SetText()
    {
        text.text = "+" + number;
    }

}
