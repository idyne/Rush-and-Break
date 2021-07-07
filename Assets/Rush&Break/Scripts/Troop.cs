using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using PathCreation;
using FateGames;
using UnityEngine.UI;
using TMPro;
using MoreMountains.NiceVibrations;

public class Troop : MonoBehaviour
{
    [SerializeField] private int startSize = 1;
    [SerializeField] private int maxNumberOfAgentObjects = 100;
    [SerializeField] private float speed = 8;
    [SerializeField] private float horizontalSpeed = 4;
    [SerializeField] private Transform troopSizeTransform = null;
    [SerializeField] private TextMeshProUGUI troopSizeText = null;
    [SerializeField] private int size = 0;
    [SerializeField] private Agent leader = null;
    [SerializeField] private List<Agent> agents = null;
    private MainLevelManager levelManager = null;
    private Swerve1D swerve;
    private Vector3[] relativePositions = { new Vector3(0, 0, -1), new Vector3(-0.6f, 0, -0.5f), new Vector3(0.6f, 0, -0.5f) };
    private Vector3 anchor = Vector3.zero;
    private bool isStopped = false;
    private bool isFinished = false;
    public Agent Leader { get => leader; }
    public int Size { get => size; }
    public List<Agent> Agents { get => agents; }

    private void Awake()
    {
        agents = new List<Agent>();
        levelManager = (MainLevelManager)LevelManager.Instance;
        swerve = InputManager.CreateSwerve1D(Vector2.right, Screen.width / 1.5f);
        swerve.OnStart = () => { anchor = leader.transform.position; };
    }

    private void Start()
    {
        for (int i = 0; i < startSize; i++)
            PushAgent();
    }

    private void Update()
    {
        if (leader)
        {
            troopSizeTransform.position = Vector3.Lerp(troopSizeTransform.position, leader.transform.position, Time.deltaTime * 20);
        }
        UpdateSizeText();
        if (GameManager.Instance.State == GameManager.GameState.STARTED)
        {
            if (leader && leader.transform.position.z >= levelManager.FinishLineZPosition)
            {
                isFinished = true;
                GameManager.Instance.State = GameManager.GameState.FINISHED;
                CameraFollow.Instance.TakeFinishPosition();
                speed = 24;
            }
        }
        if (GameManager.Instance.State != GameManager.GameState.NOT_STARTED)
        {
            if (size > 1 || GameManager.Instance.State == GameManager.GameState.STARTED)
            {
                CheckInput();
                MoveLeaderForward();
            }
            else if (!isStopped && isFinished)
            {
                isStopped = true;
                troopSizeTransform.gameObject.SetActive(false);
                leader.transform.Rotate(Vector3.up * 180);
                leader.SwitchAnimation(2);
                levelManager.FinishLevel(true);
            }
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            leader.transform.LeanMoveY(3, 0.3f).setEaseOutSine().setLoopPingPong(1);
        }
    }

    private void CheckInput()
    {
        if (swerve.Active)
            MoveLeaderHorizontally(swerve.Rate/*Input.GetAxis("Horizontal")*/);
    }

    private void UpdateSizeText()
    {
        troopSizeText.text = size.ToString();
    }

    private void MoveLeaderHorizontally(float rate)
    {
        if (Mathf.Abs(rate) == 0) return;
        Vector3 desiredPos = leader.transform.position;
        float clampMin = -5.25f;
        float clampMax = 5.25f;
        if (leader.PositionIndex == 1)
            clampMax -= 1f;
        else if (leader.PositionIndex == 2)
            clampMin += 1f;
        desiredPos.x = (anchor + Vector3.right * rate * (clampMax - clampMin)).x;
        desiredPos.x = Mathf.Clamp(desiredPos.x, clampMin, clampMax);
        if (leader.transform.position.x == clampMin || leader.transform.position.x == clampMax)
        {
            swerve.Reset();
            swerve.OnStart();
        }
        leader.transform.position = desiredPos;
    }
    private void MoveLeaderForward()
    {
        leader.transform.position = Vector3.MoveTowards(leader.transform.position, leader.transform.position + Vector3.forward, Time.deltaTime * speed);
    }

