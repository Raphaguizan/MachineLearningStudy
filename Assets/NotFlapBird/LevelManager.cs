using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private bool spawn = true;

    [SerializeField]
    private bool randomPos = true;
    [SerializeField, ShowIf("randomPos")]
    private Vector2 heigthLimits;
    [SerializeField, HideIf("randomPos")]
    private List<float> notRandomPosList;

    [SerializeField]
    private BirdController birdController;
    [SerializeField]
    private GameObject obstaclePrefab;
    [SerializeField]
    private float obstacleSpeed = 1f;
    [SerializeField]
    private float spawnDelay = 1f;
    [SerializeField]
    private Transform spawnPoint;
    [SerializeField]
    private Transform deadPoint;

    private List<GameObject> obstacles = new();
    private float elapsedTime = 0;
    private int notRandomIndex = 0;

    private void Start()
    {
        birdController.OnDieCallBack.AddListener(ResetLevel);
        ResetLevel();
    }
    private void SpawnObstacle()
    {
        GameObject newObstacle = GetUnactiveObstacle();
        newObstacle.transform.position = spawnPoint.position;
        RandomizeObstacleHeigth(newObstacle);
        newObstacle.SetActive(true);
    }

    private GameObject GetUnactiveObstacle()
    {
        GameObject obstacleFound = obstacles.Find(a => !a.activeInHierarchy);
        if(obstacleFound == null)
        {
            obstacleFound = CreateNewObstacle();
        }
        return obstacleFound;
    }

    private GameObject CreateNewObstacle()
    {
        GameObject newObstacle = Instantiate(obstaclePrefab, transform);
        obstacles.Add(newObstacle);
        return newObstacle;
    }

    private void RandomizeObstacleHeigth(GameObject obstacle)
    {
        if (randomPos)
        {
            obstacle.transform.position += Vector3.up * Random.Range(heigthLimits.x, heigthLimits.y);
            return;
        }

        obstacle.transform.position += Vector3.up * notRandomPosList[notRandomIndex];
        notRandomIndex++;

        if (notRandomIndex >= notRandomPosList.Count)
            notRandomIndex = 0;
    }
    
    void Update()
    {
        CheckDeath();
        MoveObstacles();

        if (!spawn)
            return;

        elapsedTime += Time.deltaTime;

        if(elapsedTime >= spawnDelay)
        {
            SpawnObstacle();
            elapsedTime = 0;
        }
    }

    public void ResetLevel()
    {
        if (!spawn)
            return;

        notRandomIndex = 0;
        foreach (var ob in obstacles)
        {
            ob.SetActive(false);
        }
        SpawnObstacle();
        elapsedTime = 0;
    }

    private void MoveObstacles()
    {
        foreach (var obstacle in obstacles)
        {
            if (obstacle.activeInHierarchy)
            {
                obstacle.transform.Translate(Vector2.left * obstacleSpeed * Time.deltaTime);
            }
        }
    }

    private void CheckDeath()
    {
        foreach (var obstacle in obstacles)
        {
            if (obstacle.transform.position.x < deadPoint.position.x)
            {
                obstacle.SetActive(false);
            }
        }
    }

    public Transform NextObPos()
    {
        Transform next = null;
        float closiest = float.MaxValue;
        foreach (var obstacle in obstacles)
        {
            if (obstacle.activeInHierarchy && obstacle.transform.position.x > birdController.transform.position.x && obstacle.transform.position.x < closiest)
            {
                closiest = obstacle.transform.position.x;
                next = obstacle.transform;
            }
        }

        return next;
    }

    private void OnDestroy()
    {
        birdController.OnDieCallBack.RemoveListener(ResetLevel);
    }
}
