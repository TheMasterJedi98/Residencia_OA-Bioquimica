using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class EnzymeIntroController : MonoBehaviour
{
    [Header("UI - Navegación")]
    public TextMeshProUGUI txtTitle;
    public TextMeshProUGUI txtContent;
    public TextMeshProUGUI txtPageCounter;
    public Button btnAnterior;
    public Button btnSiguiente;

    [Header("UI - Barra de progreso")]
    public List<Image> progressSegments;

    [Header("UI - Ícono")]
    public Image iconImage;
    public List<Sprite> cardIcons;

    [Header("Animación Visual")]
    public EnzymeDisplayIntro enzymeDisplay; 

    [Header("Escena siguiente")]
    private const string NEXT_SCENE = "BQSimulator"; 

    private int currentCard = 0;
    
    // --- CAMBIO APLICADO AQUÍ: Usamos EnzymeCardData ---
    private List<EnzymeCardData> cards = new List<EnzymeCardData>();

    void Start()
    {
        InicializarTarjetas();
        MostrarTarjeta(0);
    }

    void InicializarTarjetas()
    {
        // TARJETA 1
        // --- CAMBIO APLICADO AQUÍ ---
        cards.Add(new EnzymeCardData(
            "¿Qué es una enzima?",
            "Las enzimas son los catalizadores de las reacciones en sistemas biológicos. Aumentan considerablemente la velocidad de las reacciones sin consumirse en el proceso.\n\n" +
            "A su parte proteica se le llama <b>Apoenzima</b>. Si requiere un grupo inorgánico adicional (como Fe<sup>2+</sup>) se le llama <b>Cofactor</b>; si es orgánico, <b>Coenzima</b>. La enzima completa lista para actuar se conoce como <b>Holoenzima</b>.",
            EnzymeState.ShowHoloenzyme
        ));

        // TARJETA 2
        cards.Add(new EnzymeCardData(
            "Clasificación de enzimas",
            "• <b>Oxidorreductasas:</b> Transferencia de electrones.\n" +
            "• <b>Transferasas:</b> Reacciones de transferencia de grupos.\n" +
            "• <b>Hidrolasas:</b> Reacciones de hidrólisis.\n" +
            "• <b>Liasas:</b> Adición o eliminación de grupos a dobles enlaces.\n" +
            "• <b>Isomerasas:</b> Transferencia de grupos dentro de moléculas (isómeros).\n" +
            "• <b>Ligasas:</b> Formación de enlaces (C-C, C-S, C-O) acopladas a rotura de ATP.",
            EnzymeState.ShowClassificationList
        ));

        // TARJETA 3
        cards.Add(new EnzymeCardData(
            "Mecanismo de reacción",
            "Las enzimas aumentan la velocidad de la reacción al <b>disminuir la energía de activación</b> necesaria para que se lleve a cabo el proceso.\n\n" +
            "Importante: Las enzimas aceleran el camino hacia los productos, pero <b>no modifican el equilibrio químico final</b> de la reacción.",
            EnzymeState.ShowActivationEnergyGraph
        ));

        // TARJETA 4
        cards.Add(new EnzymeCardData(
            "Cinética y Concentración",
            "Al graficar la velocidad en función de la concentración de sustrato, se observa un aumento lineal al principio, que va disminuyendo a medida que aumenta la concentración.\n\n" +
            "Cuando la concentración es muy alta, la enzima se satura y la velocidad alcanza una meseta: es decir, la <b>velocidad máxima (V<sub>max</sub>)</b> de la reacción.",
            EnzymeState.ShowMichaelisCurve
        ));

        // TARJETA 5
        cards.Add(new EnzymeCardData(
            "Ecuación de Michaelis-Menten",
            "Este comportamiento se modela matemáticamente así:\n\n" +
            "<b>v = (V<sub>max</sub> · [S]) / (K<sub>S</sub> + [S])</b>\n\n" +
            "Tiene forma sigmoidal con una asíntota horizontal (V<sub>max</sub>). El valor de la mitad de la velocidad máxima se alcanza cuando la concentración de sustrato [S] es igual a la constante de saturación (K<sub>S</sub>).",
            EnzymeState.ShowEquation
        ));
    }

    void MostrarTarjeta(int index)
    {
        // --- CAMBIO APLICADO AQUÍ ---
        EnzymeCardData card = cards[index];

        txtTitle.text = card.title;
        txtContent.text = card.content;
        txtPageCounter.text = $"{index + 1} de {cards.Count}";

        if (cardIcons != null && index < cardIcons.Count && cardIcons[index] != null)
            iconImage.sprite = cardIcons[index];

        for (int i = 0; i < progressSegments.Count; i++)
        {
            progressSegments[i].color = i <= index
                ? new Color(0.18f, 0.47f, 0.87f)   
                : new Color(0.3f, 0.3f, 0.3f);      
        }

        btnAnterior.interactable = index > 0;
        Color anteriorColor = index > 0
            ? Color.white
            : new Color(1, 1, 1, 0.3f);
        btnAnterior.GetComponentInChildren<TextMeshProUGUI>().color = anteriorColor;

        TextMeshProUGUI txtBtn = btnSiguiente.GetComponentInChildren<TextMeshProUGUI>();
        txtBtn.text = index == cards.Count - 1 ? "Comenzar →" : "Siguiente →";

        if (enzymeDisplay != null)
            enzymeDisplay.SetState(card.enzymeState);
    }

    public void OnAnteriorClicked()
    {
        if (currentCard > 0)
        {
            currentCard--;
            MostrarTarjeta(currentCard);
        }
    }

    public void OnSiguienteClicked()
    {
        if (currentCard < cards.Count - 1)
        {
            currentCard++;
            MostrarTarjeta(currentCard);
        }
        else
        {
            SceneManager.LoadScene(NEXT_SCENE);
        }
    }
}

public enum EnzymeState
{
    ShowHoloenzyme,           
    ShowClassificationList,   
    ShowActivationEnergyGraph,
    ShowMichaelisCurve,       
    ShowEquation              
}

// --- CAMBIO APLICADO AQUÍ: Clase renombrada y constructor actualizado ---
public class EnzymeCardData
{
    public string title;
    public string content;
    public EnzymeState enzymeState;

    public EnzymeCardData(string title, string content, EnzymeState enzymeState)
    {
        this.title = title;
        this.content = content;
        this.enzymeState = enzymeState;
    }
}