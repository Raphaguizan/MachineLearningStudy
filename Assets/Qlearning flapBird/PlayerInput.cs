using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public BirdController controller;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            controller.Jump();
        }
    }
}
