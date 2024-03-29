﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLS.Platform
{
	public class DNA
	{

		List<int> genes = new List<int>();
		int dnaLength = 0;
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
				genes.Add(Random.Range(0, maxValues));
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
				if (i < dnaLength / 2.0)
				{
					int c = d1.genes[i];
					genes[i] = c;
				}
				else
				{
					int c = d2.genes[i];
					genes[i] = c;
				}
			}
		}

		public void Mutate()
		{
			genes[Random.Range(0, dnaLength)] = Random.Range(0, maxValues);
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
                0 => "none seeing",
                1 => "both eyes seeing",
                2 => "left eye seeing",
                3 => "right eye seeing",
                _ => "",
            };
        }
    }

}