    public void EnqueueAgent()
    {
        if (size++ >= maxNumberOfAgentObjects) return;
        Agent previousLeader = leader;
        GameObject agentObject = ObjectPooler.Instance.SpawnFromPool("Agent", previousLeader ? (previousLeader.transform.position - previousLeader.RelativePosition) : Vector3.zero, Quaternion.identity);
        Agent agent = agentObject.GetComponent<Agent>();
        agents.Insert(0, agent);
        SetLeader();
        if (previousLeader)
        {
            previousLeader.IsLeader = false;
            PlaceAgents();
        }
    }

    public void PushAgent(bool place = true)
    {
        if (size++ >= maxNumberOfAgentObjects) return;
        int index = agents.Count;
        GameObject agentObject = ObjectPooler.Instance.SpawnFromPool("Agent", leader ? (agents[agents.Count - 1].transform.position) : Vector3.zero, Quaternion.identity);
        Agent agent = agentObject.GetComponent<Agent>();
        agents.Add(agent);
        if (index == 0)
        {
            agent.PositionIndex = 0;
            SetLeader();
        }
        else if (place)
            PlaceAgents();
    }

    public void PushAgents(int number, bool place = true)
    {
        for (int i = 0; i < number; i++)
            PushAgent(false);
        if (place)
            PlaceAgents();
        AppearGainText(number);
    }

    private void AppearGainText(int number)
    {
        Transform gainTextTransform = ObjectPooler.Instance.SpawnFromPool("Gain Text", troopSizeTransform.position + Vector3.up * 1, Quaternion.identity).transform;
        TextMeshProUGUI text = gainTextTransform.GetComponentInChildren<TextMeshProUGUI>();
        CanvasGroup canvasGroup = gainTextTransform.GetComponentInChildren<CanvasGroup>();
        text.text = (number >= 0 ? "+" : "-") + Mathf.Abs(number).ToString();
        gainTextTransform.parent = troopSizeTransform;
        gainTextTransform.LeanMoveLocalY(gainTextTransform.transform.position.y + 5, 1);
        LeanTween.alphaCanvas(canvasGroup, 0, 1).setEaseInExpo().setOnComplete(() => { gainTextTransform.gameObject.SetActive(false); });
    }

