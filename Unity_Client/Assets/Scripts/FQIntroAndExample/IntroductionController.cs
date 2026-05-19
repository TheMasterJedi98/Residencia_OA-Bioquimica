using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class IntroductionController : MonoBehaviour
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

    [Header("Átomo animado")]
    public AtomDisplayIntro atomDisplay;

    [Header("Escena siguiente")]
    private const string NEXT_SCENE = "FQExample";

    private int currentCard = 0;

    private List<CardData> cards = new List<CardData>();

    void Start()
    {
        InicializarTarjetas();
        MostrarTarjeta(0);
    }

    void InicializarTarjetas()
    {
        cards.Add(new CardData(
            "¿Qué es un átomo?",
            "Los átomos son las unidades fundamentales de la materia. " +
            "Todo lo que nos rodea está compuesto por átomos: el aire que respiramos, " +
            "el agua que bebemos, e incluso nuestro propio cuerpo.\n\n" +
            "Es la unidad mínima de una sustancia que mantiene sus propiedades químicas. " +
            "Aunque antiguamente se pensaba que era indivisible, hoy sabemos que está formado " +
            "por partículas aún más pequeñas: protones, neutrones y electrones.",
            AtomState.Default
        ));

        cards.Add(new CardData(
            "El núcleo y la corteza",
            "<b>El Núcleo:</b> Es la región central, muy pequeña y densa, donde se concentra " +
            "casi toda la masa del átomo. Contiene a los <b>protones</b> (carga positiva) y los " +
            "<b>neutrones</b> (sin carga), denominados en conjunto nucleones.\n\n" +
            "<b>La Corteza o Nube Electrónica:</b> Es la zona exterior que rodea al núcleo, " +
            "compuesta por <b>electrones</b> (carga negativa) que se distribuyen en " +
            "diferentes niveles de energía.",
            AtomState.ShowNucleusAndElectrons
        ));

        cards.Add(new CardData(
            "Modelos atómicos",
            "<b>Dalton (1808):</b> La materia está formada por átomos indivisibles.\n\n" +
            "<b>Thomson (1898):</b> El átomo es una esfera de carga positiva con electrones incrustados.\n\n" +
            "<b>Rutherford (1911):</b> El átomo está mayormente vacío, con un núcleo central positivo.\n\n" +
            "<b>Bohr (1913):</b> Los electrones giran en órbitas o niveles de energía fijos.\n\n" +
            "<b>Modelo Mecanocuántico:</b> Los electrones se describen como nubes de probabilidad.",
            AtomState.Default
        ));

        cards.Add(new CardData(
            "Identificación atómica",
            "<b>Número Atómico (Z):</b> Indica la cantidad de protones en el núcleo. " +
            "Define la identidad del elemento químico. En un átomo neutro, el número de " +
            "protones es igual al de electrones.\n\n" +
            "<b>Número Másico (A):</b> Es la suma total de protones y neutrones en el núcleo.\n\n" +
            "<b>Isótopos:</b> Son átomos del mismo elemento (igual Z) que poseen un número " +
            "diferente de neutrones y, por lo tanto, distinto número másico (A).",
            AtomState.ShowNumbers
        ));

        cards.Add(new CardData(
            "Configuración electrónica",
            "La configuración electrónica indica cómo se distribuyen los electrones en los " +
            "diferentes niveles de energía.\n\n" +
            "<b>Principio de Aufbau:</b> Los electrones siempre ocupan los orbitales de menor " +
            "energía primero.\n\n" +
            "<b>Capacidad por nivel:</b>\n" +
            "• Nivel 1: máximo 2 electrones\n" +
            "• Nivel 2: máximo 8 electrones\n" +
            "• Nivel 3: máximo 18 electrones\n" +
            "• Nivel 4: máximo 32 electrones",
            AtomState.ShowOrbits
        ));
    }

    void MostrarTarjeta(int index)
    {
        CardData card = cards[index];

        txtTitle.text = card.title;
        txtContent.text = card.content;
        txtPageCounter.text = $"{index + 1} de {cards.Count}";

        // Actualizar ícono
        if (cardIcons != null && index < cardIcons.Count && cardIcons[index] != null)
            iconImage.sprite = cardIcons[index];

        // Actualizar barra de progreso
        for (int i = 0; i < progressSegments.Count; i++)
        {
            progressSegments[i].color = i <= index
                ? new Color(0.18f, 0.47f, 0.87f)   // azul activo
                : new Color(0.3f, 0.3f, 0.3f);      // gris inactivo
        }

        // Botón anterior
        btnAnterior.interactable = index > 0;
        Color anteriorColor = index > 0
            ? Color.white
            : new Color(1, 1, 1, 0.3f);
        btnAnterior.GetComponentInChildren<TextMeshProUGUI>().color = anteriorColor;

        // Texto del botón siguiente
        TextMeshProUGUI txtBtn = btnSiguiente.GetComponentInChildren<TextMeshProUGUI>();
        txtBtn.text = index == cards.Count - 1 ? "Comenzar →" : "Siguiente →";

        // Actualizar átomo animado
        if (atomDisplay != null)
            atomDisplay.SetState(card.atomState);
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

public enum AtomState
{
    Default,
    ShowNucleusAndElectrons,
    ShowNumbers,
    ShowOrbits
}

public class CardData
{
    public string title;
    public string content;
    public AtomState atomState;

    public CardData(string title, string content, AtomState atomState)
    {
        this.title = title;
        this.content = content;
        this.atomState = atomState;
    }
}