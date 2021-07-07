using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FateGames;
using TMPro;

public class Wall : MonoBehaviour
{
    [HideInInspector] public WallSet WallSet = null;
    [HideInInspector] public int MaxSize = 10;
    [SerializeField] private TextMeshProUGUI sizeText = null;
    [SerializeField] private MeshRenderer rend = null;
    [Header("Bonus")]
    [SerializeField] private bool isBonus = false;
    [SerializeField] private int bonusMaxSize = 5;
    [SerializeField] private float multiplier = 0.5f;
    private int currentSize = 0;
    private static MainLevelManager levelManager = null;
    private bool isDeactivated = false;
    private BoxCollider boxCollider = null;
    public Color Color;


    private void Awake()
    {
        if (!levelManager)
            levelManager = (MainLevelManager)LevelManager.Instance;
        boxCollider = GetComponent<BoxCollider>();
        if (isBonus)
        {
            Material mat = rend.material;
            mat.color = Color;
            rend.material = mat;
        }

    }
    private void Start()
    {
        currentSize = isBonus ? bonusMaxSize : MaxSize;
        UpdateSizeText();
        if (!isBonus)
            SetColor();
    }

    private void UpdateSizeText()
    {
        sizeText.text = currentSize.ToString();
    }

    private void Update()
    {
        if (!isBonus && !isDeactivated && levelManager.Troop.Leader.transform.position.z - 2 > transform.position.z)
            Deactivate();
    }

    public void Deactivate()
    {
        isDeactivated = true;
        boxCollider.enabled = false;
        Color color = rend.material.color;
        float alpha = 0.3f;
        float time = 0.1f;
        color.a = alpha;
        LeanTween.color(gameObject, color, time);
        LeanTween.alphaText(sizeText.rectTransform, alpha, time);
    }

    private void DecrementCurrentSize()
    {
        currentSize--;
        transform.position += Vector3.forward * (2f / (isBonus ? bonusMaxSize : MaxSize));
        UpdateSizeText();
    }

    private void CheckSize()
    {
        if (currentSize <= 0)
        {
            gameObject.SetActive(false);
            PooledEffect pooledEffect = ObjectPooler.Instance.SpawnFromPool(isBonus ? "Bonus Wall Break Effect" : "Wall Break Effect", transform.position, Quaternion.identity).GetComponent<PooledEffect>();
            ParticleSystem.MainModule main = pooledEffect.Effect.main;
            main.startColor = Color;

            if (!isBonus)
                WallSet.Penetrate();
            else
            {
                ObjectPooler.Instance.SpawnFromPool("Confetti Effect", transform.position - Vector3.right * 6, Quaternion.identity);
                ObjectPooler.Instance.SpawnFromPool("Confetti Effect", transform.position + Vector3.right * 6, Quaternion.identity);
                levelManager.Multiplier += multiplier;
            }
        }
    }

    private void SetColor()
    {
        int x = WallSet.maxSize - WallSet.minSize;
        x /= levelManager.WallColors.Length - 1;
        int index = (MaxSize - WallSet.minSize) / x;
        Color = levelManager.WallColors[index];
        Material mat = rend.material;
        mat.color = Color;
        rend.material = mat;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isDeactivated)
        {
            string tag = other.tag;
            if (tag == "Agent")
            {
                Agent agent = other.GetComponent<Agent>();
                Troop troop = levelManager.Troop;
                if (!(GameManager.Instance.State == GameManager.GameState.FINISHED && levelManager.Troop.Size <= 1))
                    troop.RemoveAgent(agent, false, true);
                DecrementCurrentSize();
                CheckSize();
            }
        }
    }
}
