using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    //Static means it can be called from anywhere.
    public static LevelManager main;

    public Transform targetPoint;

    private void Awake()
    {
        main = this;

    }
}
