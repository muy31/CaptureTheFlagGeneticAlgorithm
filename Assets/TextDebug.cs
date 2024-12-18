using UnityEngine;
using UnityEngine.UI;

public class TextDebug : MonoBehaviour
{
    public Text text;
    
    public void UpdateText(string newText)
    {
        text.text = newText + "\n";
    }
}
