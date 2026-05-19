using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class EvaluationController : MonoBehaviour
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
    private const string PRACTICE_SCENE = "FQSimulator";

    private int currentQuestion = 0;
    private int score           = 0;
    private int selectedOption  = -1;
    private bool answered       = false;

    [Header("Modal")]
    public ResultsModal resultsModal;   

    private List<QuestionData> questions = new List<QuestionData>();

    private const string MAIN_MENU_SCENE = "MainMenu";

    void Start()
    {
        InicializarPreguntas();
        MostrarPregunta(0);
    }

    void InicializarPreguntas()
    {
        questions.Add(new QuestionData(
            "¿Qué partícula determina el tipo de elemento de un átomo?",
            new string[] { "Electrones", "Neutrones", "Protones", "Todas las anteriores" },
            2
        ));

        questions.Add(new QuestionData(
            "¿Cuál es la carga eléctrica del electrón?",
            new string[] { "Positiva", "Negativa", "Neutra", "Variable" },
            1
        ));

        questions.Add(new QuestionData(
            "¿Qué representa el Número Másico (A)?",
            new string[] {
                "Solo el número de protones",
                "Solo el número de neutrones",
                "La suma de protones y neutrones",
                "La suma de protones y electrones"
            },
            2
        ));

        questions.Add(new QuestionData(
            "¿Cuántos electrones caben en el primer nivel de energía?",
            new string[] { "1", "2", "8", "18" },
            1
        ));

        questions.Add(new QuestionData(
            "¿Qué modelo atómico propuso que los electrones giran en órbitas fijas?",
            new string[] { "Dalton", "Thomson", "Rutherford", "Bohr" },
            3
        ));

        questions.Add(new QuestionData(
            "¿Cuál es el elemento con número atómico Z=6?",
            new string[] { "Hidrógeno", "Helio", "Litio", "Carbono" },
            3
        ));

        questions.Add(new QuestionData(
            "¿Qué son los isótopos?",
            new string[] {
                "Átomos con igual número de electrones",
                "Átomos con igual Z pero diferente número de neutrones",
                "Átomos con diferente número de protones",
                "Átomos con igual número másico"
            },
            1
        ));

        questions.Add(new QuestionData(
            "¿Dónde se concentra casi toda la masa del átomo?",
            new string[] { "En los electrones", "En la corteza", "En el núcleo", "En los orbitales" },
            2
        ));

        questions.Add(new QuestionData(
            "¿Cuántos electrones caben en el segundo nivel de energía?",
            new string[] { "2", "6", "8", "18" },
            2
        ));

        questions.Add(new QuestionData(
            "¿Qué partículas se encuentran en el núcleo atómico?",
            new string[] {
                "Solo protones",
                "Protones y electrones",
                "Protones y neutrones",
                "Neutrones y electrones"
            },
            2
        ));
    }

    void MostrarPregunta(int index)
    {
        selectedOption = -1;
        answered       = false;

        QuestionData q = questions[index];

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
        // Cambia directamente el color de la Image del botón
        Image img = btn.GetComponent<Image>();
        if (img != null)
            img.color = color;

        // También actualiza el ColorBlock para consistencia
        ColorBlock cb = btn.colors;
        cb.normalColor      = color;
        cb.highlightedColor = color * 1.1f;
        cb.selectedColor    = color;
        cb.disabledColor    = color; // ← esto es lo que faltaba
        btn.colors          = cb;
    }
    
}

public class QuestionData
{
    public string question;
    public string[] options;
    public int correctIndex;

    public QuestionData(string question, string[] options, int correctIndex)
    {
        this.question     = question;
        this.options      = options;
        this.correctIndex = correctIndex;
    }
}