using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FileTextSender : MonoBehaviour
{
    public DataSaver saver;
    public Text text;
    public static string wantedFile = string.Empty;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SendDataSaver()
    {
        saver.loadFileName = text.text;
        wantedFile = text.text;
    }
}
