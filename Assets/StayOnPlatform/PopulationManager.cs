using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MLS.Platform
{
	public class PopulationManager : MonoBehaviour
	{
		[SerializeField, Range(1, 10)]
		public float simulationSpeed = 1f;

		public GameObject botPrefab;
		public int populationSize = 50;
		List<GameObject> population = new();
		public static float elapsed = 0;
		public float trialTime = 5;
		int generation = 1;

		DNA fitnnessDNA = null;
		GUIStyle guiStyle = new();
		void OnGUI()
		{
			guiStyle.fontSize = 35;
			guiStyle.normal.textColor = Color.white;
			GUI.BeginGroup(new Rect(10, 10, 400, 500));
			GUI.Box(new Rect(0, 0, 400, 500), "Stats", guiStyle);
			GUI.Label(new Rect(10, 30, 200, 30), "Gen: " + generation, guiStyle);
			GUI.Label(new Rect(10, 60, 200, 30), string.Format("Time: {0:0.00}", elapsed), guiStyle);
			GUI.Label(new Rect(10, 85, 200, 30), "Population: " + population.Count, guiStyle);
			
			if(fitnnessDNA != null)
				GUI.Label(new Rect(40, 125, 300, 150), "Best DNA:\n\n" + fitnnessDNA.ToString(), guiStyle);
			
			GUI.EndGroup();
		}


		// Use this for initialization
		void Start()
		{
			for (int i = 0; i < populationSize; i++)
			{
				Vector3 startingPos = GetRandomPos();

				GameObject b = Instantiate(botPrefab, startingPos, this.transform.rotation, transform);
				b.GetComponent<Brain>().Init();
				population.Add(b);

				if(i < 5)
				{
					b.GetComponent<Brain>().dna.SetInt(0, 0);

                }
			}
		}

		GameObject Breed(GameObject parent1, GameObject parent2)
		{
			Vector3 startingPos = GetRandomPos();
			GameObject offspring = Instantiate(botPrefab, startingPos, this.transform.rotation, transform);
			Brain b = offspring.GetComponent<Brain>();
			
			b.Init();
			b.dna.Combine(parent1.GetComponent<Brain>().dna, parent2.GetComponent<Brain>().dna);

            if (Random.Range(0, 100) == 1) //mutate 1 in 100
            {
                b.dna.Mutate();
            }

            return offspring;
		}

		void BreedNewPopulation()
		{
			List<GameObject> sortedList = population.OrderByDescending(o =>FitnessCalculus(o.GetComponent<Brain>())).ToList();
			fitnnessDNA = sortedList[0].GetComponent<Brain>().dna;

			population.Clear();
            //breed upper half of sorted list
            //for (int i = (int)(sortedList.Count / 2.0f) - 1; i < sortedList.Count - 1; i++)
            for (int i = 0; i < (int)(sortedList.Count / 2.0f); i++)
			{
				population.Add(Breed(sortedList[i], sortedList[i + 1]));
				population.Add(Breed(sortedList[i + 1], sortedList[i]));
			}
			//destroy all parents and previous population
			for (int i = 0; i < sortedList.Count; i++)
			{
				Destroy(sortedList[i]);
			}
			generation++;
		}

		[ContextMenu ("Fitness DNA")]
		public void PrintFitness()
		{
			Debug.Log(fitnnessDNA.ToString());
		}

		private float FitnessCalculus(Brain b)
		{
			float resp = 0;
			resp += ((b.distWalked + (Vector3.Distance(b.transform.position, transform.position)*2)) / 2) * 5;
			resp += b.timeAlive; 
			resp -= b.collideCount * 3;
			return resp;
        }

		// Update is called once per frame
		void Update()
		{
            elapsed += Time.deltaTime;
			if (elapsed >= trialTime)
			{
				BreedNewPopulation();
				elapsed = 0;
			}
			
			Time.timeScale = simulationSpeed;
		}

		private Vector3 GetRandomPos()
		{
            return new Vector3(this.transform.position.x + Random.Range(-2f, 2f),
                               this.transform.position.y,
                               this.transform.position.z + Random.Range(-2f, 2f));
        }
	}

}