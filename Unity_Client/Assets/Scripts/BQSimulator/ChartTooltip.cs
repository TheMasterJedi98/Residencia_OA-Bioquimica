using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ChartTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IPointerMoveHandler
{
    [Header("Referencias UI")]
    public GameObject tooltipGameObject; // Arrastrar 'UI_Tooltip_Box'
    public TextMeshProUGUI textTooltip;  // Arrastrar el 'TooltipText'
    public RectTransform graphContainer;  // Arrastrar el cuadro contenedor de la gráfica derecha
    public BioUIController uiController;  // Arrastrar el objeto 'BioManager' (o quien tenga el script principal)

    private bool isMouseOver = false;

    void Start()
    {
        if (tooltipGameObject != null)
            tooltipGameObject.SetActive(false);
    }

    // Se ejecuta cuando el mouse entra al cuadro de la gráfica
    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
        if (uiController.historialPuntos.Count > 0 && tooltipGameObject != null)
            tooltipGameObject.SetActive(true);
    }

    // Se ejecuta cuando el mouse sale de la gráfica
    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
        if (tooltipGameObject != null)
            tooltipGameObject.SetActive(false);
    }

    // Se ejecuta de manera continua mientras movemos el mouse por encima
    public void OnPointerMove(PointerEventData eventData)
    {
        ActualizarTooltip(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        ActualizarTooltip(eventData); // Permite que funcione incluso si mantenemos presionado el clic
    }

    private void ActualizarTooltip(PointerEventData eventData)
    {
        if (!isMouseOver || uiController.historialPuntos.Count == 0 || tooltipGameObject == null) return;

        // 1. Hacer que la cajita flote siguiendo la posición exacta del mouse
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)tooltipGameObject.transform.parent, 
            eventData.position, 
            eventData.pressEventCamera, 
            out position
        );
        // Desplazamos un poco en X e Y para que no quede justo pegado a la punta de la flecha
        tooltipGameObject.transform.localPosition = new Vector3(position.x + 15f, position.y + 15f, 0f);

        // 2. Calcular en qué porcentaje horizontal (X) de la gráfica está parado el mouse
        Vector2 localMousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(graphContainer, eventData.position, eventData.pressEventCamera, out localMousePos);

        float width = graphContainer.rect.width;
        // Normalizamos la posición de la izquierda (0) a la derecha (1)
        float pctX = (localMousePos.x + (graphContainer.pivot.x * width)) / width;
        pctX = Mathf.Clamp01(pctX);

        // 3. Determinar el tiempo objetivo que corresponde a esa coordenada X
        float tiempoMaxRegistrado = uiController.historialPuntos[uiController.historialPuntos.Count - 1].tiempo;
        float tiempoObjetivo = pctX * tiempoMaxRegistrado;

        // 4. Buscar el punto simulado más cercano en el historial
        BioUIController.PuntoSimulado puntoMasCercano = BuscarPuntoMasCercano(tiempoObjetivo);

        // 5. Imprimir los datos limpios dentro de la caja flotante
        textTooltip.text = $"<b>Tiempo:</b> {puntoMasCercano.tiempo:F2} hrs\n" +
                           $"<b><color=#5DD35D>[X]:</color></b> {puntoMasCercano.biomasa:F2} g/L\n" +
                           $"<b><color=#4B9CD3>[S]:</color></b> {puntoMasCercano.sustrato:F2} g/L";
    }

    private BioUIController.PuntoSimulado BuscarPuntoMasCercano(float tiempoObjetivo)
    {
        var lista = uiController.historialPuntos;
        BioUIController.PuntoSimulado mejorPunto = lista[0];
        float menorDiferencia = Mathf.Abs(mejorPunto.tiempo - tiempoObjetivo);

        for (int i = 1; i < lista.Count; i++)
        {
            float diferenciaActual = Mathf.Abs(lista[i].tiempo - tiempoObjetivo);
            if (diferenciaActual < menorDiferencia)
            {
                menorDiferencia = diferenciaActual;
                mejorPunto = lista[i];
            }
        }
        return mejorPunto;
    }
}