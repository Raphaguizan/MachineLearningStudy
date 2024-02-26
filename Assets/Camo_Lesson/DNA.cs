using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLS.Camouflage
{
    public class DNA : MonoBehaviour
    {
        // gene for color
        public float r;
        public float g;
        public float b;
        public float size;

        public float timeToDie = 0f;
        SpriteRenderer sRender;
        Collider2D sCollider;

        //bool dead = false;

        private void OnMouseDown()
        {
            //dead = true;
            timeToDie = PopulationManager.elapsed;
            //Debug.Log("Dead At:" + timeToDie);
            sRender.enabled = false;
            sCollider.enabled = false;
        }

        // Start is called before the first frame update
        void Start()
        {
            sRender = GetComponent<SpriteRenderer>();
            sCollider = GetComponent<Collider2D>();
            sRender.color = new Color(r, g, b);
            transform.localScale *= size;
        }
    }
}