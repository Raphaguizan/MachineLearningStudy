using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BirdController : MonoBehaviour
{
    [SerializeField]
    private float jumpForce = 100f;
    [SerializeField]
    private bool autoReset = false;

    public UnityEvent OnDieCallBack;


    private Vector2 startPos;
    private Rigidbody2D rb;

    private bool died = false;

    public bool Die => died;
    public Vector2 Velocity => rb.velocity;

    private void Start()
    {
        startPos = transform.position;
        rb = GetComponent<Rigidbody2D>();
    }

    public void ResetBird()
    {
        transform.position = startPos;
        rb.velocity = Vector2.zero;
        died = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        died = true;
        OnDieCallBack.Invoke();
        if(autoReset)ResetBird();
    }

    public void Jump()
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
}
