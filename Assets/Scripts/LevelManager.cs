using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    //Static means it can be called from anywhere.
    public static LevelManager main;

    public Transform startPoint;
    public Transform[] path;
    public Transform targetPoint;

    public int currency;

    private void Awake()
    {
        main = this;

    }

    private void Start()
    {
        //Game starts with 100
        currency = 100;
    }

    public void GainMoney(int cash)
    {
        //Called in the EnemyCtrl to gain Money
        currency += cash;
    }

    //This is where the UI comes in
    public bool SpendMoney(int cash)
    {
        if(cash <= currency)
        {
            //Buy Torrent
            currency -= cash;
            return true;
        } else
        {
            Debug.Log("Not enough cash");
            return false;
        }
    }
}
