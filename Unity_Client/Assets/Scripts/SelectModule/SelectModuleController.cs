using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectModuleController : MonoBehaviour
{
    private const string FQINTRODUCTION_SCENE = "FQIntroduction";
    private const string BQINTRODUCTION_SCENE = "BQIntroduction";
    private const string LOGIN_SCENE     = "Login";

    public void OnFisicoquimicaClicked()
    {
        SceneManager.LoadScene(FQINTRODUCTION_SCENE);
    }

    public void OnBiocineticaClicked()
    {
        SceneManager.LoadScene(BQINTRODUCTION_SCENE);
    }

    public void OnBackButtonClicked()
    {
        SceneManager.LoadScene(LOGIN_SCENE);
    }
}