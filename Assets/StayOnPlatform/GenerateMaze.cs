using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMaze : MonoBehaviour {

	public GameObject blockPrefab;
	int width = 40;
	int depth = 40;

    public float timeToRegen = 60f;
    float elapsedTime = 0;

	// Use this for initialization
	void Awake () 
    {
        Generate();
	}

    [ContextMenu("Regen")]
    private void Regenerate()
    {
        for (int i = 0; i < transform.childCount; i++) 
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        Generate();
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        if(elapsedTime >= timeToRegen)
        {
            Regenerate();
            elapsedTime = 0;
        }
    }

    private void Generate()
	{
        for (int w = 0; w < width; w++)
        {
            for (int d = 0; d < depth; d++)
            {
                if (w == 0 || d == 0)   //outside walls bottom and left
                {
                    Instantiate(blockPrefab, new Vector3(w + this.transform.position.x, this.transform.position.y, d + this.transform.position.z), Quaternion.identity, transform);
                }
                else if (w < 6 && d < 6)
                {
                    continue;
                }
                else if (w == width - 1 || d == depth - 1) //outside walls top and right
                {
                    Instantiate(blockPrefab, new Vector3(w + this.transform.position.x, this.transform.position.y, d + this.transform.position.z), Quaternion.identity, transform);
                }
                else if (w > width - 5 && d > depth - 5)
                {
                    continue;
                }
                else if (Random.Range(0, 5) < 1)
                {
                    Instantiate(blockPrefab, new Vector3(w + this.transform.position.x, this.transform.position.y, d + this.transform.position.z), Quaternion.identity, transform);
                }
            }
        }
    }
}
