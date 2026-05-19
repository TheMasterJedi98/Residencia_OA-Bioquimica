using UnityEngine;

public class AnalyticChart : MonoBehaviour
{
    public RectTransform chartBounds; // Asignar el recuadro gris (Analytic_Graphic_Box)
    public RectTransform lineRect;    // Asignar el objeto de la imagen roja (Line_Render)

    // Límites VISUALES
    private const float X_VISUAL_MIN = -2f;
    private const float X_VISUAL_MAX = 6f;
    private const float Y_VISUAL_MIN = -2f;
    private const float Y_VISUAL_MAX = 8f;

    [Header("Grosor de la línea")]
    public float lineWidth = 6f; 

    public void ActualizarLineWeaverBurk(double uMax, double ks)
    {
        // Validaciones de seguridad
        if (chartBounds == null || lineRect == null || uMax <= 0 || ks <= 0) return;

        float width = chartBounds.rect.width;
        float height = chartBounds.rect.height;

        if (width <= 0 || height <= 0) return;

        // Punto 1 (Calculamos Y teórica usando la X mínima visual)
        float x1 = X_VISUAL_MIN;
        float y1 = (float)((ks / uMax) * x1 + (1.0 / uMax));
        
        // Punto 2 (Calculamos Y teórica usando la X máxima visual)
        float x2 = X_VISUAL_MAX;
        float y2 = (float)((ks / uMax) * x2 + (1.0 / uMax));

        // Convertir a coordenadas locales de la pantalla
        Vector2 p1 = NormalizarPunto(x1, y1, width, height);
        Vector2 p2 = NormalizarPunto(x2, y2, width, height);

        // --- MAGIA MATEMÁTICA ---
        // Calculamos la distancia y el ángulo entre los dos puntos
        Vector2 dir = p2 - p1;
        float distance = dir.magnitude;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // Estiramos la imagen para que mida la 'distancia' y la rotamos hacia el 'ángulo'
        lineRect.sizeDelta = new Vector2(distance, lineWidth);
        lineRect.anchoredPosition = p1 + dir / 2f; // La centramos a la mitad del trayecto
        lineRect.localEulerAngles = new Vector3(0, 0, angle);
    }

    // Transforma las coordenadas de Monod a píxeles de Unity
    private Vector2 NormalizarPunto(float mathX, float mathY, float width, float height)
    {
        float rangeX = X_VISUAL_MAX - X_VISUAL_MIN;
        float rangeY = Y_VISUAL_MAX - Y_VISUAL_MIN;

        float pctX = (mathX - X_VISUAL_MIN) / rangeX;
        float pctY = (mathY - Y_VISUAL_MIN) / rangeY;

        float localX = (pctX - 0.5f) * width;
        float localY = (pctY - 0.5f) * height;

        return new Vector2(localX, localY);
    }
}