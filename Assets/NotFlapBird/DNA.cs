using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Random = UnityEngine.Random;

namespace MLS.Bird
{
	[Serializable]
	public class DNA
	{
		[SerializeField, AllowNesting]
		List<int> genes = new List<int>();
		[SerializeField, AllowNesting]
		int dnaLength = 0;
		[SerializeField, AllowNesting]
		int maxValues = 0;

		public DNA(int l, int v)
		{
			dnaLength = l;
			maxValues = v;
			SetRandom();
		}

		public void SetRandom()
		{
			genes.Clear();
			for (int i = 0; i < dnaLength; i++)
			{
				genes.Add(Random.Range(-maxValues, maxValues));
			}
		}

		public void SetInt(int pos, int value)
		{
			genes[pos] = value;
		}

		public void Combine(DNA d1, DNA d2)
		{
			for (int i = 0; i < dnaLength; i++)
			{
				genes[i] = Random.Range(0, 10) < 5 ? d1.genes[i] : d2.genes[i];
			}
		}

		public void Mutate()
		{
			genes[Random.Range(0, dnaLength)] = Random.Range(-maxValues, maxValues);
		}

		public int GetGene(int pos)
		{
			return genes[pos];
		}

        public override string ToString()
        {
            string resp = "";
            for (int i = 0; i < genes.Count; i++)
            {
                resp += $"{GeneName(i)} ({genes[i]})\n";
            }
            return resp;
        }

        private string GeneName(int index)
        {
            return index switch
            {
                0 => "see Up Wall",
                1 => "see Down Wall",
                2 => "see Top",
                3 => "see Bottom",
				4 => "normal Upward",
                _ => "",
            };
        }

    }
}