using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class DataSaver : MonoBehaviour
{
    public string loadFileName;

    public void SaveFile(Brain br)
    {
        string filename = "/save" + DateTime.Now.Second + ".dat";
        Debug.Log("Attempting to save network to " +Application.persistentDataPath+ filename);
        string destination = Application.persistentDataPath + filename;
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, br);
        file.Close();
    }

    public Brain LoadFile()
    {
        string destination = Application.persistentDataPath + "/" + loadFileName;
        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            Debug.LogError("File not found");
            return null;
        }

        BinaryFormatter bf = new BinaryFormatter();
        Brain data = (Brain) bf.Deserialize(file);
        file.Close();
        return data;
    }

}