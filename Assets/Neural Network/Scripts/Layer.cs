using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MLS.NNw
{
    public class Layer
    {
        public int numNeurons;
        public List<Neuron> neurons = new();

        public Layer(int nNeurons, int nNeuronInputs) 
        {
            numNeurons = nNeurons;
            for (int i = 0; i < nNeurons; i++)
            {
                neurons.Add(new Neuron(nNeuronInputs));
            }
        }
    }
}