using System.Collections.Generic;
using UnityEngine;

public class KeyInventory : MonoBehaviour
{   /*las claves, keys, estan tambien en KeyUIManager pero las necesito tambien
     en este script para que se compruebe desde las llaves y puertas hasta aqui si existe esa clave
   */
    public HashSet<string> keys = new HashSet<string>();// son las llaves recogidas, no las totales
    public KeyUIManager uiManager; // Asignar desde el inspector
    public GameObject[] Llaves; // todas

    private void Start()
    {
        Llaves = GameObject.FindGameObjectsWithTag("Key");
    }
    // no comprueba si existe la llave antes de añadirla pues al inicio del proyecto no sabia si una llave se podia repetir y ser desechable o no..
    public bool AddKey(string _keyName) //AÑADIR UNA LLAVE AL INVENTARIO
    {
        if (keys.Contains(_keyName))
            return false;
        keys.Add(_keyName);
        Debug.Log("Llave añadida: " + _keyName);
        // elimino todas lasllaves de ese color que esten en la escena:
        foreach (GameObject llave in Llaves)
        {            
            if(llave != null)
            {
                if(llave.GetComponent<KeyPickup>().keyName == _keyName) 
                    Destroy(llave);
            }               
        }
        //
        uiManager?.UpdateKeyList(GetKeysArray(), _keyName);
        return true;      
    }

    public void vaciarInventario()
    {
        uiManager.EliminarTodosLosIconos();
        keys.Clear();
    }

    public void RemoveKey(string keyName)
    {
        string claveEliminada =  uiManager.EliminarIcono(keyName);
        if(claveEliminada != "")
        {
            string claveAEliminar = claveEliminada;

            if (keys.Remove(claveAEliminar))
            {
                Debug.Log($"Clave '{claveAEliminar}' eliminada correctamente.");
            }
            else
            {
                Debug.LogWarning($"La clave '{claveAEliminar}' no se encontró en el conjunto.");
            }
        }
        // si se agotan los scripts de esa clave hay que eliminar aqui ese elemento de la ashSet keys!!!!!!!!!!!!!
    }

    public void MostrarAviso(string textoAviso)
    {
        uiManager?.MostrarAviso(textoAviso);
    }


    public bool HasKey(string keyName)
    {  // pasar a nombre de color !!!!!!
        return keys.Contains(keyName);
    }
 

    public string[] GetKeysArray()
    {
        var array = new string[keys.Count];
        keys.CopyTo(array);
        return array;
    }
}
