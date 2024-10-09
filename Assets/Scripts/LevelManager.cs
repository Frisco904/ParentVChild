using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    //Static means it can be called from anywhere.
    public static LevelManager main;

    [SerializeField] public Transform[] startPoint;
    [SerializeField] public List<Transform> path1;
    [SerializeField] public List<Transform> path2;
    [SerializeField] public List<Transform> path3;
    [SerializeField] public GameObject CandyPile;

    

    private void Awake()
    {
        main = this;
        //Adding the Candy Pile transform to the end of the list for the AI pathing.
        path1.Add(CandyPile.transform);
        path2.Add(CandyPile.transform);
        path3.Add(CandyPile.transform);
        //path1.AddRange(new Transform[] { targetPoint });
        //path2.AddRange(new Transform[] { targetPoint });

    }
}