    public void DequeueAgent(bool place = true)
    {
        RemoveAgentAt(0, place);
    }
    public void DequeueAgents(int number)
    {
        number = Mathf.Clamp(number, 0, size);
        if (size - number >= maxNumberOfAgentObjects)
        {
            int newNumber = Mathf.Clamp(number, 0, maxNumberOfAgentObjects / 2);
            for (int i = 0; i < newNumber; i++)
                DequeueAgent(false);
            size = Mathf.Clamp(size - (number - newNumber), 0, size);
        }
        else
        {
            for (int i = 0; i < agents.Count - (size - number); i++)
                DequeueAgent(false);
            size -= number - (agents.Count - (size - number));
        }

        AppearGainText(-number);
        if (size > 0)
            PlaceAgents();
    }
    public void PopAgent(bool place = true)
    {
        RemoveAgentAt(agents.Count - 1, place);
    }
    public void RemoveAgentAt(int index, bool place = true)
    {
        //TODO preconditions (index bound)
        RemoveAgent(agents[index], place);
    }
    public void PopAgents(int number)
    {
        int newNumber = Mathf.Clamp(number, 0, maxNumberOfAgentObjects / 2);
        for (int i = 0; i < newNumber; i++)
            PopAgent(false);
        size = Mathf.Clamp(size - (number - newNumber), 0, size);
        AppearGainText(-number);
    }
    public void RemoveAgent(Agent agent, bool place = true, bool haptic = false)
    {
        size--;
        ObjectPooler.Instance.SpawnFromPool("Agent Death Effect", agent.transform.position, Quaternion.identity);
        agents.Remove(agent);
        agent.gameObject.SetActive(false);
        if (agent.IsLeader)
        {
            if (agents.Count > 0)
            {
                SetLeader();
                if (place)
                    leader.transform.position -= leader.RelativePosition;
                Agent connectedAgent;
                for (int i = 0; i < agent.ConnectedAgents.Count; i++)
                {
                    connectedAgent = agent.ConnectedAgents[i];
                    if (connectedAgent == leader)
                        continue;
                    connectedAgent.Target = leader;
                    connectedAgent.RelativePosition -= leader.RelativePosition;
                    leader.ConnectedAgents.Add(connectedAgent);
                }
            }
            else
                levelManager.FinishLevel(false);
        }
        else
        {
            Agent connectedAgent;
            for (int i = 0; i < agent.ConnectedAgents.Count; i++)
            {
                connectedAgent = agent.ConnectedAgents[i];
                connectedAgent.Target = agent.Target;
                connectedAgent.RelativePosition += agent.RelativePosition;
                agent.Target.ConnectedAgents.Add(connectedAgent);
            }
            agent.Target.ConnectedAgents.Remove(agent);
        }
        if (size + 1 > maxNumberOfAgentObjects)
        {
            GameObject agentObject = ObjectPooler.Instance.SpawnFromPool("Agent", agents[agents.Count - 1].transform.position, Quaternion.identity);
            agent = agentObject.GetComponent<Agent>();
            agent.PositionIndex = (agents[agents.Count - 1].PositionIndex + 1) % 3;
            agent.RelativePosition = relativePositions[agent.PositionIndex];
            int index = agents.Count;
            agents.Add(agent);
            int targetIndex = index - agent.PositionIndex;
            if (agent.PositionIndex == 0) targetIndex -= 3;
            Agent targetAgent = agents[targetIndex];
            agent.Target = targetAgent;
            targetAgent.ConnectedAgents.Add(agent);
        }
        if (place)
            PlaceAgents();
        if (haptic)
        {
            MMVibrationManager.Haptic(HapticTypes.LightImpact);
        }

    }
    private void SetLeader()
    {
        leader = agents[0];
        CameraFollow.Instance.Target = leader.transform;
        leader.IsLeader = true;
    }
    public void RemoveAgentsBetween(int startInclusive, int endExclusive, bool place = true)
    {
        startInclusive = Mathf.Clamp(startInclusive, 0, agents.Count - 1);
        endExclusive = Mathf.Clamp(endExclusive, 1, agents.Count);
        if (startInclusive >= endExclusive)
        {
            Debug.LogError("Start must be less than end.");
            return;
        }
        for (int i = startInclusive; i < endExclusive; i++)
        {
            RemoveAgentAt(i, false);
        }
        if (place)
            PlaceAgents();
    }
    public void PlaceAgents()
    {
        Agent agent, targetAgent;
        for (int i = 0; i < agents.Count; i++)
        {
            targetAgent = agents[i];
            targetAgent.ConnectedAgents.Clear();
        }
        for (int i = 1; i < agents.Count; i++)
        {
            agent = agents[i];
            int targetIndex = (i <= 3 - leader.PositionIndex) ? 0 : ((i + leader.PositionIndex - 1) / 3 * 3 - leader.PositionIndex);
            targetAgent = agents[targetIndex];
            targetAgent.ConnectedAgents.Add(agent);
            if ((i + leader.PositionIndex) % 3 == 0)
                agent.PositionIndex = 0;
            else if ((i + leader.PositionIndex) % 3 == 1)
                agent.PositionIndex = 1;
            else if ((i + leader.PositionIndex) % 3 == 2)
                agent.PositionIndex = 2;
            if (i <= 3 - leader.PositionIndex)
            {
                if (targetAgent.PositionIndex == 0)
                    agent.RelativePosition = relativePositions[agent.PositionIndex];
                else if (targetAgent.PositionIndex == 1)
                {
                    if (agent.PositionIndex == 0)
                        agent.RelativePosition = new Vector3(0.6f, 0, -0.5f);
                    else if (agent.PositionIndex == 1)
                        agent.RelativePosition = new Vector3(0, 0, -1f);
                    else if (agent.PositionIndex == 2)
                        agent.RelativePosition = new Vector3(1.2f, 0, 0f);
                }
                else if (targetAgent.PositionIndex == 2)
                {
                    if (agent.PositionIndex == 0)
                        agent.RelativePosition = new Vector3(-0.6f, 0, -0.5f);
                    else if (agent.PositionIndex == 1)
                        agent.RelativePosition = new Vector3(-1.2f, 0, 0);
                    else if (agent.PositionIndex == 2)
                        agent.RelativePosition = new Vector3(0, 0, -1);
                }
            }
            else
                agent.RelativePosition = relativePositions[agent.PositionIndex];
            agent.Target = targetAgent;
        }
    }
}
