using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    // Esta función recibe el nombre de la escena a la que quieres viajar
    public void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}