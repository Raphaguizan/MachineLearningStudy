using MLS.QLearning;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdBrain : MonoBehaviour
{
    public GameObject topObj;
    public GameObject bottomObj;

    [Space, SerializeField]
    private float forceTilt = 10f;

    [Space, SerializeField]
    private bool showDebug = false;
    [Range(1f, 5f), SerializeField]
    private float simulationTime = 5f;

    private bool hasCollided = false;

    private Agent agent;
    private Vector2 startPos;
    private Rigidbody2D rb;

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject == topObj || collision.gameObject == bottomObj)
            hasCollided = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
        agent = new(3, 2, 1, 6, .3f);

        Time.timeScale = simulationTime;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (Input.GetKeyDown("space"))
            ResetBird();

        if (Time.timeScale != simulationTime)
            Time.timeScale = simulationTime;
    }

    private void FixedUpdate()
    {
        List<double> states = new();

        states.Add(rb.velocity.y);
        states.Add(Vector2.Distance(transform.position, topObj.transform.position));
        states.Add(Vector2.Distance(transform.position, bottomObj.transform.position));

        int qIndex = agent.GetQIndex(states);
        float IAResp = agent.GetQValue(qIndex);


        if (showDebug)
        {
            Debug.Log($"index = {qIndex}");
            Debug.Log($"value = {IAResp}");
        }


        if(qIndex == 0)
            rb.AddForce(forceTilt * IAResp * Vector2.up, ForceMode2D.Impulse);



        if (hasCollided)
            agent.Reward(-1f);
        else
            agent.Reward(.1f);

        if (hasCollided)
        {
            agent.Train();
            ResetBird();

            if (timer > maxBalanceTime)
            {
                maxBalanceTime = timer;
            }

            timer = 0;

            failCount++;
            hasCollided = false;
        }

    }

    private void ResetBird()
    {
        transform.position = startPos;
        rb.velocity = Vector2.zero;
    } 
}
