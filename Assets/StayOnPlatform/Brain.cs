using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLS.Platform
{
    [RequireComponent(typeof(Rigidbody))]
    public class Brain : MonoBehaviour
    {
        public float timeAlive;
        public float distWalked;
        public int collideCount;
        public DNA dna;

        public GameObject eyes;
        [SerializeField]
        float eyesSpace = 1f;
        [SerializeField]
        float eyesDist = 1f;

        int DNALength = 4;
        bool alive = true;
        Vector3 lastPos;

        [SerializeField]
        LayerMask myLayer;

        //public GameObject ethanPrefab;
        //GameObject ethan;
        Rigidbody rb;

        //void OnDestroy()
        //{
        //    Destroy(ethan);
        //}

        void OnCollisionEnter(Collision obj)
        {
            if (obj.gameObject.tag == "dead")
            {
                alive = false;
                timeAlive = 0;
                distWalked = 0;
                return;
            }
            collideCount++;
        }

        public void Init()
        {
            //initialise DNA
            //0 none seeing
            //1 both seeing
            //2 left seeing
            //3 right seeing

            rb = GetComponent<Rigidbody>();
            dna = new DNA(DNALength, 360);
            timeAlive = 0;
            collideCount = 0;
            lastPos = transform.position;
            alive = true;
            //ethan = Instantiate(ethanPrefab, this.transform.position, this.transform.rotation);
            //ethan.GetComponent<UnityStandardAssets.Characters.ThirdPerson.AICharacterControl>().target =
                    //this.transform;

        }

        private void Update()
        {
            if (!alive) return;

            // left eye
            //Debug.DrawRay(eyes.transform.position + (Vector3.left * eyesSpace), eyes.transform.forward * eyesDist, Color.red, 1);
            bool leftEyeSeen = false;
            if (Physics.Raycast(eyes.transform.position + (Vector3.left * eyesSpace), eyes.transform.forward * eyesDist, out RaycastHit hitLeft, eyesDist, ~myLayer))
            {
                if (hitLeft.collider.gameObject.CompareTag("platform"))
                {
                    leftEyeSeen = true;
                }
            }

            // right eye
            //Debug.DrawRay(eyes.transform.position + (Vector3.right * eyesSpace), eyes.transform.forward * eyesDist, Color.red, 1);
            bool rightEyeSeen = false;
            if (Physics.Raycast(eyes.transform.position + (Vector3.right * eyesSpace), eyes.transform.forward * eyesDist, out RaycastHit hitRight, eyesDist, ~myLayer))
            {
                if (hitRight.collider.gameObject.CompareTag("platform"))
                {
                    rightEyeSeen = true;
                }
            }

            timeAlive = PopulationManager.elapsed;

            // read DNA
            float turn = 0;
            float move = 0;
            (float value, bool move) result = new();

            if (leftEyeSeen && rightEyeSeen)
            {
                result = GetMovementByDNA(0);
            }
            else if (!leftEyeSeen && !rightEyeSeen)
            {
                result = GetMovementByDNA(1);
            }
            else if (leftEyeSeen && !rightEyeSeen)
            {
                result = GetMovementByDNA(2);
            }
            else if (!leftEyeSeen && rightEyeSeen)
            {
                result = GetMovementByDNA(3);
            }

            // adjust result
            if (result.move)
            {
                move = result.value;
            }
            else
            {
                turn = result.value;
            }

            Vector3 turnVector = new(0, turn, 0);
            Vector3 moveVector = transform.position + (0.05f * move * transform.forward );


            distWalked += Vector3.Distance(lastPos, transform.position);

            lastPos = transform.position;
            
            rb.MovePosition(moveVector);
            transform.Rotate(turnVector);
        }

        private (float value, bool move) GetMovementByDNA(int geneIndex)
        {
            if (dna.GetGene(geneIndex) == 0)
            {
                return (1f, true);
            }
            return (dna.GetGene(geneIndex), false);
        }
    }
}