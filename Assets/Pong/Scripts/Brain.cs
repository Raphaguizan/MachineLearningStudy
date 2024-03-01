using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLS.Pong
{
    public class Brain : MonoBehaviour
    {
        public GameObject paddle;
        public GameObject ball;
        Rigidbody2D brb;

        float yVel;
        float paddleMinY = 8.8f;
        float paddleMaxY = 17.4f;
        float paddleMaxSpeed = 15f;

        public float numSaved = 0;
        public float numIssed = 0;

        ANN ann;

        void Start()
        {
            ann = new(6, 1, 1, 4, .1f);
            brb = ball.GetComponent<Rigidbody2D>();
        }

        List<double> Run(double bx, double by, double bvx, double bvy, double px, double py, double pv, bool train)
        {
            List<double> inputs = new();
            List<double> output = new();

            inputs.Add(bx);
            inputs.Add(by);
            inputs.Add(bvx);
            inputs.Add(bvy);
            inputs.Add(px);
            inputs.Add(py);
            output.Add(pv);

            if (train)
                return ann.Train(inputs, output);
            else
                return ann.CalcOutput(inputs, output);
        }

        void Update()
        {
            float posy = Mathf.Clamp(paddle.transform.position.y + (yVel * Time.deltaTime * paddleMaxSpeed), paddleMinY, paddleMaxY); 
            paddle.transform.position = new(paddle.transform.position.x, posy, paddle.transform.position.z);
            
            List<double> output = new(); 
            int layerMask = 1 << 9; 
            RaycastHit2D hit = Physics2D.Raycast(ball.transform.position, brb.velocity, 1000, layerMask);
            if(hit.collider != null)
            {
                if (hit.collider.CompareTag("tops"))
                {
                    Vector3 reflection = Vector3.Reflect(brb.velocity, hit.normal);
                    hit = Physics2D.Raycast(hit.point, reflection, 1000, layerMask);
                }

                if (hit.collider != null && hit.collider.gameObject.tag == "backwall")
                {
                    float dy = (hit.point.y - paddle.transform.position.y);
                    output = Run(ball.transform.position.x, ball.transform.position.y, brb.velocity.x, brb.velocity.y, paddle.transform.position.x, paddle.transform.position.y, dy, true);
                    yVel = (float)output[0];
                }
            }
            else
                yVel = 0;
        }
    }
}