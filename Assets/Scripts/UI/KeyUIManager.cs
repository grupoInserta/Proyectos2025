using UnityEngine;
using TMPro; // O usar UnityEngine.UI si usas Text normal
using System.Collections.Generic;
using UnityEngine.UI;

public class KeyUIManager : MonoBehaviour
{
    public TextMeshProUGUI keyListText; // Asignar desde el inspector
    public TextMeshProUGUI Aviso; // Avisar pulsar tecla o falta llave
    public Transform iconContainer;
    private Dictionary<string, string> iconosLlaves = new Dictionary<string, string>();
    [Header("Prefab base del icono")]
    public GameObject iconPrefab;
    [SerializeField] private Image fondo;
    private float iconSize = 60f;
    private float incrPosXIcon = 0.1f;
    private float incrPosYIcon = 0.18f;
    private float posTotYIncr = 0.18f;
    private float posTotXIcon = 0.25f;
    private float posTotYIconIni = 0.25f;
    private float posTotYIcon;
    private int contadorFilasIconos = 0;
    private List<string> filasColores = new List<string>();
    // lo siguiente deberia ir en KeyInventory (componente del player)
    private Dictionary<string, List<GameObject>> llavesInventario = new Dictionary<string, List<GameObject>>();
    //public KeyInventory miKeyInventory;

    public void MostrarAviso(string textoAviso)
    {
        Aviso.text = textoAviso;
    }

    private void PosicionarSprite(GameObject _nuevoIcono, float posX, float posY)
    {
        RectTransform fondoRect = fondo.GetComponent<RectTransform>();
        RectTransform iconRect = _nuevoIcono.GetComponent<RectTransform>();

        // Obtener dimensiones actuales del fondo
        float ancho = fondoRect.rect.width;
        float alto = fondoRect.rect.height;

        // Posición relativa (0–1)
        float relX = posX;
        float relY = posY;

        // Calcular posición en píxeles dentro del fondo
        Vector2 localPos = new Vector2(
            (relX - 0.8f) * ancho,
            (relY - 0.5f) * alto
        );// (relY - 0.5f) seria posicionar desde el centro

        // Aplicar posición
        iconRect.anchoredPosition = localPos;
    }

    private void MostrarIcono(string nombreSprite, string _elColor)
    {
        // 1. Cargar sprite desde Resources
        Sprite sprite = Resources.Load<Sprite>($"HUDIcons/{nombreSprite}");
 
        if (sprite == null)
        {
            Debug.LogWarning($"No se encontró el sprite {nombreSprite} en Resources/HUDIcons/");
            return;
        }
        // 2. Instanciar prefab en el HUD
        GameObject nuevoIcono = Instantiate(iconPrefab, fondo.transform, false);
        // agregar objeto visual y su clave a una lista:
      
        if (!llavesInventario.ContainsKey(_elColor))
         {
            llavesInventario[_elColor] = new List<GameObject>(); // crea la lista si no existe
         }
        llavesInventario[_elColor].Add(nuevoIcono); // agrega el objeto a la lista del diccionario

        // 3. Asignar el sprite al componente Image
        Image img = nuevoIcono.GetComponent<Image>();
        img.sprite = sprite;
        img.preserveAspect = true;
        //
        Vector2 size = img.rectTransform.sizeDelta;
        float maxSize = iconSize; // tamaño del icono de llave

        if (size.x > maxSize || size.y > maxSize)
        {
            float scale = maxSize / Mathf.Max(size.x, size.y);
            img.rectTransform.sizeDelta *= scale;
        }
        // 4. (Opcional) Ajustar visibilidad o animación
        nuevoIcono.SetActive(true);
        if (!filasColores.Contains(_elColor))
        {
            contadorFilasIconos++;
            filasColores.Add(_elColor);
        }
        int posicionColor = filasColores.IndexOf(_elColor);
        Debug.Log("POSICION COLORRR: " + posicionColor);
        posTotYIcon = posTotYIconIni - posTotYIncr * posicionColor;
        // posicionamiento:
        PosicionarSprite(nuevoIcono, posTotXIcon, posTotYIcon);
    }


    public string EliminarIcono(string _elColor)
    {      string resultado = "";
        if (llavesInventario.TryGetValue(_elColor, out var lista) && lista.Count > 0)
        {                     
            // Obtener el último icono
            GameObject ultimo = lista[lista.Count - 1];
            // Eliminarlo de la lista

            lista.RemoveAt(lista.Count - 1);
            // Destruirlo en la escena
            if (ultimo != null)
                Destroy(ultimo); 

            // Si la lista quedó vacía, remover la clave
            if (lista.Count == 0)
            {
                llavesInventario.Remove(_elColor);
                Debug.Log($"Icono '{_elColor}' eliminado y destruido");
                resultado = _elColor;
            }
        }
        else
        {
            Debug.LogWarning($"No se encontró ningún icono con la clave '{_elColor}'");
        }
        return resultado;
    }


    

    public void EliminarTodosLosIconos()
    {
        // Copia para evitar modificar el diccionario mientras lo recorres
        var claves = new List<string>(llavesInventario.Keys);

        foreach (var color in claves)
        {
            // Mientras existan iconos en esa lista, elimínalos
            while (llavesInventario.ContainsKey(color))
            {
                EliminarIcono(color);
            }
        }
    }


    private void Start()
    {
        keyListText.text = "Llaves:\n";        
        iconosLlaves.Add("verde", "circle-button-green");
        iconosLlaves.Add("roja", "circle-button-red");
        iconosLlaves.Add("azul", "circle-button-blue");        
    }

    public void UpdateKeyList(string[] keys, string clave)
    {
        keyListText.text = "Llaves:\n";

        foreach (string key in keys)
        {            
            string[] arrNombre = key.Split('-');
            string elColor = arrNombre[0];
            
            if(arrNombre[0] == clave)
            {
                if(elColor != "")
                MostrarIcono(iconosLlaves[elColor], elColor);
            }            
            
        }
    }
}
