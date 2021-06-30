using UnityEngine;
using FateGames;
using System.Collections.Generic;
using FSG.MeshAnimator;

public class Agent : MonoBehaviour, IPooledObject
{

    private static Player player = null;
    private static Troop troop = null;
    private static MainLevelManager levelManager = null;
    public Vector3 RelativePosition = Vector3.zero;
    public Agent Target;
    [SerializeField] private List<Agent> connectedAgents = null;
    public bool IsLeader = false;
    public int PositionIndex = -1;
    [SerializeField] private MeshAnimator meshAnimator = null;
    private static float speed = 8;
    private bool isAnimationSet = false;

    public List<Agent> ConnectedAgents { get => connectedAgents; }

    private void Awake()
    {
        if (!levelManager)
        {
            levelManager = (MainLevelManager)LevelManager.Instance;
            speed = levelManager.AgentSpeed;
        }
        if (!player)
            player = FindObjectOfType<Player>();
        if (!troop)
            troop = FindObjectOfType<Troop>();
        connectedAgents = new List<Agent>();
    }

    private void Update()
    {
        if (!isAnimationSet)
        {
            isAnimationSet = true;
            if (GameManager.Instance.State == GameManager.GameState.NOT_STARTED)
                SwitchAnimation(0);
            else
                SwitchAnimation(1);
            /*else if (GameManager.Instance.State == GameManager.GameState.FINISHED)
                SwitchAnimation(2);*/
        }
        if (!IsLeader)
            Follow();
    }

    private void FixedUpdate()
    {
        if (!IsLeader)
            FollowHorizontally();
    }

    private void FollowHorizontally()
    {
        Vector3 pos = Vector3.Lerp(transform.position, Target.transform.position + RelativePosition, 0.4f);
        pos.z = transform.position.z;
        transform.position = pos;
    }
    private void Follow()
    {
        Vector3 pos = transform.position;
        pos.z = Vector3.MoveTowards(transform.position, Target.transform.position + RelativePosition, speed * Time.deltaTime).z;
        transform.position = pos;
    }

    public void SwitchAnimation(int index)
    {
        meshAnimator.Play(index);
    }

    public void OnObjectSpawn()
    {
        IsLeader = false;
        connectedAgents.Clear();
        isAnimationSet = false;
    }
}
