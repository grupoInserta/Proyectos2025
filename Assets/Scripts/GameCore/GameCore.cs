using System;
using System.Collections.Generic;
using UnityEngine;

public class GameCore : MonoBehaviour
{
    // ------------------------------------------------------------
    // SINGLETON
    // ------------------------------------------------------------

    /*  utilización de métodos desde el resto del componentes:
     Cambiar estado:
    GameCore.Instance.SetGameState(GameCore.GameState.Inventory);

    Registrar llave:
    GameCore.Instance.RegisterKey("KeyRoja");

    Registrar puerta abierta:
    GameCore.Instance.RegisterDoorOpen("PuertaPrincipal");

    Añadir item al inventario:
    GameCore.Instance.AddItem("MunicionEscopeta");

    Registrar enemigo:
    GameCore.Instance.RegisterEnemy(this.gameObject); 
     */
    public static GameCore Instance { get; private set; }

    [Header("Registrador de puntos Zonas Seguras")]
    public List<PointData> securePointList = new List<PointData>();
    private Dictionary<string, Transform> pointLookup;

    private GameObject Jugador;
    [SerializeField]
    private GameObject Enemigo;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        BuildLookupTable();
        InitializeCoreSystems();
    }

    private void BuildLookupTable()
    {
        pointLookup = new Dictionary<string, Transform>();

        foreach (var p in securePointList)
        {
            Debug.Log("punto seguro");
            if (!pointLookup.ContainsKey(p.id))
                pointLookup.Add(p.id, p.point);
        }
    }
    public Transform GetPoint(string id)
    {
        return pointLookup.TryGetValue(id, out Transform result)
            ? result
            : null;
    }

    public void RegisterPoint(string id, Transform tf)
    {
        pointLookup[id] = tf;
    }


    // ------------------------------------------------------------
    // ESTADOS DEL JUEGO
    // ------------------------------------------------------------

    public enum GameState
    {
        Loading,
        Gameplay,
        Paused,
        Inventory,
        //Cutscene,//cinematica 
        GameOver
    }

    public GameState CurrentState { get; private set; } = GameState.Loading;
    public event Action<GameState> OnGameStateChanged;

    public void SetGameState(GameState newState)
    {
        CurrentState = newState;
        Debug.Log($"[GameCore] Estado cambiado a: {newState}");
        OnGameStateChanged?.Invoke(newState);
    }


    // ------------------------------------------------------------
    // PROGRESO DEL JUGADOR (llaves, puertas, puzzles)
    // ------------------------------------------------------------

    private HashSet<string> keysCollected = new HashSet<string>();
    private HashSet<string> doorsOpened = new HashSet<string>();
    // se guarda el nivel y la posicion
    public Dictionary<string, Transform> zonasSeguras = new Dictionary<string, Transform>();

    public void RegisterKey(string keyID)
    {
        if (keysCollected.Add(keyID))
            Debug.Log($"[GameCore] Llave recogida: {keyID}");
    }

    public bool HasKey(string keyID) => keysCollected.Contains(keyID);

    public void RegisterDoorOpen(string doorID)
    {
        if (doorsOpened.Add(doorID))
            Debug.Log($"[GameCore] Puerta abierta: {doorID}");
    }

    public bool IsDoorOpen(string doorID) => doorsOpened.Contains(doorID);


    // ------------------------------------------------------------
    // CHECKPOINTS
    // ------------------------------------------------------------
    public Vector3 lastCheckpointPosition = Vector3.zero;

    public void SetCheckpoint(Vector3 pos)
    {
        lastCheckpointPosition = pos;
        Debug.Log($"[GameCore] Checkpoint actualizado: {pos}");
    }

    // ------------------------------------------------------------
    // ENEMIGOS REGISTRADOS GLOBALMENTE
    // ------------------------------------------------------------
    private List<GameObject> activeEnemies = new List<GameObject>();

    public void RegisterEnemy(GameObject enemy)
    {
        if (!activeEnemies.Contains(enemy))
            activeEnemies.Add(enemy);
    }

    public void UnregisterEnemy(GameObject enemy)
    {
        activeEnemies.Remove(enemy);
    }

    public IEnumerable<GameObject> GetAllEnemies() => activeEnemies;


    // ------------------------------------------------------------
    // INVENTARIO SIMPLE
    // ------------------------------------------------------------
    private List<string> inventoryItems = new List<string>();

    public void AddItem(string itemID)
    {
        inventoryItems.Add(itemID);
        Debug.Log($"[GameCore] Item añadido: {itemID}");
    }

    public bool HasItem(string itemID) => inventoryItems.Contains(itemID);


    // ------------------------------------------------------------
    // SISTEMA DE SALUD JUGADOR (se conecta externamente)
    // ------------------------------------------------------------
    public PlayerHealth playerHealth;

    private void ConnectPlayerHealth()
    {
        if (playerHealth == null)  return;
        Jugador = playerHealth.gameObject;

        playerHealth.OnPlayerDamaged += OnPlayerDamage;
        playerHealth.OnPlayerDeath += OnPlayerDied;
    }

    private void OnPlayerDamage(int newHealth)
    {
        Debug.Log($"[GameCore] Salud del jugador actualizada: {newHealth}");
        // comunicar a HUD ++++++++++++++++++++++++++++++++++++++++++++++++++++
        mandarZonaSegura();
    }

    private void OnPlayerDied()
    {
        Debug.Log("[GameCore] Jugador muerto.");
        SetGameState(GameState.GameOver);
    }

    public string obtenerNivel(string quien)
    {
        GameObject objeto;
        if(quien == "jugador")
        {
            objeto = Jugador;
        }
        else
        {
            objeto = Enemigo;
        }
        string Nivel = "0";
        if (Jugador.transform.position.y > 8f && Jugador.transform.position.y < 12f)
        {
            Nivel = "1";
        }
        else if (Jugador.transform.position.y > 7f && Jugador.transform.position.y <= 8f)
        {
            Nivel = "2";
        }
        else if ( Jugador.transform.position.y <= 7f)
        {
            Nivel = "3";
        }
        return Nivel;
    }

    private void mandarZonaSegura() {
        string elNivel = obtenerNivel("jugador");
        Jugador.transform.position = GetPoint(elNivel).position;        
    }


    // ------------------------------------------------------------
    // GUARDADO / CARGA (placeholder)
    // ------------------------------------------------------------
    public void SaveGame()
    {
        Debug.Log("[GameCore] Guardado ejecutado.");
    }

    public void LoadGame()
    {
        Debug.Log("[GameCore] Carga ejecutada.");
    }


    // ------------------------------------------------------------
    // INICIALIZACIÓN GENERAL
    // ------------------------------------------------------------
    private void InitializeCoreSystems()
    {
        Debug.Log("[GameCore] Sistemas centrales inicializados.");

        // Inicia en Loading y pasa a Gameplay al segundo
        SetGameState(GameState.Loading);

        Invoke(nameof(StartGame), 1f);
    }

    private void StartGame()
    {
        SetGameState(GameState.Gameplay);
        ConnectPlayerHealth();     
    }


    // ------------------------------------------------------------
    // MÉTODOS DE CONTROL EXTERNOS (pausa, inventario, etc.)
    // ------------------------------------------------------------
    public void TogglePause()
    {
        if (CurrentState == GameState.Gameplay)
            SetGameState(GameState.Paused);
        else if (CurrentState == GameState.Paused)
            SetGameState(GameState.Gameplay);
    }

    public void OpenInventory()
    {
        if (CurrentState == GameState.Gameplay)
            SetGameState(GameState.Inventory);
    }

    public void CloseInventory()
    {
        if (CurrentState == GameState.Inventory)
            SetGameState(GameState.Gameplay);
    }
}
