using MLS.Pong;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MLS.Pong
{
    public class LevelManager : MonoBehaviour
    {
        public int player1Point = 0;
        public int player2Point = 0;

        public TextMeshPro player1UI;
        public TextMeshPro player2UI;

        public GameObject player1Wall;
        public GameObject player2Wall;

        public MoveBall ball;

        public float maxBallTime = 10f;

        [Range(1f, 5f)]
        public float timeScale = 1f;

        Coroutine myTimeCount = null;
        float elapsedTime = 0;

        // Start is called before the first frame update
        IEnumerator Start()
        {
            UpdateUI();

            yield return new WaitUntil(()=> ball.RB != null);
            StartBall();
            ball.onHitBackWall.AddListener(ResetGame);
            ball.onHitpaddle.AddListener(CountBallTime);
        }

        void StartBall()
        {
            ball.ResetBall(0);
        }

        void CountBallTime()
        {
            myTimeCount ??= StartCoroutine(CountBallTimeCoroutine());
            elapsedTime = 0;
        }

        IEnumerator CountBallTimeCoroutine()
        {
            while (gameObject.activeInHierarchy)
            {
                elapsedTime += Time.deltaTime;
                if(elapsedTime >= maxBallTime)
                {
                    StartBall();
                    elapsedTime = 0;
                }
                yield return new WaitForEndOfFrame();
            }
        }

        private void ResetGame(GameObject backWall)
        {
            if (backWall.Equals(player1Wall))
            {
                player2Point++;
                ball.ResetBall(-1);
            }
            else
            {
                player1Point++;
                ball.ResetBall(1);
            }
            UpdateUI();
        }

        void UpdateUI()
        {
            player1UI.text = player1Point.ToString("00");
            player2UI.text = player2Point.ToString("00");
        }

        private void Update()
        {
            Time.timeScale = timeScale;
        }

        private void OnDestroy()
        {
            ball.onHitBackWall.RemoveListener(ResetGame);
            ball.onHitpaddle.RemoveListener(CountBallTime);
        }
    }
}