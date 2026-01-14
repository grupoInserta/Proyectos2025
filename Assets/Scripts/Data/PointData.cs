
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PointData
{
    public string id;
    public Transform point;
}

public class PointRegistry : MonoBehaviour
{
    public List<PointData> points = new List<PointData>();
}