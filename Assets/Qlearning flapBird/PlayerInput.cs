using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public BirdController controller;
    public FlapBirdBrain birdBrain;
    

    [Space, SerializeField]
    private bool activeTrain = false;


    private void Awake()
    {
        birdBrain.ActivePlayerMode(activeTrain);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            controller.Jump();
        }
    }
}
