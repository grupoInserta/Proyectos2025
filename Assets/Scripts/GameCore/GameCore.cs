using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.Universal;

public class GameCore : MonoBehaviour
{
    // ------------------------------------------------------------
    // SINGLETON
    // ------------------------------------------------------------

    /*  utilización de métodos desde el resto del componentes:
     Cambiar estado:
    GameCore.Instance.SetGameState(GameCore.GameState.Inventory);

    Registrar enemigo:
    GameCore.Instance.RegisterEnemy(this.gameObject); 
     */
    public static GameCore Instance { get; private set; }

    public GameObject Jugador;
    public GameObject Enemigo;
    //
    public Button botonGuardar;
    public Button botonCargar;
    //
    private GameObject PanelInicio;
    // controles de ajuste de color pantalla
    public Volume globalVolume;
    private ColorAdjustments colorAdjustments;
    public float valorSaturacion = 0;
    public float valorContraste = 0;
    private ControladorNivel controladorNivel;
    private GameCoreUI gameCoreUI;
    //
    private AudioSource audioSource;
    public AudioClip ClicSound;
    private  BackgroundMusic BGM;


    private void Awake()
    {// importante e lo sigiente porque al volver a la escena se crea otra instancia...
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);        
        SceneManager.sceneLoaded += OnSceneLoaded;
        BGM = GetComponent<BackgroundMusic>();
    }

    private void OnSceneLoaded(Scene escena, LoadSceneMode modo)
    {
        audioSource = transform.GetChild(0).GetComponent<AudioSource>();
        // Si estamos en una escena donde no existen jugador/enemigo, NO fallará
        if (escena.name != "MenuPrincipal" && escena.name != "Derrota" && escena.name != "Victoria")
        {
            BuscarReferencias(escena.name);            
            //
            Button[] botones = Resources.FindObjectsOfTypeAll<Button>();

            foreach (Button b in botones)
            {
                if (b.name == "botonGuardar")
                    botonGuardar = b;

                if (b.name == "botonCargar")
                    botonCargar = b;
            }

            // Reconectar eventos
            if (botonGuardar != null)
            {
                if (!SistemadeGuardado.comprobarHayGuardado())
                {
                    botonGuardar.gameObject.SetActive(false);
                }
                botonGuardar.onClick.RemoveAllListeners();
                botonGuardar.onClick.AddListener(GuardarPartida);
            }

            if (botonCargar != null)
            {
                botonCargar.onClick.RemoveAllListeners();
                botonCargar.onClick.AddListener(CargarPartida);
              
            }
            InitializeCoreSystems();
        }

       if (escena.name != "MenuPrincipal")
       {            
            if (globalVolume != null)
            {
                    globalVolume.profile.TryGet<ColorAdjustments>(out colorAdjustments);
                    colorAdjustments.saturation.value = valorSaturacion;
                    colorAdjustments.contrast.value = valorContraste;
            }
            if (escena.name == "Derrota" || escena.name == "Victoria")
            {
                Debug.Log("ARRREGLAR CURSOR");
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
       }
        BGM.Reproducir(escena.name);
    }

    void BuscarReferencias(string nombreEscena)
    {
        if ( nombreEscena == "Nivel")
        {
            // Solo los busca si no los tiene
            if (Jugador == null)
            {
                Jugador = GameObject.FindWithTag("Player");
                playerHealth = Jugador.GetComponent<PlayerHealth>();
            }

            if (Enemigo == null)
            {
                Enemigo = GameObject.FindWithTag("Enemy");
            }
            if (controladorNivel == null)
            {
                controladorNivel = GameObject.FindWithTag("Controlador").GetComponent<ControladorNivel>();
            }
            if (PanelInicio == null)
            {
                PanelInicio = GameObject.FindWithTag("PanelInicio");
            }
            
            if (gameCoreUI == null)
            {
                gameCoreUI = GameObject.FindWithTag("HUDGame").GetComponent<GameCoreUI>();
            }
            
        }
        
        globalVolume = BuscarGlobalVolume();
 
        if (globalVolume != null)
            Debug.Log($"Global Volume encontrado");
        else
            Debug.LogWarning("No se encontró Global Volume en esta escena");
    }
    /// //////
    Volume BuscarGlobalVolume()
    {
        // 1. Buscar objetos activos normalmente
        var vol = GameObject.FindObjectOfType<Volume>();
        if (vol != null) return vol;

        // 2. Si no lo encuentra, buscar objetos desactivados
        var todos = Resources.FindObjectsOfTypeAll<Volume>();
        foreach (var v in todos)
        {
            if (v.gameObject.hideFlags == HideFlags.None)
                return v; // primer Volume válido encontrado
        }

        return null; // no existe en esta escena
    }

    
    private Transform GetPoint(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;
        id = id.Trim();
        return controladorNivel.pointLookup.TryGetValue(id, out Transform result)
            ? result
            : null;
    }

    public void RegisterPoint(string id, Transform tf)
    {
        controladorNivel.pointLookup[id] = tf;
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

    //private HashSet<string> keysCollected = new HashSet<string>();
    private HashSet<string> doorsOpened = new HashSet<string>();
    // se guarda el nivel y la posicion
    public Dictionary<string, Transform> zonasSeguras = new Dictionary<string, Transform>();
    private string nombresPuertas = "";

    
    /*****************************************************
    Cada vez que se abra una puerta hay que registrarla en GameCore.
    Hay que implementar una coleccion de puertas...

     ***********************************************/
    public void RegisterDoorOpen(string doorID)
    {
        if (doorsOpened.Add(doorID))
            Debug.Log($"[GameCore] Puerta abierta: {doorID}");
    }

    public bool IsDoorOpen(string doorID) => doorsOpened.Contains(doorID);


    // ------------------------------------------------------------
    // CHECKPOINTS NO UTILIZADOS POR AHORA, SON COMO CONTROLES DEL JUEGO
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
        Debug.Log("ConnectPlayerHealth Player Health");
        playerHealth = Jugador.GetComponent<PlayerHealth>();
        if (playerHealth == null)  return;
        //Jugador = playerHealth.gameObject;
        playerHealth.OnPlayerDamaged += OnPlayerDamage;
        playerHealth.OnPlayerDeath += OnPlayerDied;
    }

    private void OnPlayerDamage(int newHealth)
    {
        Debug.Log($"[GameCore] Salud del jugador actualizada: {newHealth}");
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
        if (objeto.transform.position.y > 8f && objeto.transform.position.y < 13f)
        {
            Nivel = "1";
        }
        else if (objeto.transform.position.y > 6.5f && objeto.transform.position.y <= 8f)
        {
            Nivel = "2";
        }
        else if (objeto.transform.position.y < 7f)
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
    public void GuardarPartida()
    {
        audioSource.PlayOneShot(ClicSound);
        foreach (string nombre in doorsOpened)
        {
            nombresPuertas += nombre;            
        }
        SistemadeGuardado.GuardarPartida(playerHealth, Enemigo, nombresPuertas);
    }

    public void CargarPartida()
    {
        audioSource.PlayOneShot(ClicSound);
        SistemadeGuardado.CargarPartida(playerHealth, Enemigo);
        PanelInicio.SetActive(false);
        gameCoreUI.CerrarPanelInicio();
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
        Debug.Log("START GAME");
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

}
