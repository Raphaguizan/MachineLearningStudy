using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLS.Pong;
using System;
using System.IO;
using System.Data;
using System.Linq;

namespace MLS.Race
{
    public class ANNDrive : MonoBehaviour
    {
        ANN ann;
        public float visibleDistance = 200f;
        public int epochs = 1000;
        public float speed = 50f;
        public float rotationSpeed = 100f;

        bool trainingDone = false;
        float trainingProgress = 0;
        double sse = 0;
        double lastSSE = 1;

        public float translation;
        public float rotation;

        public bool showDebug = true;
        public bool loadFromFile = false;


        // Start is called before the first frame update
        void Start()
        {
            ann = new(5,2,2,10,.5f);

            if (loadFromFile && LoadWeightsFromFile())
                trainingDone = true;
            else
                StartCoroutine(LoadTrainingSet());
        }

        private void OnGUI()
        {
            GUIStyle style = new();
            style.fontSize = 50;
            style.normal.textColor = Color.white;
            GUI.Label(new Rect(25, 25, 250, 30), "SSE: " + lastSSE, style);
            GUI.Label(new Rect(25, 80, 250, 30), "alpha: " + ann.alpha, style);
            GUI.Label(new Rect(25, 135, 250, 30), "Trained: " + trainingProgress.ToString("00%"), style);
        }


        IEnumerator LoadTrainingSet()
        {
            string path = Application.dataPath + "/RaceIA/Data/Training.txt";
            string line;

            if (!File.Exists(path))
            {

                yield break;
            }

            int lineCount = File.ReadAllLines(path).Length;
            StreamReader tdf = File.OpenText(path);

            List<double> calcOutputs = new();
            List<double> inputs = new();
            List<double> outputs = new();

            for (int i = 0; i < epochs; i++)
            {
                // set file pointer to beginning of file
                sse = 0;
                tdf.BaseStream.Position = 0;
                string currentWeights = ann.PrintWeights();
                while ((line = tdf.ReadLine()) != null)
                {
                    string[] data = line.Split(';');
                    //if nothing to be learned ignore this line
                    float thisError = 0;

                    inputs.Clear();
                    outputs.Clear();
                    inputs.Add(double.Parse(data[0]));
                    inputs.Add(double.Parse(data[1]));
                    inputs.Add(double.Parse(data[2]));
                    inputs.Add(double.Parse(data[3]));
                    inputs.Add(double.Parse(data[4]));

                    double o1 = float.Parse(data[5]).Map(0, 1f, -1f, 1f);
                    outputs.Add(o1);
                    double o2 = float.Parse(data[6]).Map(0, 1f, -1f, 1f);
                    outputs.Add(o2);

                    calcOutputs = ann.Train(inputs, outputs);
                    thisError = (Mathf.Pow((float)(outputs[0] - calcOutputs[0]), 2) * Mathf.Pow((float)(outputs[1] - calcOutputs[1]), 2)) / 2f;
                    sse += thisError;
                }
                trainingProgress = (float)i / (float)epochs;
                sse /= lineCount;

                //if sse isn't better then reload previous set of weights
                //and decrease alpha

                if (lastSSE < sse)
                {
                    ann.LoadWeights(currentWeights);
                    ann.alpha = Mathf.Clamp((float)ann.alpha - .001f, .01f, .9f);
                }
                else // increase alpha
                {
                    ann.alpha = Mathf.Clamp((float)ann.alpha + .001f, .01f, .9f);
                    lastSSE = sse;
                }

                yield return null;
            }
            trainingDone = true;
            SaveWeightsToFile();
        }

        // Update is called once per frame
        void Update()
        {
            if (!trainingDone)
                return;

            List<double> inputs = new();
            List<double> outputs = new();

            List<float> sensors = RaycastEyes();

            inputs.Add(sensors[0]);
            inputs.Add(sensors[1]);
            inputs.Add(sensors[2]);
            inputs.Add(sensors[3]);
            inputs.Add(sensors[4]);
            
            outputs.Add(0);
            outputs.Add(0);

            List<double> calcOutputs = ann.CalcOutput(inputs, outputs);
            float translationInput = ((float)calcOutputs[0]).Map(-1f, 1f, 0, 1f);
            float rotationInput = ((float)calcOutputs[1]).Map(-1f, 1f, 0, 1f);

            translation = translationInput * speed * Time.deltaTime;
            rotation = rotationInput * rotationSpeed * Time.deltaTime;

            transform.Translate(0,0,translation);
            transform.Rotate(0,rotation,0);
        }

        List<float> RaycastEyes()
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

            return new List<float>() { fDist, rDist, lDist, r45Dist, l45Dist };
        }

        void SaveWeightsToFile()
        {
            string path = Application.dataPath + "/RaceIA/Data/weights.txt";
            StreamWriter wf = File.CreateText(path);
            wf.WriteLine(ann.PrintWeights());
            wf.Close();
        }

        bool LoadWeightsFromFile()
        {
            string path = Application.dataPath + "/RaceIA/Data/weights.txt";
            StreamReader wf;
            try
            {
                wf = File.OpenText(path);
            }
            catch (Exception e)
            {
                return false;
            }

            if (File.Exists(path))
            {
                string line = wf.ReadLine();
                ann.LoadWeights(line);
                return true;
            }

            return false;
        }
    }
}