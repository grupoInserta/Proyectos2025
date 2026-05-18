using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class SistemadeGuardado
{
    public static void BorrarPartida()
    {
        string ubicacion = Application.persistentDataPath + "archivoGuardado";
        if (File.Exists(ubicacion))
        {
            File.Delete(ubicacion);
        }
    }
    public static bool comprobarHayGuardado()
    {
        string ubicacion = Application.persistentDataPath + "archivoGuardado";
        bool hayGuardado = false;
        if (File.Exists(ubicacion))
        {
            hayGuardado = true;
        }
        return hayGuardado;
    }
    public static void GuardarPartida(PlayerHealth jugador, GameObject enemigo, string PuertasAbiertas)  //, KeyInventory llaves)
    {
        Debug.Log("PUERTAS ABIERTAS: " + PuertasAbiertas);
        string ubicacion = Application.persistentDataPath + "archivoGuardado";// carpeta y nombre archivo
        var archivo = File.Open(ubicacion, FileMode.Create);
        var escribir = new BinaryWriter(archivo, Encoding.Default, false);// false para que el archivo no se mantenga abierto
        /*
         Solo se guardan numeros enteros, decimales, booleanos, cadenas
         * */    
        escribir.Write(jugador.currentHealth);
        escribir.Write(jugador.transform.position.x);
        escribir.Write(jugador.transform.position.y);
        escribir.Write(jugador.transform.position.z);
        //
        escribir.Write(enemigo.transform.position.x);
        escribir.Write(enemigo.transform.position.y);
        escribir.Write(enemigo.transform.position.z);
        // PUERTAS:
        escribir.Write(PuertasAbiertas);
        // Llaves:
        HashSet<string> llavesInventario = jugador.GetComponent<KeyInventory>().keys;
        //
        string ListaLlaves  = "";
        foreach (string llave in llavesInventario)
        {
            ListaLlaves += llave + "#";           
        }
        if(ListaLlaves.Length > 0)
            ListaLlaves = ListaLlaves.Substring(0, ListaLlaves.Length - 1);
        //
        escribir.Write(ListaLlaves);
        //BinaryWriter.Write(string))
        escribir.Close();
    }

    public static void CargarPartida(PlayerHealth jugador, GameObject enemigo) //, keyinventory llaves) 
    {
        KeyInventory Inventariollaves = jugador.GetComponent<KeyInventory>();

        string ubicacion = Application.persistentDataPath + "archivoGuardado";

        if(File.Exists(ubicacion))
        {
            var archivo = File.Open(ubicacion, FileMode.Open);
            var lectura = new BinaryReader(archivo, Encoding.Default, false); // codificación
            jugador.currentHealth = lectura.ReadInt32();
            jugador.NotifyHealthChange();
            Debug.Log("salud guardada:"+jugador.currentHealth);  //OK         
           
            Vector3 pos;

            pos.x = lectura.ReadSingle();
            pos.y = lectura.ReadSingle();
            pos.z = lectura.ReadSingle();

            jugador.transform.position = pos;
            // enemigo:
            Vector3 posE;
            posE.x = lectura.ReadSingle();
            posE.y = lectura.ReadSingle();
            posE.z = lectura.ReadSingle();

            enemigo.transform.position = posE;
            //
            //puertas:
            //obtenerlas
            string puertasAbiertas = lectura.ReadString();
            Debug.Log("PUERTAS ABIERTAS: " + puertasAbiertas);
            char[] puertas = puertasAbiertas.ToCharArray();
            foreach (char puerta in puertas)
            {
                string Puerta = puerta.ToString();
                GameObject go = GameObject.Find(Puerta);

                if (go != null)
                {
                    Debug.Log("Encontrado: " + go.name);
                    go.GetComponent<Door>().CargarPosicionPuerta();//abierta
                }
                else
                {
                    Debug.LogWarning("No existe un GameObject llamado: " + Puerta);
                }
            }
            //llaves.
            var cadena = lectura.ReadString();
            Debug.Log("LLAVES GUARDADAS: "+cadena);
            //
            string[] nombresArray = cadena.Split('#');

            // borrar las llaves previas:
            Inventariollaves.vaciarInventario();
            foreach (string nombre in nombresArray)
            {
                Inventariollaves.AddKey(nombre);               
            }
            //
            archivo.Close();
        }
        else
        {
            Debug.Log("No se encuentra el archivo de guardaddo");
        }
    }
}
