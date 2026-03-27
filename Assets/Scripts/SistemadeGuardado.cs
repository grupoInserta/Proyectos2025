using UnityEngine;
using System.IO;
using System.Text;

public class SistemadeGuardado
{
   
    public static void GuardarPartida(PlayerHealth jugador, KeyInventory llaves)
    {
        string ubicacion = Application.persistentDataPath + "archivoGuardado";
        var archivo = File.Open(ubicacion, FileMode.Create);
        var escribir = new BinaryWriter(archivo, Encoding.Default, false);

        escribir.Write(jugador.currentHealth);
        //escribir.Write(llaves.);

        escribir.Write(jugador.transform.position.x);
        escribir.Write(jugador.transform.position.y);
        escribir.Write(jugador.transform.position.z);

        escribir.Close();
    }
}
