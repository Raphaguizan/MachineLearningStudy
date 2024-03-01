using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MLS.Pong
{
	public class MoveBall : MonoBehaviour
	{

		Vector3 ballStartPosition;
		Rigidbody2D rb;
		float speed = 400;
		public AudioSource blip;
		public AudioSource blop;

		[HideInInspector]
		public UnityEvent<GameObject> onHitBackWall = new();
        [HideInInspector]
        public UnityEvent onHitpaddle = new();

        public Rigidbody2D RB => rb;
		// Use this for initialization
		void Start()
		{
			rb = this.GetComponent<Rigidbody2D>();
			ballStartPosition = this.transform.position;
		}

		void OnCollisionEnter2D(Collision2D col)
		{
			if (col.gameObject.CompareTag("backwall"))
			{
				blop.Play();
				onHitBackWall.Invoke(col.gameObject);
			}
			else
				blip.Play();

            if (col.gameObject.CompareTag("paddle"))
            {
                onHitpaddle.Invoke();
            }
        }

		public void ResetBall(int side)
		{
			this.transform.position = ballStartPosition;
			rb.velocity = Vector3.zero;

            float xDir = 0;
			if (side > 0)
				xDir = Random.Range(0, 100f);
			else if (side < 0)
				xDir = Random.Range(-100f, 0);
			else
                xDir = Random.Range(-100f, 100f);


            Vector2 dir = new Vector2(xDir, Random.Range(-100f, 100f)).normalized;
			rb.AddForce(dir * speed);
		}
    }
}