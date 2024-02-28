using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLS.Bird
{
    public class Brain : MonoBehaviour
    {
        int DNALength = 5;
        
        public DNA dna;
        public BirdEyes eyes;
        public float timeAlive = 0;
        public float distanceTravelled = 0;
        public int crash = 0;
        [SerializeField]
        private bool showDebug = false;

        Vector3 startPosition;
        bool alive = true;
        Rigidbody2D rb;

        BirdVision currentVision => eyes.CurrentVision;

        public void Init()
        {
            //initialise DNA
            //0 forward
            //1 upwall
            //2 downwall
            //3 normal upward
            dna = new DNA(DNALength, 200);
            this.transform.Translate(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), 0);
            startPosition = this.transform.position;
            rb = this.GetComponent<Rigidbody2D>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "dead")
            {
                alive = false;
            }
        }
        void OnCollisionStay2D(Collision2D col)
        {
            if (col.gameObject.tag == "top" ||
                col.gameObject.tag == "bottom" ||
                col.gameObject.tag == "upwall" ||
                col.gameObject.tag == "downwall")
            {
                crash++;
            }
        }


        void Update()
        {
            if (!alive) return;
            timeAlive = PopulationManager.elapsed;
        }


        void FixedUpdate()
        {
            if (!alive) return;

            // read DNA
            float h = 0;
            float v = 1.0f; //dna.GetGene(0);

            if (showDebug)
                Debug.Log(gameObject.name +" -> "+ currentVision);

            h = dna.GetGene((int)currentVision);

            //if (seeUpWall)
            //{
            //    h = dna.GetGene(0);
            //}
            //else if (seeDownWall)
            //{
            //    h = dna.GetGene(1);
            //}
            //else if (seeTop)
            //{
            //    h = dna.GetGene(2);
            //}
            //else if (seeBottom)
            //{
            //    h = dna.GetGene(3);
            //}
            //else
            //{
            //    h = dna.GetGene(4);
            //}

            rb.AddForce(this.transform.right * v);
            rb.AddForce(this.transform.up * h * 0.1f);
            distanceTravelled = Vector3.Distance(startPosition, this.transform.position);
        }
    }

}