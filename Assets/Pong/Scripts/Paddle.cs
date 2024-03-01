using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

namespace MLS.Pong
{
    public class Paddle : MonoBehaviour
    {
        float yVel = 0;
        float paddleMinY = -4.6f;
        float paddleMaxY = 4.6f;
        float paddleMaxSpeed = 15f;

        public void ChangeVel(float value)
        {
            yVel = Mathf.Clamp(value, -1, 1);
        }


        void FixedUpdate()
        {
            float posy = Mathf.Clamp(transform.position.y + (yVel * Time.fixedDeltaTime * paddleMaxSpeed), paddleMinY, paddleMaxY);
            transform.position = new(transform.position.x, posy, transform.position.z);
        }
    }
}