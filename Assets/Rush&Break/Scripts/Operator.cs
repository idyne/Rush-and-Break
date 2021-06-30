using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FateGames;
using MoreMountains.NiceVibrations;

public abstract class Operator : MonoBehaviour
{
    protected static MainLevelManager levelManager = null;
    [SerializeField] protected Text text = null;
    [SerializeField] private MeshRenderer rend = null;
    private Collider coll = null;
    private bool isDeactivated = false;

    private void Awake()
    {
        if (!levelManager)
            levelManager = (MainLevelManager)LevelManager.Instance;
        coll = GetComponent<Collider>();
        SetText();
    }

    private void Update()
    {
        if (!isDeactivated && levelManager.Troop.Leader.transform.position.z - 2 > transform.position.z)
            Deactivate(false);
    }

    protected abstract void Operate();
    private void Deactivate(bool scale = true)
    {
        isDeactivated = true;
        coll.enabled = false;
        if (scale)
            transform.LeanScale(Vector3.zero, 0.05f).setOnComplete(() => { gameObject.SetActive(false); });
        else
        {
            Color color = rend.material.color;
            float alpha = 0.6f;
            float time = 0.1f;
            color.a = alpha;
            LeanTween.color(gameObject, color, time);
            LeanTween.alphaText(text.rectTransform, alpha, time);
        }
    }

    protected void DoHaptic()
    {
        MMVibrationManager.Haptic(HapticTypes.HeavyImpact);
    }

    private void OnTriggerEnter(Collider other)
    {
        string tag = other.tag;
        if (tag == "Agent")
        {
            Operate();
            Deactivate();
        }
    }

    protected abstract void SetText();
}
