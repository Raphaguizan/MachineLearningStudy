using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLS.Pong
{
    public class PlayerInputs : MonoBehaviour
    {
        public Paddle paddle;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                paddle.ChangeVel(1f);
            }
            else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                paddle.ChangeVel(-1f);
            }
            else
            {
                paddle.ChangeVel(0);
            }
        }
    }
}