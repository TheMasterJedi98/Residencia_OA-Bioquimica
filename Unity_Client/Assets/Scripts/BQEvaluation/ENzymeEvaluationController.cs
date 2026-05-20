using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class EnzymeEvaluationController : MonoBehaviour
{
    [Header("UI - Header")]
    public TextMeshProUGUI txtQuestionCounter;
    public TextMeshProUGUI txtScore;
    public Image progressBar;

    [Header("UI - Pregunta")]
    public TextMeshProUGUI txtQuestionType;
    public TextMeshProUGUI txtQuestion;

    [Header("UI - Opciones")]
    public List<Button> optionButtons;
    public List<TextMeshProUGUI> optionLetters;
    public List<TextMeshProUGUI> optionTexts;

    [Header("UI - Navegación")]
    public Button btnConfirm;
    public TextMeshProUGUI txtBtnConfirm;

    [Header("Colores")]
    public Color colorDefault    = new Color(0.13f, 0.19f, 0.27f);
    public Color colorSelected   = new Color(0.18f, 0.35f, 0.55f);
    public Color colorCorrect    = new Color(0.08f, 0.45f, 0.25f);
    public Color colorIncorrect  = new Color(0.5f,  0.1f,  0.1f);

    [Header("Escenas")]
    private const string MAIN_MENU_SCENE = "MainMenu";

    private int currentQuestion = 0;
    private int score           = 0;
    private int selectedOption  = -1;
    private bool answered       = false;

    [Header("Modal")]
    public ResultsModal resultsModal;   

    // --- CAMBIO: Usamos EnzymeQuestionData para evitar conflictos ---
    private List<EnzymeQuestionData> questions = new List<EnzymeQuestionData>();

    void Start()
    {
        InicializarPreguntas();
        MostrarPregunta(0);
    }

    void InicializarPreguntas()
    {
        // Pregunta 1
        questions.Add(new EnzymeQuestionData(
            "¿Qué función principal tienen las enzimas en los sistemas biológicos?",
            new string[] { "Aportar energía a la célula", "Actuar como catalizadores", "Formar membranas celulares", "Almacenar información genética" },
            1
        ));

        // Pregunta 2
        questions.Add(new EnzymeQuestionData(
            "¿Cómo se le llama a la enzima completa unida a sus grupos prostéticos, lista para actuar?",
            new string[] { "Apoenzima", "Coenzima", "Holoenzima", "Cofactor" },
            2
        ));

        // Pregunta 3
        questions.Add(new EnzymeQuestionData(
            "Si una enzima requiere un ion metálico inorgánico (como Fe2+) para funcionar, este grupo se conoce como:",
            new string[] { "Cofactor", "Coenzima", "Apoenzima", "Sustrato" },
            0
        ));

        // Pregunta 4
        questions.Add(new EnzymeQuestionData(
            "Las enzimas del grupo 'Hidrolasas' se encargan de catalizar:",
            new string[] { "Transferencia de electrones", "Reacciones de hidrólisis", "Transferencia de grupos isómeros", "Formación de enlaces C-C" },
            1
        ));

        // Pregunta 5
        questions.Add(new EnzymeQuestionData(
            "¿Qué efecto tienen las enzimas sobre la energía de activación de una reacción?",
            new string[] { "La aumentan considerablemente", "La mantienen igual", "La disminuyen", "La eliminan por completo" },
            2
        ));

        // Pregunta 6
        questions.Add(new EnzymeQuestionData(
            "Al usar una enzima, ¿se modifica el equilibrio químico final de la reacción?",
            new string[] { "Sí, genera más productos", "Sí, genera menos productos", "No, solo acelera la llegada al equilibrio", "Depende de la temperatura" },
            2
        ));

        // Pregunta 7
        questions.Add(new EnzymeQuestionData(
            "En la cinética de Michaelis-Menten, ¿qué ocurre cuando la concentración de sustrato es muy alta?",
            new string[] { "La velocidad disminuye a cero", "La velocidad alcanza una meseta (Vmax)", "La enzima se destruye", "La reacción se invierte" },
            1
        ));

        // Pregunta 8
        questions.Add(new EnzymeQuestionData(
            "En la ecuación de Michaelis-Menten, ¿cuándo se alcanza la mitad de la velocidad máxima?",
            new string[] { "Cuando [S] es igual a cero", "Cuando [S] es igual a la constante de saturación (Ks)", "Cuando se agota la enzima", "Cuando se duplica la temperatura" },
            1
        ));
    }

    void MostrarPregunta(int index)
    {
        selectedOption = -1;
        answered       = false;

        EnzymeQuestionData q = questions[index];

        txtQuestionCounter.text = $"Pregunta {index + 1} de {questions.Count}";
        txtScore.text           = $"Puntuación: {score}/{index}";
        txtQuestionType.text    = "Opción Múltiple";
        txtQuestion.text        = q.question;

        // Barra de progreso
        float progress = (float)index / questions.Count;
        progressBar.fillAmount = progress;

        // Opciones
        string[] letters = { "A", "B", "C", "D" };
        for (int i = 0; i < optionButtons.Count; i++)
        {
            optionLetters[i].text = letters[i];
            optionTexts[i].text   = q.options[i];
            SetButtonColor(optionButtons[i], colorDefault);
            optionButtons[i].interactable = true;
        }

        // Botón confirmar
        txtBtnConfirm.text          = "Confirmar Respuesta";
        btnConfirm.interactable     = false;
    }

    public void OnOptionSelected(int index)
    {
        if (answered) return;

        selectedOption = index;

        for (int i = 0; i < optionButtons.Count; i++)
            SetButtonColor(optionButtons[i],
                i == index ? colorSelected : colorDefault);

        btnConfirm.interactable = true;
    }

    public void OnConfirmClicked()
    {
        if (answered) 
        {
            // Avanzar a la siguiente pregunta
            currentQuestion++;
            if (currentQuestion < questions.Count)
                MostrarPregunta(currentQuestion);
            else
                MostrarResultados();
            return;
        }

        answered = true;

        int correct = questions[currentQuestion].correctIndex;
        bool isCorrect = selectedOption == correct;

        if (isCorrect) score++;

        // Mostrar retroalimentación
        for (int i = 0; i < optionButtons.Count; i++)
        {
            optionButtons[i].interactable = false;

            if (i == correct)
                SetButtonColor(optionButtons[i], colorCorrect);
            else if (i == selectedOption)
                SetButtonColor(optionButtons[i], colorIncorrect);
        }

        // Actualizar score
        txtScore.text = $"Puntuación: {score}/{currentQuestion + 1}";

        // Cambiar texto del botón
        bool isLast = currentQuestion == questions.Count - 1;
        txtBtnConfirm.text = isLast ? "Ver Resultados" : "Siguiente →";
        btnConfirm.interactable = true;
    }

    void MostrarResultados()
    {
        if (resultsModal != null)
            resultsModal.Mostrar(score, questions.Count);
        else
            SceneManager.LoadScene(MAIN_MENU_SCENE);
    }

    void SetButtonColor(Button btn, Color color)
    {
        Image img = btn.GetComponent<Image>();
        if (img != null)
            img.color = color;

        ColorBlock cb = btn.colors;
        cb.normalColor      = color;
        cb.highlightedColor = color * 1.1f;
        cb.selectedColor    = color;
        cb.disabledColor    = color; 
        btn.colors          = cb;
    }
}

// --- CAMBIO: Clase renombrada para evitar choque con la del compañero ---
public class EnzymeQuestionData
{
    public string question;
    public string[] options;
    public int correctIndex;

    public EnzymeQuestionData(string question, string[] options, int correctIndex)
    {
        this.question     = question;
        this.options      = options;
        this.correctIndex = correctIndex;
    }
}