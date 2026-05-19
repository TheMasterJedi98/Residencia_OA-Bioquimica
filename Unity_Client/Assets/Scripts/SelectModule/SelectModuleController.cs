using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectModuleController : MonoBehaviour
{
    private const string FQINTRODUCTION_SCENE = "FQIntroduction";
    private const string LOGIN_SCENE     = "Login";

    public void OnFisicoquimicaClicked()
    {
        SceneManager.LoadScene(FQINTRODUCTION_SCENE);
    }

    public void OnBiocineticaClicked()
    {
        
        Debug.Log("Biocinetica: aún no implementado");
    }

    public void OnBackButtonClicked()
    {
        SceneManager.LoadScene(LOGIN_SCENE);
    }
}