using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Dream")]
public class Dream : ScriptableObject
{
    public item[] items;
}

[System.Serializable]
public class item
{
    public GameObject prefab;
    public bool delivered;
    public string name;
}
