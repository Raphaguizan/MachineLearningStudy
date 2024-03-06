using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MLS.QLearning;

namespace MLS.BallBalance
{
	public class Brain : MonoBehaviour
	{

		public GameObject ball;                         //object to monitor

		Agent ag;

		Vector3 ballStartPos;                           //record start position of object
		int failCount = 0;                              //count when the ball is dropped
		float tiltSpeed = 0.5f;                         //max angle to apply to tilting each update
														//make sure this is large enough so that the q value
														//multiplied by it is enough to recover balance
														//when the ball gets a good speed up
		float timer = 0;                                //timer to keep track of balancing
		float maxBalanceTime = 0;                       //record time ball is kept balanced	
														// Use this for initialization
		void Start()
		{
			ag = new(6, 4, 1, 6, 0.2f);
			ballStartPos = ball.transform.position;
			Time.timeScale = 5.0f;
		}

		GUIStyle guiStyle = new GUIStyle();
		void OnGUI()
		{
			guiStyle.fontSize = 25;
			guiStyle.normal.textColor = Color.white;
			GUI.BeginGroup(new Rect(10, 10, 600, 150));
			GUI.Box(new Rect(0, 0, 140, 140), "Stats", guiStyle);
			GUI.Label(new Rect(10, 25, 500, 30), "Fails: " + failCount, guiStyle);
			GUI.Label(new Rect(10, 50, 500, 30), "Decay Rate: " + ag.ExploreRate, guiStyle);
			GUI.Label(new Rect(10, 75, 500, 30), "Last Best Balance: " + maxBalanceTime, guiStyle);
			GUI.Label(new Rect(10, 100, 500, 30), "This Balance: " + timer, guiStyle);
			GUI.EndGroup();
		}

		// Update is called once per frame
		void Update()
		{
			if (Input.GetKeyDown("space"))
				ResetBall();
		}

		void FixedUpdate()
		{
			timer += Time.deltaTime;
			List<double> states = new List<double>();

			states.Add(this.transform.rotation.x);
			states.Add(this.transform.rotation.z);
			states.Add(ball.transform.position.x);
			states.Add(ball.transform.position.z);
			states.Add(ball.GetComponent<Rigidbody>().angularVelocity.x);
			states.Add(ball.GetComponent<Rigidbody>().angularVelocity.z);

			int maxQIndex = ag.GetQIndex(states);
			float qValue = ag.GetQValue(maxQIndex);

			Debug.Log(qValue);

            switch (maxQIndex)
			{
				case 0:
                    this.transform.Rotate(Vector3.right, tiltSpeed * qValue);
					break;
				case 1:
                    this.transform.Rotate(Vector3.right, -tiltSpeed * qValue);
					break;
				case 2:
                    this.transform.Rotate(Vector3.forward, -tiltSpeed * qValue);
					break;
				case 3:
                    this.transform.Rotate(Vector3.forward, -tiltSpeed * qValue);
					break;
            }
			//if (maxQIndex == 0)
			//	this.transform.Rotate(Vector3.right, tiltSpeed * (float)qs[maxQIndex]);
			//else if (maxQIndex == 1)
			//	this.transform.Rotate(Vector3.right, -tiltSpeed * (float)qs[maxQIndex]);

			if (ball.GetComponent<BallState>().dropped)
				ag.Reward(-1f);
			else
                ag.Reward(.1f);



            if (ball.GetComponent<BallState>().dropped)
			{
				ag.Train();

				if (timer > maxBalanceTime)
				{
					maxBalanceTime = timer;
				}

				timer = 0;

				ball.GetComponent<BallState>().dropped = false;
				this.transform.rotation = Quaternion.identity;
				ResetBall();
				failCount++;
			}
		}

		void ResetBall()
		{
			ball.transform.position = ballStartPos;
			ball.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
			ball.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
		}

		
	}
}