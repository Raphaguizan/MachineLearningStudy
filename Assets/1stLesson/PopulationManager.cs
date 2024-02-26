using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Palmmedia.ReportGenerator.Core;
using Unity.VisualScripting;

public class PopulationManager : MonoBehaviour
{
    public GameObject personPrefab;
    public int populationSize = 10;
    List<GameObject> population = new();

    public static float elapsed = 0f;

    int trialTime = 10;
    int generation = 1;

    void Start()
    {
        for (int i = 0; i < populationSize; i++)
        {
            Vector3 pos = GetPos();
            GameObject go = Instantiate(personPrefab, pos, Quaternion.identity);
            DNA newDNA = go.GetComponent<DNA>();
            RandomizeGenes(newDNA);

            population.Add(go);
        }
    }
    void OnGUI()
    {
        GUIStyle myStile = new();
        myStile.fontSize = 50;
        myStile.normal.textColor = Color.white;
        GUI.Label(new Rect(10, 10, 100, 20), "Generation: " + generation, myStile);
        GUI.Label(new Rect(10, 65, 100, 20), "Trial Time: " + (10 - (int)elapsed), myStile);
    }
    void Update()
    {
        elapsed += Time.deltaTime;
        bool noPopulation = true;
        foreach (GameObject go in population)
        {
            if (go.GetComponent<SpriteRenderer>().enabled)
            {
                noPopulation = false;
                break;
            }
        }
        if (elapsed > trialTime || noPopulation)
        {
            BreedNewPopulation(); 
            elapsed = 0;
        }
    }

    void BreedNewPopulation()
    { 
        List<GameObject> newPopulation = new List<GameObject>();
        //get rid of unfit individuals
        List<GameObject> sortedList = population.OrderBy(o => o.GetComponent<DNA>().timeToDie).ToList();

        population.Clear( ) ;
        //breed upper half of sorted list
        for (int i = (int) (sortedList. Count / 2.0f) - 1; i < sortedList.Count - 1; i++) 
        {
            population.Add(Breed(sortedList[i], sortedList[i + 1]) ); 
            population.Add(Breed(sortedList[i + 1], sortedList[i]));
        }
        //destroy all parents and previous population
        for(int i = 0; i < sortedList. Count; i++)
        {
            Destroy(sortedList[i]);
        }
        generation++;
    }

    GameObject Breed(GameObject parent1, GameObject parent2)
    {
        Vector3 pos = GetPos();
        GameObject offspring = Instantiate(personPrefab, pos, Quaternion.identity);
        DNA offspringDNA = offspring.GetComponent<DNA>();

        DNA dna1 = parent1.GetComponent<DNA>(); 
        DNA dna2 = parent2.GetComponent<DNA>();
        //swap parent dna
        if(Random.Range(0,100) > 1)
        {
            offspringDNA.r = Random.Range(0, 10) < 5 ? dna1.r : dna2.r;
            offspringDNA.g = Random.Range(0, 10) < 5 ? dna1.g : dna2.g;
            offspringDNA.b = Random.Range(0, 10) < 5 ? dna1.b : dna2.b;
            offspringDNA.size = Random.Range(0, 10) < 5 ? dna1.size : dna2.size;
        }
        else
        {
            RandomizeGenes(offspringDNA);
        }

        return offspring;
    }

    Vector3 GetPos()
    {
        return new Vector3(Random.Range(-8, 8), Random.Range(-4f, 4f), 0);
    }

    private void RandomizeGenes(DNA dna)
    {
        dna.GetComponent<DNA>().r = Random.Range(0, 1f);
        dna.GetComponent<DNA>().g = Random.Range(0, 1f);
        dna.GetComponent<DNA>().b = Random.Range(0, 1f);
        dna.GetComponent<DNA>().size = Random.Range(.1f, 2f);
    }
}
