using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Text.RegularExpressions; 

public class LoginController : MonoBehaviour
{
    [Header("Campos de Texto")]
    public TMP_InputField inputEmail;
    public TMP_InputField inputPassword;
    // NUEVO: Campo para el código que le darás a la escuela
    public TMP_InputField inputAccessCode; 

    [Header("Feedback Visual")]
    public TextMeshProUGUI txtError;

    private const string SELECT_MODULE_SCENE = "SelectModule";
    private const string MAIN_MENU_SCENE     = "MainMenu";

    // Expresión Regular estricta (8 números, prefijos 2146-2646)
    private const string TEC_EMAIL_PATTERN = @"^2[1-6]46[0-9]{4}@colima\.tecnm\.mx$";

    // NUEVO: Código de acceso ficticio para tu demo/presentación
    private const string HARDCODED_ACCESS_CODE = "TEC-BIO-2026";

    void Start()
    {
        if (txtError != null)
            txtError.gameObject.SetActive(false);
    }

    public void OnIngresarButtonClicked()
    {
        // 1. Validación: Campos vacíos (incluyendo el nuevo código)
        if (string.IsNullOrEmpty(inputEmail.text) || 
            string.IsNullOrEmpty(inputPassword.text) ||
            string.IsNullOrEmpty(inputAccessCode.text))
        {
            MostrarError("Completa todos los campos, incluyendo el código de acceso.");
            return;
        }

        // 2. Validación: Estructura del Correo Tec
        string emailIngresado = inputEmail.text.Trim();
        if (!Regex.IsMatch(emailIngresado, TEC_EMAIL_PATTERN))
        {
            MostrarError("El formato del correo institucional no es válido para esta facultad.");
            return;
        }

        // 3. NUEVO: Validación del código de la facultad
        string codigoIngresado = inputAccessCode.text.Trim();
        if (codigoIngresado != HARDCODED_ACCESS_CODE)
        {
            MostrarError("Código de acceso incorrecto o vencido. Verifica con tu profesor.");
            return;
        }

        // 4. Validación extra rápida: Contraseña mínimo 6 caracteres (Opcional para demo)
        if (inputPassword.text.Length < 6)
        {
            MostrarError("La contraseña debe tener al menos 6 caracteres.");
            return;
        }

        // Si todo está bien, pasa al simulador
        SceneManager.LoadScene(SELECT_MODULE_SCENE);
    }

    public void OnBackButtonClicked()
    {
        SceneManager.LoadScene(MAIN_MENU_SCENE);
    }

    private void MostrarError(string mensaje)
    {
        if (txtError != null)
        {
            txtError.text = mensaje;
            txtError.gameObject.SetActive(true);
        }
    }
}