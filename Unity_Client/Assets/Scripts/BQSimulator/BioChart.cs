using UnityEngine;

public class BioChart : MonoBehaviour
{
    public LineRenderer lineBiomasa;
    public LineRenderer lineSustrato;
    public RectTransform chartBounds; // Objeto "Graphic" (panel oscuro)

    private int pointCount = 0;
    
    // Límites lógicos fijos para el renderizado visual (MVC View bounds)
    // Coinciden con los ejes dibujados en tu imagen de fondo (ej. 30 hrs y 100 g/L)
    private const float MAX_VISUAL_TIME = 30f; 
    private const float MAX_VISUAL_CONC = 100f; 

    void Awake()
    {
        // Forzar UseWorldSpace a false por código para evitar errores humanos
        if(lineBiomasa) lineBiomasa.useWorldSpace = false;
        if(lineSustrato) lineSustrato.useWorldSpace = false;
        LimpiarGrafica();
    }

    // Ya no necesitamos configurar límites dinámicos para el renderizado visual simple,
    // usamos límites fijos para normalizar la gráfica al fondo estático.
    public void ConfigurarLimites(float maxSustratoInicial) { /* Obsoleto en este enfoque robústo */ }

    public void LimpiarGrafica()
    {
        if(lineBiomasa) lineBiomasa.positionCount = 0;
        if(lineSustrato) lineSustrato.positionCount = 0;
        pointCount = 0;
    }

    public void AgregarPunto(float tiempo, float biomasa, float sustrato)
    {
        if(!lineBiomasa || !lineSustrato || !chartBounds) return;

        pointCount++;
        lineBiomasa.positionCount = pointCount;
        lineSustrato.positionCount = pointCount;

        // --- LÓGICA DE COORDENADAS LOCALES ROBUSTA (MVC View) ---

        // 1. Normalizar valores matemáticos a porcentaje (0 a 1) respecto a los ejes visuales fijos
        float pctX = Mathf.Clamp01(tiempo / MAX_VISUAL_TIME);
        float pctY_B = Mathf.Clamp01(biomasa / MAX_VISUAL_CONC);
        float pctY_S = Mathf.Clamp01(sustrato / MAX_VISUAL_CONC);

        // 2. Obtener el tamaño exacto en píxeles UI del panel "Graphic"
        float width = chartBounds.rect.width;
        float height = chartBounds.rect.height;

        // 3. Calcular posición local en píxeles, asumiendo que el pivote de Graphic está en el centro (0.5, 0.5)
        // Convertimos el rango (0 a 1) al rango (-width/2 a +width/2)
        float finalLocalX = (pctX - 0.5f) * width;
        float finalLocalY_B = (pctY_B - 0.5f) * height;
        float finalLocalY_S = (pctY_S - 0.5f) * height;

        // --- SOLUCIÓN DE VISIBILIDAD DEFINITIVA (Local Z-Depth) ---
        // Al ser hijos de 'Graphic', -1.0f localmente los pone justo ENFRENTE del panel en GAME.
        float posZ = -1.0f; 

        Vector3 localPosBiomasa = new Vector3(finalLocalX, finalLocalY_B, posZ);
        Vector3 localPosSustrato = new Vector3(finalLocalX, finalLocalY_S, posZ);

        // Asignar los puntos finales directamente (sin TransformPoint)
        lineBiomasa.SetPosition(pointCount - 1, localPosBiomasa);
        lineSustrato.SetPosition(pointCount - 1, localPosSustrato);
    }
}