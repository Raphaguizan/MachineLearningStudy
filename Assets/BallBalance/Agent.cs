using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;
using Random = UnityEngine.Random;

namespace MLS.QLearning
{
    public class Replay
    {
        public List<double> states;
        public double reward;

        public Replay(List<double> st, double r)
        {
            states = st;
            reward = r;
        }
    }
    public class Agent
    {
        ANN ann;

        float discount = 0.99f;                         //how much future states affect rewards
        public float exploreRate = 90.0f;                     //chance of picking random action
        float maxExploreRate = 100.0f;                  //max chance value
        float minExploreRate = 0.01f;                   //min chance value
        float exploreDecay = 0.0005f;                   //chance decay amount for each update

        float reward = 0.0f;                            //reward to associate with actions
        List<Replay> replayMemory = new List<Replay>(); //memory - list of past actions and rewards
        int mCapacity = 1000;                          //memory capacity
        private int numOutput;

        double maxQ;
        private List<double> currentState = new();


        List<double> qs = new List<double>();

        private string savePath = "";
        public bool showDebug = false;

        public Agent(int nI, int nO, int nH, int nPH, double a, ActivationType hiddenAct = ActivationType.TANH, ActivationType outputAct = ActivationType.SIGMOID)
        {
            ann = new ANN(nI, nO, nH, nPH, a, hiddenAct, outputAct);
            numOutput = nO;
        }

        public void SetActivationFunction(ActivationType hiddenActivationFunc, ActivationType outputActivationFunc)
        {
            ann.ChangeActivationFunction(hiddenActivationFunc, outputActivationFunc);
        }

        List<double> SoftMax(List<double> values)
        {
            double max = values.Max();

            float scale = 0.0f;
            for (int i = 0; i < values.Count; ++i)
                scale += Mathf.Exp((float)(values[i] - max));

            List<double> result = new List<double>();
            for (int i = 0; i < values.Count; ++i)
                result.Add(Mathf.Exp((float)(values[i] - max)) / scale);

            return result;
        }

        public int GetQIndex(List<double> states)
        {
            currentState = states;
            qs.Clear();

            qs = SoftMax(ann.CalcOutput(states));
            maxQ = qs.Max();
            int maxQIndex = qs.ToList().IndexOf(maxQ);
            exploreRate = Mathf.Clamp(exploreRate - exploreDecay, minExploreRate, maxExploreRate);

            if (Random.Range(0, 100) < exploreRate)
                maxQIndex = Random.Range(0, numOutput);

            return maxQIndex;
        } 

        public float GetQValue(int index)
        {
            if (qs == null || qs.Count <= 0)
                return 0;

            return (float)qs[index];
        }

        public float GetQValue(List<double> states)
        {
            currentState = states;
            int index = GetQIndex(states);

            return (float)qs[index];
        }

        public void Reward(float reward)
        {
            if (currentState.Count <= 0)
                return;

            Reward(currentState, reward);
        }

        public void Reward(List<double> state, float reward)
        {
            this.reward = reward;
            if(showDebug)Debug.Log($"Rewarded ( {reward} )");
            Replay lastMemory = new Replay(state, reward);

            if (replayMemory.Count > mCapacity)
                replayMemory.RemoveAt(0);

            replayMemory.Add(lastMemory);
        }

        public void Train()
        {
            if (replayMemory.Count < mCapacity)
                return;

            for (int i = replayMemory.Count - 1; i >= 0; i--)
            {
                List<double> toutputsOld = new List<double>();
                List<double> toutputsNew = new List<double>();
                toutputsOld = SoftMax(ann.CalcOutput(replayMemory[i].states));

                double maxQOld = toutputsOld.Max();
                int action = toutputsOld.ToList().IndexOf(maxQOld);

                double feedback;
                if (i == replayMemory.Count - 1 || replayMemory[i].reward == -1)
                    feedback = replayMemory[i].reward;
                else
                {
                    toutputsNew = SoftMax(ann.CalcOutput(replayMemory[i + 1].states));
                    maxQ = toutputsNew.Max();
                    feedback = (replayMemory[i].reward +
                        discount * maxQ);
                }

                toutputsOld[action] = feedback;
                ann.Train(replayMemory[i].states, toutputsOld);
            }
            //ResetMemory();
        }

        public void ResetMemory()
        {
            replayMemory.Clear();
        }

        public void SaveWeights()
        {
            if (savePath.Equals(""))
            {
                Debug.LogError("The save path is empty!");
                return;
            }
            SaveWeights(savePath);
        }

        public void SaveWeights(string path)
        {
            savePath = path;
            string p = Application.dataPath + path;
            StreamWriter wf = File.CreateText(p);
            wf.WriteLine(ann.PrintWeights());
            wf.Close();
        }

        public bool LoadWeights()
        {
            return LoadWeights(savePath);
        }

        public bool LoadWeights(string path)
        {
            savePath = path;
            string p = Application.dataPath + path;
            StreamReader wf;
            try
            {
                wf = File.OpenText(p);
            }
            catch (Exception e)
            {
                return false;
            }

            if (File.Exists(p))
            {
                string line = wf.ReadLine();
                ann.LoadWeights(line);
                return true;
            }

            return false;
        }
    }
}