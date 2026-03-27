using UnityEngine;
using System.IO;
using System.Text;

public class SistemadeGuardado
{
   
    public static void GuardarPartida(PlayerHealth jugador)  //, KeyInventory llaves)
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

    public static void CargarPartida(PlayerHealth jugador) //, keyinventory llaves) 
    {
        string ubicacion = Application.persistentDataPath + "archivoGuardado";

        if(File.Exists(ubicacion))
        {
            var archivo = File.Open(ubicacion, FileMode.Open);
            var lectura = new BinaryReader(archivo, Encoding.Default, false);


            jugador.currentHealth = lectura.ReadInt32();

            //llaves.

            Vector3 pos;

            pos.x = lectura.ReadSingle();
            pos.y = lectura.ReadSingle();
            pos.z = lectura.ReadSingle();

            jugador.transform.position = pos;

            archivo.Close();
        }
        else
        {

        }
    }
}
