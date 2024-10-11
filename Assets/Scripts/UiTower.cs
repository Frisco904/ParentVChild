using System;
using UnityEngine;

[Serializable]
public class UiTower
{
    public string name;
    public int cost;
    public GameObject prefab;

    public UiTower(string _name, int _cost, GameObject _prefab)
    {
        name = _name;
        cost = _cost;
        prefab = _prefab;
    }
}
