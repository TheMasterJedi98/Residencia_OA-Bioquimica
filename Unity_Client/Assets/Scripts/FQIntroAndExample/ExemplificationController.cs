using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class ExemplificationController : MonoBehaviour
{
    [Header("Navegación de elementos")]
    public Button btnPrev;
    public Button btnNext;
    public TextMeshProUGUI txtPageCounter;
    public List<Button> elementButtons;

    [Header("Panel izquierdo")]
    public TextMeshProUGUI txtSymbol;
    public TextMeshProUGUI txtElementName;
    public TextMeshProUGUI txtAtomicNumber;
    public AtomDisplayIntro atomDisplay;

    [Header("Panel central")]
    public TextMeshProUGUI txtProtonCount;
    public TextMeshProUGUI txtNeutronCount;
    public TextMeshProUGUI txtElectronCount;
    public TextMeshProUGUI txtAtomicNumberInfo;
    public TextMeshProUGUI txtMassNumber;

    [Header("Panel derecho")]
    public TextMeshProUGUI txtAboutTitle;
    public TextMeshProUGUI txtAboutContent;
    public TextMeshProUGUI txtFunFacts;
    public TextMeshProUGUI txtChargeInfo;

    [Header("Escenas")]
    private const string INTRO_SCENE    = "FQIntroduction";
    private const string PRACTICE_SCENE = "FQSimulator";

    private int currentIndex = 0;
    private List<ElementData> elements  = new List<ElementData>();

    void Start()
    {
        InicializarElementos();

        // Botones anterior y siguiente
        btnPrev.onClick.AddListener(OnPrevClicked);
        btnNext.onClick.AddListener(OnNextClicked);

        // Botones de elementos
        for (int i = 0; i < elementButtons.Count; i++)
        {
            int index = i; // Importante para evitar captura incorrecta
            elementButtons[i].onClick.AddListener(() => OnElementButtonClicked(index));
        }

        MostrarElemento(0);
    }

    void InicializarElementos()
    {
        elements.Add(new ElementData(
            "H", "Hidrógeno", 1, 0, 1,
            "El elemento más simple y abundante del universo. " +
            "Forma parte del agua y es esencial para la vida.",
            new string[]
            {
                "Compone el 75% de la masa del universo",
                "Es el combustible de las estrellas",
                "El átomo más ligero que existe"
            },
            AtomStateEx.H
        ));

        elements.Add(new ElementData(
            "He", "Helio", 2, 2, 2,
            "Gas noble inerte descubierto primero en el Sol. " +
            "Es el segundo elemento más abundante del universo " +
            "y se usa en globos y tecnología de enfriamiento.",
            new string[]
            {
                "Fue descubierto en el Sol antes que en la Tierra",
                "No forma compuestos químicos de forma natural",
                "Su punto de ebullición es el más bajo de todos los elementos"
            },
            AtomStateEx.He
        ));

        elements.Add(new ElementData(
            "Li", "Litio", 3, 4, 3,
            "Metal alcalino muy ligero y reactivo. " +
            "Es fundamental en la tecnología moderna, " +
            "especialmente en baterías recargables.",
            new string[]
            {
                "Es el metal sólido más ligero que existe",
                "Sus baterías alimentan teléfonos y autos eléctricos",
                "Se usa en tratamientos de trastorno bipolar"
            },
            AtomStateEx.Li
        ));

        elements.Add(new ElementData(
            "C", "Carbono", 6, 6, 6,
            "Base de toda la química orgánica y de la vida. " +
            "Puede formar millones de compuestos diferentes " +
            "gracias a su capacidad de enlazarse con otros átomos.",
            new string[]
            {
                "Es la base de toda la vida en la Tierra",
                "El diamante y el grafito están hechos de carbono puro",
                "Puede formar más compuestos que cualquier otro elemento"
            },
            AtomStateEx.C
        ));

        elements.Add(new ElementData(
            "O", "Oxígeno", 8, 8, 8,
            "Gas esencial para la respiración y la combustión. " +
            "Es el elemento más abundante en la corteza terrestre " +
            "y el tercero más abundante en el universo.",
            new string[]
            {
                "Representa el 21% de la atmósfera terrestre",
                "Es indispensable para la respiración celular",
                "Forma parte del agua junto con el hidrógeno"
            },
            AtomStateEx.O
        ));
    }

    void MostrarElemento(int index)
    {
        currentIndex = index;
        ElementData el = elements[index];

        // Panel izquierdo
        txtSymbol.text      = el.symbol;
        txtElementName.text = el.name;
        txtAtomicNumber.text = el.protons.ToString();

        // Panel central
        txtProtonCount.text   = el.protons.ToString();
        txtNeutronCount.text  = el.neutrons.ToString();
        txtElectronCount.text = el.electrons.ToString();
        txtAtomicNumberInfo.text = el.protons.ToString();
        txtMassNumber.text = (el.protons + el.neutrons).ToString();

        // Panel derecho
        txtAboutTitle.text   = "Sobre el " + el.name;
        txtAboutContent.text = el.about;

        // Fun facts
        string facts = "";
        foreach (string fact in el.funFacts)
            facts += "• " + fact + "\n";
        txtFunFacts.text = facts.TrimEnd();

        // Carga neta
        int charge = el.protons - el.electrons;
        string chargeStr = charge == 0
            ? $"{el.protons}+ + {el.electrons}- = <b>Neutro</b>"
            : charge > 0
                ? $"{el.protons}+ + {el.electrons}- = <b>+{charge}</b>"
                : $"{el.protons}+ + {el.electrons}- = <b>{charge}</b>";
        txtChargeInfo.text = chargeStr;

        // Contador
        txtPageCounter.text = $"{index + 1} de {elements.Count}";

        // Botones de navegación
        btnPrev.interactable = index > 0;
        btnNext.interactable = index < elements.Count - 1;

        // Resaltar botón del elemento activo
        for (int i = 0; i < elementButtons.Count; i++)
        {
            ColorBlock cb = elementButtons[i].colors;
            cb.normalColor = i == index
                ? new Color(0.1f, 0.6f, 0.4f)
                : new Color(0.15f, 0.2f, 0.28f);
            elementButtons[i].colors = cb;
        }

        // Actualizar átomo
        if (atomDisplay != null)
        {
            atomDisplay.InicializarCentro();
            atomDisplay.SetStateEx(el.atomState, el.protons, el.neutrons, el.electrons);
        }
    }

    public void OnPrevClicked()
    {
        if (currentIndex > 0)
            MostrarElemento(currentIndex - 1);
    }

    public void OnNextClicked()
    {
        if (currentIndex < elements.Count - 1)
            MostrarElemento(currentIndex + 1);
    }

    public void OnElementButtonClicked(int index)
    {
        MostrarElemento(index);
    }

    public void OnBackClicked()
    {
        SceneManager.LoadScene(INTRO_SCENE);
    }

    public void OnPracticeClicked()
    {
        SceneManager.LoadScene(PRACTICE_SCENE);
    }
}

public enum AtomStateEx { H, He, Li, C, O }

public class ElementData
{
    public string symbol;
    public string name;
    public int protons;
    public int neutrons;
    public int electrons;
    public string about;
    public string[] funFacts;
    public AtomStateEx atomState;

    public ElementData(string symbol, string name, int protons,
        int neutrons, int electrons, string about,
        string[] funFacts, AtomStateEx atomState)
    {
        this.symbol   = symbol;
        this.name     = name;
        this.protons  = protons;
        this.neutrons = neutrons;
        this.electrons = electrons;
        this.about    = about;
        this.funFacts = funFacts;
        this.atomState = atomState;
    }
}