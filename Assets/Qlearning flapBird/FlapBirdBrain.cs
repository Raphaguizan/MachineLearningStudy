using MLS.NNw;
using MLS.QLearning;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class FlapBirdBrain : MonoBehaviour
{
    [SerializeField]
    private BirdController birdCtrl;
    [SerializeField]
    private LevelManager manager;
    [SerializeField, ShowNativeProperty]
    private int MemoryCount => CountMemory();

    [Space, SerializeField]
    private string pathToSaveWeights = "/weights.txt";
    [SerializeField]
    private string pathToSaveTrainingData = "/data.txt";
    [SerializeField]
    private string pathToSaveMemory = "/memory.txt";
    [SerializeField]
    private int autoSaveTime = 60;

    [Space, SerializeField]
    private bool showDebug = false;
    [Range(1f, 5f), SerializeField]
    private float simulationTime = 5f;

    private bool alreadySaved = false; 

    private Agent agent;

    private int failCount = 0;
    private float timer = 0;
    private float totalTime = 0;
    private float maxRoundTime = 0;

    private Transform lastObstacle = null;


    private bool playerMode = false;
    

    GUIStyle guiStyle = new GUIStyle();
    void OnGUI()
    {
        guiStyle.fontSize = 25;
        guiStyle.normal.textColor = Color.white;
        GUI.BeginGroup(new Rect(10, 10, 600, 150));
        GUI.Box(new Rect(0, 0, 140, 140), "Stats", guiStyle);
        GUI.Label(new Rect(10, 25, 500, 30), "Fails: " + failCount, guiStyle);
        GUI.Label(new Rect(10, 50, 500, 30), "Decay Rate: " + agent.exploreRate, guiStyle);
        GUI.Label(new Rect(10, 75, 500, 30), "Last Best round time: " + maxRoundTime, guiStyle);
        GUI.Label(new Rect(10, 100, 500, 30), "This round time: " + timer, guiStyle);
        GUI.Label(new Rect(10, 125, 500, 30), "Total Time: " + (totalTime + Time.realtimeSinceStartup).ToTime(), guiStyle);
        GUI.EndGroup();
    }

    // Start is called before the first frame update
    void Start()
    {
        agent = new(3, 2, 2, 6, .2f);
        agent.SetActivationFunction(ActivationType.TANH, ActivationType.STEP);

        Debug.Log($"weights {(agent.LoadWeights(pathToSaveWeights)? "": "not")} loaded");
        Debug.Log($"Training data {(LoadData()? "": "not")} loaded");
        
        if(!playerMode)
            Debug.Log($"Memory data {(agent.LoadMemory(pathToSaveMemory)? "": "not")} loaded");

        Time.timeScale = simulationTime;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        AutoSave();

        // controller & debug
        if (Time.timeScale != simulationTime)
            Time.timeScale = simulationTime;

        if (Input.GetKeyDown("space"))
            birdCtrl.ResetBird();

        if(agent.showDebug != showDebug)
            agent.showDebug = showDebug;
        //----------
    }

    private void AutoSave()
    {
        if ((int)(Time.realtimeSinceStartup + totalTime) % autoSaveTime == 0)
        {
            if (!alreadySaved)
            {
                Save();
                alreadySaved = true;
            }
        }
        else
        {
            alreadySaved = false;
        }
    }

    public void ActivePlayerMode(bool val)
    {
        playerMode = val;
    }

    private void FixedUpdate()
    {
        List<double> states = new();

        states.Add(birdCtrl.transform.position.y);
        states.Add(manager.NextObsHeight());
        states.Add(manager.DistFromDeathPoint());

        if (!playerMode)
        {
            int qIndex = agent.GetQIndex(states);
            float IAResp = agent.GetQValue(qIndex);


            if (showDebug)
            {
                Debug.Log($"index = {qIndex}");
                Debug.Log($"value = {IAResp}");
            }

            //movement

            if ((qIndex == 0 && IAResp > .5f) || (qIndex == 1 && IAResp <= .5f))
                birdCtrl.Jump();
        }

        // reward
        if (birdCtrl.Die)
        {
            if (birdCtrl.lastDeathTag.Equals("wall"))
                agent.Reward(states, -5f);
            else
                agent.Reward(states, -1f);
        }
        else if (lastObstacle != null && lastObstacle != manager.nextObstacle)
        {
            states.Clear();
            states.Add(birdCtrl.transform.position.y);
            states.Add(manager.NextObsHeight(false));
            states.Add(manager.DistFromDeathPoint(false));
            agent.Reward(states, 10f);
        }
        else
            agent.Reward(states, .1f);

        lastObstacle = manager.nextObstacle;

        // train and restart
        if (birdCtrl.Die)
        {
            if(!playerMode)
                agent.Train();

            birdCtrl.ResetBird();

            if (timer > maxRoundTime)
            {
                maxRoundTime = timer;
            }

            timer = 0;

            failCount++;
            lastObstacle = null;
        }

    }
    private void Save()
    {
        if (playerMode)
        {
            agent.SaveMemory(pathToSaveMemory);
            Debug.Log("memory saved");
            return;
        }

        agent.SaveWeights(pathToSaveWeights);
        SaveData();
        Debug.Log("saved");
    }

    public void SaveData()
    {
        string p = Application.dataPath + pathToSaveTrainingData;
        StreamWriter wf = File.CreateText(p);
        string data = $"{failCount}|{agent.exploreRate}|{maxRoundTime}|{totalTime + Time.realtimeSinceStartup}";
        wf.WriteLine(data);
        wf.Close();
    }

    public bool LoadData()
    {
        string p = Application.dataPath + pathToSaveTrainingData;
        StreamReader wf;
        try
        {
            wf = File.OpenText(p);
        }
        catch (Exception e)
        {
            return false;
        }

        if (File.Exists(p))
        {
            string line = wf.ReadLine();
            var list = line.Split('|');

            failCount = int.Parse(list[0]);
            agent.exploreRate = float.Parse(list[1]);
            maxRoundTime = float.Parse(list[2]);
            totalTime = float.Parse(list[3]);

            return true;
        }

        return false;
    }

    [ContextMenu("Reset explore rate")]
    public void ResetExploreRate()
    {
        Agent newAg = new(1,1,1,1,1);
        agent.exploreRate = newAg.exploreRate;
        newAg = null;
    }
    private int CountMemory()
    {
        if (agent != null)
            return agent.MemoryCount;
        else return -1;
    }

    private void OnApplicationQuit()
    {
        Save();
    }
}
