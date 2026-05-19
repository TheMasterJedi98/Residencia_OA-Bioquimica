using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ResultsModal : MonoBehaviour
{
    [Header("UI")]
    public GameObject modalRoot;
    public TextMeshProUGUI txtCompleted;
    public TextMeshProUGUI txtScoreResult;
    public TextMeshProUGUI txtMessage;
    public Button btnFinish;

    [Header("Confeti")]
    public RectTransform confettiContainer;
    public int confettiCount = 60;

    private const string MAIN_MENU_SCENE = "MainMenu";

    private Color[] confettiColors = new Color[]
    {
        new Color(0.95f, 0.26f, 0.21f),
        new Color(0.13f, 0.59f, 0.95f),
        new Color(0.30f, 0.69f, 0.31f),
        new Color(1.00f, 0.76f, 0.03f),
        new Color(0.61f, 0.15f, 0.69f),
        new Color(0.00f, 0.74f, 0.83f)
    };

    void Start()
    {
        modalRoot.SetActive(false);
    }

    public void Mostrar(int score, int total)
    {
        modalRoot.SetActive(true);

        txtCompleted.text   = "¡Has completado una sección!";
        txtScoreResult.text = $"{score} / {total}";

        float percent = (float)score / total;

        if (percent >= 0.9f)
            txtMessage.text = "¡Excelente! Dominas el tema perfectamente.";
        else if (percent >= 0.7f)
            txtMessage.text = "¡Muy bien! Tienes un buen entendimiento del tema.";
        else if (percent >= 0.5f)
            txtMessage.text = "Bien. Repasa algunos conceptos y lo tendrás.";
        else
            txtMessage.text = "Sigue practicando. Revisa la introducción nuevamente.";

        StartCoroutine(LanzarConfeti());
    }

    IEnumerator LanzarConfeti()
    {
        for (int i = 0; i < confettiCount; i++)
        {
            CrearPieza();
            yield return new WaitForSeconds(0.05f);
        }
    }

    void CrearPieza()
    {
        GameObject pieza = new GameObject("Confetti");
        pieza.transform.SetParent(confettiContainer, false);

        RectTransform rt = pieza.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(
            Random.Range(8f, 16f),
            Random.Range(8f, 20f)
        );

        // Posición inicial aleatoria en la parte superior
        rt.anchorMin = new Vector2(Random.Range(0f, 1f), 1f);
        rt.anchorMax = rt.anchorMin;
        rt.anchoredPosition = new Vector2(0, 20f);

        Image img = pieza.AddComponent<Image>();
        img.color = confettiColors[Random.Range(0, confettiColors.Length)];

        // Rotación aleatoria
        rt.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

        StartCoroutine(AnimarPieza(rt, img));
    }

    IEnumerator AnimarPieza(RectTransform rt, Image img)
    {
        float duration  = Random.Range(2f, 4f);
        float elapsed   = 0f;
        float speedY    = Random.Range(-400f, -200f);
        float speedX    = Random.Range(-80f, 80f);
        float rotSpeed  = Random.Range(-180f, 180f);
        Vector2 startPos = rt.anchoredPosition;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            rt.anchoredPosition = startPos + new Vector2(
                speedX * elapsed,
                speedY * elapsed
            );

            rt.rotation = Quaternion.Euler(0, 0, rotSpeed * elapsed);

            // Fade out al final
            Color c = img.color;
            c.a = Mathf.Lerp(1f, 0f, Mathf.Max(0f, (t - 0.7f) / 0.3f));
            img.color = c;

            yield return null;
        }

        Destroy(rt.gameObject);
    }

    public void OnFinishClicked()
    {
        SceneManager.LoadScene(MAIN_MENU_SCENE);
    }
}