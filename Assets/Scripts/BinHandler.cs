using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BinHandler : MonoBehaviour
{
    public int xExtent = 500;
    public int yExtent = 200;
    public int zExtent = 500;

    public int binSize = 50;

    private int numCol;
    private int numHigh;
    private int numRow;

    private List<GameObject>[,,] bin;
    private Vector3[,,] flowField;

	// Use this for initialization
	void Start ()
    {
        numCol = xExtent / binSize;
        numHigh = yExtent / binSize;
        numRow = zExtent / binSize;

        bin = new List<GameObject>[numCol, numHigh, numRow];

        flowField = new Vector3[numCol, numHigh, numRow];
        for (int i = 0; i < numCol; i++)
        {
            for (int j = 0; j < numHigh; j++)
            {
                for (int k = 0; k < numRow; k++)
                {
                    float theta = Mathf.PerlinNoise(i, j);
                    float phi = Mathf.PerlinNoise(i, k);
                    float beta = Mathf.PerlinNoise(j, k);
                    flowField[i, j, k] = new Vector3(Mathf.Cos(theta), Mathf.Cos(phi), Mathf.Cos(beta));
                }
            }
        }
    }

    public Vector3 GetFlowAt(Vector3 pos)
    {
        return flowField[(int)pos.x / binSize, (int)pos.y / binSize, (int)pos.z / binSize];
    }

    void ClearBin()
    {
        for(int i = 0; i < numCol; i++)
        {
            for(int j = 0; j < numHigh; j++)
            {
                for(int k = 0; k < numRow; k++)
                {
                    bin[i, j, k].Clear();
                }
            }
        }
    }

    void UpdateBin(List<GameObject> flock)
    {
        ClearBin();

        foreach(GameObject guy in flock)
        {
            Vector3 pos = guy.transform.position;
            bin[(int)pos.x / binSize, (int)pos.y / binSize, (int)pos.z / binSize].Add(guy);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
	    
	}
}
