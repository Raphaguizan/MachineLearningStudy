using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MLS.Bird
{
	public class PopulationManager : MonoBehaviour
	{
		[SerializeField, Range(1f, 5f)]
		private float simulationSpeed = 1f;

		public GameObject botPrefab;
		public GameObject startingPos;
		public int populationSize = 50;
		public float trialTime = 5;

		[SerializeField]
		private bool activeSpawn = true;

		List<GameObject> population = new List<GameObject>();
		public static float elapsed = 0;
		int generation = 1;

        DNA fitnnessDNA = null;

        GUIStyle guiStyle = new GUIStyle();
		void OnGUI()
		{
			guiStyle.fontSize = 35;
			guiStyle.normal.textColor = Color.white;
			GUI.BeginGroup(new Rect(10, 10, 400, 500));
			GUI.Box(new Rect(0, 0, 140, 140), "Stats", guiStyle);
			GUI.Label(new Rect(10, 25, 200, 30), "Gen: " + generation, guiStyle);
			GUI.Label(new Rect(10, 60, 200, 30), string.Format("Time: {0:0.00}", elapsed), guiStyle);
			GUI.Label(new Rect(10, 85, 200, 30), "Population: " + population.Count, guiStyle);

            if (fitnnessDNA != null)
                GUI.Label(new Rect(40, 125, 300, 150), "Best DNA:\n\n" + fitnnessDNA.ToString(), guiStyle);

            GUI.EndGroup();
		}


		// Use this for initialization
		IEnumerator Start()
		{
			yield return new WaitUntil(()=> activeSpawn);
			for (int i = 0; i < populationSize; i++)
			{
				GameObject b = Instantiate(botPrefab, startingPos.transform.position, this.transform.rotation, transform);
				b.name += " " + i;
				b.GetComponent<Brain>().Init();
				population.Add(b);
			}
		}

		GameObject Breed(GameObject parent1, GameObject parent2)
		{
			GameObject offspring = Instantiate(botPrefab, startingPos.transform.position, this.transform.rotation, transform);
			offspring.name += " " + int.Parse(parent1.name[^1].ToString()) + int.Parse(parent2.name[^1].ToString());
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
			if (!activeSpawn)
				return;

			List<GameObject> sortedList = population.OrderBy(o => FitnessCalculus(o.GetComponent<Brain>())).ToList();

			fitnnessDNA = sortedList[^1].GetComponent<Brain>().dna;

			population.Clear();
			for (int i = (int)(3 * sortedList.Count / 4.0f) - 1; i < sortedList.Count - 1; i++)
			{
				population.Add(Breed(sortedList[i], sortedList[i + 1]));
				population.Add(Breed(sortedList[i + 1], sortedList[i]));
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
		private float FitnessCalculus(Brain b)
		{
			return b.timeAlive - b.crash;
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
	}
}