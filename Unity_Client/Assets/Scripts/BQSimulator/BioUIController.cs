using UnityEngine;
using TMPro; // Necesario para la UI moderna de Unity
using System;

public class UIController : MonoBehaviour
{
    [Header("Inputs del Usuario (Separados por coma)")]
    public TMP_InputField inputSustrato;
    public TMP_InputField inputVelocidad;

    [Header("Outputs (Resultados)")]
    public TMP_Text textVmax;
    public TMP_Text textKm;
    public TMP_Text textR2;
    public TMP_Text textMensajeError;

    public void OnCalcularButtonClicked()
    {
        textMensajeError.text = ""; // Limpiar errores previos

        try
        {
            // 1. Leer y convertir los datos ingresados por el estudiante
            string[] strS = inputSustrato.text.Split(',');
            string[] strV = inputVelocidad.text.Split(',');

            if (strS.Length != strV.Length)
            {
                throw new Exception("Las columnas de Sustrato y Velocidad deben tener la misma cantidad de datos.");
            }

            double[] S = Array.ConvertAll(strS, double.Parse);
            double[] v = Array.ConvertAll(strV, double.Parse);

            // 2. Ejecutar el motor matemático
            double Vmax, Km, rSquared;
            BiocineticsEngine.CalculateParameters(S, v, out Vmax, out Km, out rSquared);

            // 3. Mostrar resultados en la UI
            textVmax.text = "Vmax: " + Vmax.ToString("F4");
            textKm.text = "Km: " + Km.ToString("F4");
            textR2.text = "R²: " + rSquared.ToString("F4");

            // 4. Guardar datos locales (Simulación de la base de datos)
            SimulationData newData = new SimulationData("Alumno_Test", S, v, Vmax, Km, rSquared);
            string jsonOutput = JsonUtility.ToJson(newData, true);
            Debug.Log("Datos guardados localmente:\n" + jsonOutput);

            // 5. Llamada a la actualización de gráficas
            UpdateGraphVisuals(Vmax, Km);
        }
        catch (FormatException)
        {
            textMensajeError.text = "Error: Por favor ingresa solo números separados por comas.";
        }
        catch (Exception e)
        {
            textMensajeError.text = e.Message;
        }
    }

    private void UpdateGraphVisuals(double Vmax, double Km)
    {
        // Aquí se conectaría con el script encargado de mover los puntos en la gráfica 2D
        Debug.Log($"Actualizando gráfica 2D con los nuevos parámetros: Vmax={Vmax}, Km={Km}");
    }
}