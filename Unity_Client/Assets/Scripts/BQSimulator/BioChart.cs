using UnityEngine;
using System.Collections.Generic;

public class BioChart : MonoBehaviour
{
    public LineRenderer lineBiomasa;
    public LineRenderer lineSustrato;
    public RectTransform chartBounds; // Objeto "Graphic" (panel oscuro)

    // Almacenamos los datos reales para poder redibujar si la escala cambia
    private List<Vector2> datosBiomasa = new List<Vector2>();
    private List<Vector2> datosSustrato = new List<Vector2>();
    
    // Límites dinámicos (Auto-escalado) que reemplazan a los const fijos
    // Los iniciamos en 1f para evitar divisiones entre cero en el primer frame
    private float maxVisualTime = 1f; 
    private float maxVisualConc = 1f; 

    void Awake()
    {
        // Forzar UseWorldSpace a false por código para evitar errores humanos
        if(lineBiomasa) lineBiomasa.useWorldSpace = false;
        if(lineSustrato) lineSustrato.useWorldSpace = false;
        LimpiarGrafica();
    }

    public void LimpiarGrafica()
    {
        if(lineBiomasa) lineBiomasa.positionCount = 0;
        if(lineSustrato) lineSustrato.positionCount = 0;
        
        datosBiomasa.Clear();
        datosSustrato.Clear();
        
        // Reiniciamos los límites al limpiar
        maxVisualTime = 1f;
        maxVisualConc = 1f;
    }

    public void AgregarPunto(float tiempo, float biomasa, float sustrato)
    {
        if(!lineBiomasa || !lineSustrato || !chartBounds) return;

        // 1. Guardar los datos matemáticos reales
        datosBiomasa.Add(new Vector2(tiempo, biomasa));
        datosSustrato.Add(new Vector2(tiempo, sustrato));

        // 2. Comprobar si este nuevo punto requiere que expandamos los límites visuales
        bool requiereRedibujado = false;

        if (tiempo > maxVisualTime) 
        {
            maxVisualTime = tiempo;
            requiereRedibujado = true;
        }
        if (biomasa > maxVisualConc) 
        {
            maxVisualConc = biomasa;
            requiereRedibujado = true;
        }
        if (sustrato > maxVisualConc) 
        {
            maxVisualConc = sustrato;
            requiereRedibujado = true;
        }

        // 3. Dibujar
        if (requiereRedibujado)
        {
            // Si el límite creció, recalculamos TODOS los puntos para encoger la gráfica y hacer espacio
            RedibujarTodo();
        }
        else
        {
            // Si el límite no cambió, es más eficiente solo dibujar el punto nuevo
            DibujarUltimoPunto();
        }
    }

    private void RedibujarTodo()
    {
        lineBiomasa.positionCount = datosBiomasa.Count;
        lineSustrato.positionCount = datosSustrato.Count;

        for (int i = 0; i < datosBiomasa.Count; i++)
        {
            CalcularYAsignarPosicion(i, datosBiomasa[i].x, datosBiomasa[i].y, datosSustrato[i].y);
        }
    }

    private void DibujarUltimoPunto()
    {
        int index = datosBiomasa.Count - 1;
        lineBiomasa.positionCount = datosBiomasa.Count;
        lineSustrato.positionCount = datosSustrato.Count;
        
        CalcularYAsignarPosicion(index, datosBiomasa[index].x, datosBiomasa[index].y, datosSustrato[index].y);
    }

    private void CalcularYAsignarPosicion(int index, float tiempo, float biomasa, float sustrato)
    {
        // --- LÓGICA DE COORDENADAS LOCALES ROBUSTA (MVC View) ---

        // 1. Normalizar valores a porcentaje (0 a 1) usando los LÍMITES DINÁMICOS
        float pctX = Mathf.Clamp01(tiempo / maxVisualTime);
        float pctY_B = Mathf.Clamp01(biomasa / maxVisualConc);
        float pctY_S = Mathf.Clamp01(sustrato / maxVisualConc);

        // 2. Obtener el tamaño exacto en píxeles UI
        float width = chartBounds.rect.width;
        float height = chartBounds.rect.height;

        // 3. Calcular posición local (Pivote 0.5, 0.5)
        float finalLocalX = (pctX - 0.5f) * width;
        float finalLocalY_B = (pctY_B - 0.5f) * height;
        float finalLocalY_S = (pctY_S - 0.5f) * height;

        // Profundidad Z para visibilidad
        float posZ = -1.0f; 

        Vector3 localPosBiomasa = new Vector3(finalLocalX, finalLocalY_B, posZ);
        Vector3 localPosSustrato = new Vector3(finalLocalX, finalLocalY_S, posZ);

        lineBiomasa.SetPosition(index, localPosBiomasa);
        lineSustrato.SetPosition(index, localPosSustrato);
    }
}