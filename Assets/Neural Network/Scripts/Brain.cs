using MLS.DodgeBall;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLS.NNw
{
    [System.Serializable]
    public class TrainingSet
    {
        public double[] input;
        public double output;
    }
    public class Brain : MonoBehaviour
    {
        [SerializeField]
        private int NumEpocs = 1000;
        public TrainingSet[] ts;

        ANN ann;
        double sumSquareError = 0;

        private void Start()
        {
            ann = new(2, 1, 1, 2, .1f);
            List<double> results;

            for (int i = 0; i < NumEpocs; i++)
            {
                sumSquareError = 0;
                for (int j = 0; j < ts.Length; j++)
                {
                    results = Train(ts[j].input[0], ts[j].input[1], ts[j].output);
                    sumSquareError += Mathf.Pow((float)results[0] - (float)ts[j].output, 2);
                }
            }

            Debug.Log("SSE: "+sumSquareError);

            for (int i = 0; i < ts.Length; i++)
            {
                results = Train(ts[i].input[0], ts[i].input[1], ts[i].output);
                Debug.Log($"{(int)ts[i].input[0]}, {(int)ts[i].input[1]} => {Mathf.RoundToInt((float)results[0])}  ({results[0]})");
            }
        }

        private List<double> Train(double v1, double v2, double o)
        {
            List<double> inputs = new();
            List<double> outputs = new();

            inputs.Add(v1);
            inputs.Add(v2);
            outputs.Add(o);

            return ann.Go(inputs, outputs);
        }
    }
}