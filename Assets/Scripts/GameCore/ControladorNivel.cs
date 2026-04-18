using UnityEngine;
using System.Collections.Generic;

public class ControladorNivel : MonoBehaviour
{
    [Header("Registrador de puntos Zonas Seguras")]
    public List<PointData> securePointList = new List<PointData>();
    public Dictionary<string, Transform> pointLookup;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        BuildLookupTable();// es para hacer una tabla de puntos seguros en el editor Unity
    }

    private void BuildLookupTable()
    {
        pointLookup = new Dictionary<string, Transform>();

        foreach (var p in securePointList)
        {
            Debug.Log("punto seguro" + p.id);
            if (!pointLookup.ContainsKey(p.id))
                pointLookup.Add(p.id, p.point);
        }
    }
}
