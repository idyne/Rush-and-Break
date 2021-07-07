using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;
using UnityEngine.UI;
using TMPro;
using MoreMountains.NiceVibrations;

public class MainLevelManager : LevelManager
{
    [SerializeField] private int numberOfRoadParts = 10;
    [SerializeField] private float agentSpeed = 8;
    [SerializeField] private LayerMask wallLayerMask = 0;
    [SerializeField] private LayerMask agentLayerMask = 0;
    [SerializeField] private GameObject wallPrefab = null;
    [SerializeField] private Color[] wallColors = new Color[5];
    [SerializeField] private Slider progressSlider = null;
    [SerializeField] private TextMeshProUGUI coinText = null;
    private Text fromLevelText, toLevelText;
    private Troop troop = null;
    private Camera mainCamera;
    private Road road = null;
    public int Coin = 0;
    private float finishLineZPosition = 0;
    public float Multiplier = 1.0f;

    public float AgentSpeed { get => agentSpeed; }
    public Troop Troop { get => troop; }
    public LayerMask WallLayerMask { get => wallLayerMask; }
    public GameObject WallPrefab { get => wallPrefab; }
    public LayerMask AgentLayerMask { get => agentLayerMask; }
    public Color[] WallColors { get => wallColors; }
    public float FinishLineZPosition { get => finishLineZPosition; }

    private new void Awake()
    {
        base.Awake();
        troop = FindObjectOfType<Troop>();
        mainCamera = Camera.main;
        road = FindObjectOfType<Road>();
        road.CreateRoad(numberOfRoadParts);
        finishLineZPosition = numberOfRoadParts * 11 + road.transform.position.z + 36;
        coinText.text = GameManager.COIN.ToString();
        fromLevelText = GameObject.Find("From").GetComponentInChildren<Text>();
        toLevelText = GameObject.Find("To").GetComponentInChildren<Text>();
        fromLevelText.text = GameManager.Instance.CurrentLevel.ToString();
        toLevelText.text = (GameManager.Instance.CurrentLevel + 1).ToString();
        

    }
    private void Start()
    {
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        for (int i = 0; i < canvases.Length; i++)
            if (canvases[i].renderMode == RenderMode.WorldSpace)
                canvases[i].worldCamera = mainCamera;
    }

    private void Update()
    {
        if (troop.Leader)
        {
            progressSlider.value = Mathf.Clamp(troop.Leader.transform.position.z / (finishLineZPosition - (36)), 0, 1);
        }
    }

    public override void StartLevel()
    {
        for (int i = 0; i < troop.Agents.Count; i++)
        {
            troop.Agents[i].SwitchAnimation(1);
        }
    }
    public override void FinishLevel(bool success)
    {
        /*for (int i = 0; i < troop.Agents.Count; i++)
        {
            troop.Agents[i].SwitchAnimation(2);
        }*/
        MMVibrationManager.Haptic(success ? HapticTypes.Success : HapticTypes.Failure);
        GameManager.Instance.State = GameManager.GameState.FINISHED;
        Coin = (int)(Coin * Multiplier);
        if (success)
            GameManager.COIN += Coin;
        GameManager.Instance.FinishLevel(success);
    }
    public bool IsTargetVisible(GameObject go)
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
        var point = go.transform.position;
        foreach (var plane in planes)
        {
            if (plane.GetDistanceToPoint(point) < 0)
                return false;
        }
        return true;
    }

}
