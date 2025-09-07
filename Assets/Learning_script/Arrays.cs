using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Arrays : MonoBehaviour
{
    public int[] NoOfHouses = {1,2,3,4,5 };
    public int[,] _2darray = {{1,2,3},{6,7,8}};

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(NoOfHouses.Length);
        Debug.Log("Sum Of elements"+ NoOfHouses.Sum());

        for(int i =0; i < NoOfHouses.Length; i++)
        {
            Debug.Log(NoOfHouses[i]);
        }

        Debug.Log("Foreach");

        //Foreach
        foreach (int i in NoOfHouses)
        {
            Debug.Log(i);
        }
        Console.Clear();

        foreach(int i in _2darray)
        {
            Debug.Log(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
