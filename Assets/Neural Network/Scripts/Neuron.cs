using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLS.NNw
{
    public class Neuron
    {
        public int numInputs;
        public double bias;
        public double output;
        public double errorGradient;
        public List<double> weights = new();
        public List<double> inputs = new();

        public Vector2 valuesRange = new(-1f, 1f);

        public Neuron(int nInputs)
        {
            bias = Random.Range(valuesRange.x, valuesRange.y);
            numInputs = nInputs;
            for (int i = 0; i < nInputs; i++)
            {
                weights.Add(Random.Range(valuesRange.x, valuesRange.y));
            }
        }
    }
}