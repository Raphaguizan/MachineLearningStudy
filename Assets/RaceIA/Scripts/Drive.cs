using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.IO;
using TMPro;

namespace MLS.Race
{
    public class Drive : MonoBehaviour
    {
        public float speed = 50f; 
        public float rotationSpeed = 100f;

        public float visibleDistance = 200f;
        List<string> collectedTrainingData = new();

        [SerializeField]
        private bool showDebug = true;

        StreamWriter tdf;


        float translationInput;
        float rotationInput;

        private void Start()
        {
            string path = Application.dataPath + "/RaceIA/Data/Training.txt";
            tdf = File.CreateText(path);
        }

        private void OnApplicationQuit()
        {
            foreach (var td in collectedTrainingData)
            {
                tdf.WriteLine(td);
            }
            tdf.Close();
        }

        void Update()
        {
            translationInput = Input.GetAxis("Vertical");
            rotationInput = Input.GetAxis("Horizontal");

            float translation = Time.deltaTime * speed * translationInput;
            float rotation = Time.deltaTime * rotationSpeed * rotationInput;


            transform.Translate(0, 0, translation); 
            transform.Rotate(0, rotation, 0);

            string td = RaycastEyes();

            if (translationInput != 0 && rotationInput != 0)
            {
                if (!collectedTrainingData.Contains(td))
                    collectedTrainingData.Add(td);
            }
        }

        string RaycastEyes()
        {
            if (showDebug)
            {
                Debug.DrawRay(transform.position, transform.forward * visibleDistance, Color.red);
                Debug.DrawRay(transform.position, transform.right * visibleDistance, Color.red);
                Debug.DrawRay(transform.position, -transform.right * visibleDistance, Color.red);
                Debug.DrawRay(transform.position, Quaternion.AngleAxis(-45, Vector3.up) * transform.right * visibleDistance, Color.red);
                Debug.DrawRay(transform.position, Quaternion.AngleAxis(45, Vector3.up) * -transform.right * visibleDistance, Color.red);
            }

            RaycastHit hit;
            float fDist, rDist, lDist, r45Dist, l45Dist;
            fDist = rDist = lDist = r45Dist = l45Dist = 0;

            // forward
            if (Physics.Raycast(transform.position, transform.forward, out hit, visibleDistance))
            {
                fDist = 1 - (hit.distance / visibleDistance).Round();
            }
            // right
            if (Physics.Raycast(transform.position, transform.right, out hit, visibleDistance))
            {
                rDist = 1 - (hit.distance / visibleDistance).Round();
            }
            // left
            if (Physics.Raycast(transform.position, -transform.right, out hit, visibleDistance))
            {
                lDist = 1 - (hit.distance / visibleDistance).Round();
            }
            // right 45
            if (Physics.Raycast(transform.position, Quaternion.AngleAxis(-45, Vector3.up) * transform.right, out hit, visibleDistance))
            {
                r45Dist = 1 - (hit.distance / visibleDistance).Round();
            }
            // left 45
            if (Physics.Raycast(transform.position, Quaternion.AngleAxis(45, Vector3.up) * -transform.right, out hit, visibleDistance))
            {
                l45Dist = 1 - (hit.distance / visibleDistance).Round();
            }

            return $"{fDist};{rDist};{lDist};{r45Dist};{l45Dist};{translationInput.Round()};{rotationInput.Round()}";
        }
    }
}