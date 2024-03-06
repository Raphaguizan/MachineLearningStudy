using MLS.QLearning;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlapBirdBrain : MonoBehaviour
{
    [SerializeField]
    private BirdController birdCtrl;
    [SerializeField]
    private LevelManager manager;

    [Space, SerializeField]
    private bool showDebug = false;
    [Range(1f, 5f), SerializeField]
    private float simulationTime = 5f;


    private Agent agent;

    private int failCount;
    private float timer;
    private float maxBalanceTime;

    GUIStyle guiStyle = new GUIStyle();
    void OnGUI()
    {
        guiStyle.fontSize = 25;
        guiStyle.normal.textColor = Color.white;
        GUI.BeginGroup(new Rect(10, 10, 600, 150));
        GUI.Box(new Rect(0, 0, 140, 140), "Stats", guiStyle);
        GUI.Label(new Rect(10, 25, 500, 30), "Fails: " + failCount, guiStyle);
        GUI.Label(new Rect(10, 50, 500, 30), "Decay Rate: " + agent.ExploreRate, guiStyle);
        GUI.Label(new Rect(10, 75, 500, 30), "Last Best Balance: " + maxBalanceTime, guiStyle);
        GUI.Label(new Rect(10, 100, 500, 30), "This Balance: " + timer, guiStyle);
        GUI.Label(new Rect(10, 125, 500, 30), "Total Time: " + Time.realtimeSinceStartup, guiStyle);
        GUI.EndGroup();
    }

    // Start is called before the first frame update
    void Start()
    {
        agent = new(3, 2, 1, 6, .3f);

        Time.timeScale = simulationTime;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (Input.GetKeyDown("space"))
            birdCtrl.ResetBird();

        if (Time.timeScale != simulationTime)
            Time.timeScale = simulationTime;
    }

    private void FixedUpdate()
    {
        List<double> states = new();

        Vector3 nextObstaclePos = manager.NextObPos();

        states.Add(birdCtrl.Velocity.y);
        states.Add(nextObstaclePos.y - birdCtrl.transform.position.y);
        states.Add(nextObstaclePos.x = birdCtrl.transform.position.x);

        int qIndex = agent.GetQIndex(states);
        float IAResp = agent.GetQValue(qIndex);


        if (showDebug)
        {
            Debug.Log($"index = {qIndex}");
            Debug.Log($"value = {IAResp}");
        }


        if (qIndex == 0)
            birdCtrl.Jump();



        if (birdCtrl.Die)
            agent.Reward(-1f);
        else
            agent.Reward(.1f);

        if (birdCtrl.Die)
        {
            agent.Train();
            birdCtrl.ResetBird();

            if (timer > maxBalanceTime)
            {
                maxBalanceTime = timer;
            }

            timer = 0;

            failCount++;
        }

    }
}
