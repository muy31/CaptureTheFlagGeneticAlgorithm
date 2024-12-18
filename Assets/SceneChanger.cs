using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
    public void NextScene()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("SampleScene"))
        {
            SceneManager.LoadScene("TrainAgainstAgent");
        }
        else if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("TrainAgainstAgent"))
        {
            SceneManager.LoadScene("TestScene");
        }
        else
        {
            SceneManager.LoadScene("SampleScene");
        }
        
    }   
}
