using UnityEngine;

public class EnzymeDisplayIntro : MonoBehaviour
{
    [Header("Contenedores Visuales (Arrastra aquí tus paneles/imágenes)")]
    public GameObject objHoloenzyme;       // Tarjeta 1
    public GameObject objClassification;   // Tarjeta 2
    public GameObject objActivationEnergy; // Tarjeta 3
    public GameObject objMichaelisCurve;   // Tarjeta 4
    public GameObject objEquation;         // Tarjeta 5

    // Apaga todo y enciende solo el estado correcto
    public void SetState(EnzymeState state)
    {
        // 1. Apagamos todo por defecto
        if (objHoloenzyme) objHoloenzyme.SetActive(false);
        if (objClassification) objClassification.SetActive(false);
        if (objActivationEnergy) objActivationEnergy.SetActive(false);
        if (objMichaelisCurve) objMichaelisCurve.SetActive(false);
        if (objEquation) objEquation.SetActive(false);

        // 2. Encendemos el correspondiente
        switch (state)
        {
            case EnzymeState.ShowHoloenzyme:
                if (objHoloenzyme) objHoloenzyme.SetActive(true);
                break;
            case EnzymeState.ShowClassificationList:
                if (objClassification) objClassification.SetActive(true);
                break;
            case EnzymeState.ShowActivationEnergyGraph:
                if (objActivationEnergy) objActivationEnergy.SetActive(true);
                break;
            case EnzymeState.ShowMichaelisCurve:
                if (objMichaelisCurve) objMichaelisCurve.SetActive(true);
                break;
            case EnzymeState.ShowEquation:
                if (objEquation) objEquation.SetActive(true);
                break;
        }
    }
}