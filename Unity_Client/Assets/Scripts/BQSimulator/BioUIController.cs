using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Threading;

public class BioUIController : MonoBehaviour
{
    [Header("Sliders - Condiciones Iniciales")]
    public Slider sliderS0;
    public Slider sliderX0;
    public Slider sliderV0;
    public TMP_Text indicatorS0;
    public TMP_Text indicatorX0;
    public TMP_Text indicatorV0;

    [Header("Sliders - Parámetros Cinéticos")]
    public Slider sliderUmax;
    public Slider sliderKs;
    public Slider sliderYxs;
    public TMP_Text indicatorUmax;
    public TMP_Text indicatorKs;
    public TMP_Text indicatorYxs;

    [Header("Textos de Interfaz")]
    public TMP_Text textConsole;
    public TMP_Text topHudTextS;
    public TMP_Text topHudTextX;
    public TMP_Text topHudTextU; // ¡Nuevo! Para mostrar la µ

    [Header("Panel Derecho (KPIs)")]
    public TextMeshProUGUI textTiempo;
    public TextMeshProUGUI textProductividad;

    [Header("Panel Central (Indicadores)")]
    public TextMeshProUGUI textTiempoDuplicacion;
    public TextMeshProUGUI textSustratoAgotado;

    // --- VARIABLES INTERNAS DEL SIMULADOR ---
    private bool isSimulating = false;
    private double currentS;
    private double currentX;
    private double simUmax;
    private double simKs;
    private double simYxs;
    private double timeElapsed = 0;
    private float simSpeed = 1f; // Controla el multiplicador de tiempo

    public BioChart bioChart;
    private float timerGrafica = 0f;
    private float intervaloMuestreo = 0.2f; // Registra un punto cada 0.2 segundos de simulación

    private double tiempoAgotadoFinal = 0;

    private bool yaSeAgoto = false;

    void Start()
    {
        // Vinculamos los eventos de los sliders
        sliderS0.onValueChanged.AddListener(delegate { ActualizarTextosSliders(); });
        sliderX0.onValueChanged.AddListener(delegate { ActualizarTextosSliders(); });
        sliderV0.onValueChanged.AddListener(delegate { ActualizarTextosSliders(); });
        sliderUmax.onValueChanged.AddListener(delegate { ActualizarTextosSliders(); });
        sliderKs.onValueChanged.AddListener(delegate { ActualizarTextosSliders(); });
        sliderYxs.onValueChanged.AddListener(delegate { ActualizarTextosSliders(); });

        textConsole.text = "Sistema listo. Esperando parámetros...";
        ActualizarTextosSliders(); 
    }

    private void ActualizarTextosSliders()
    {
        indicatorS0.text = sliderS0.value.ToString("F2");
        indicatorX0.text = sliderX0.value.ToString("F2");
        indicatorV0.text = sliderV0.value.ToString("F2");
        
        indicatorUmax.text = sliderUmax.value.ToString("F2");
        indicatorKs.text = sliderKs.value.ToString("F2");
        indicatorYxs.text = sliderYxs.value.ToString("F2");
    }

    public void OnIniciarSimulacionClicked()
    {
        textConsole.text = "Simulación en curso...";
        bioChart.ConfigurarLimites((float)sliderS0.value); // Ajustamos el techo de la gráfica según el S0 inicial
        bioChart.LimpiarGrafica();
        timerGrafica = 0f; // Reiniciamos el temporizador de la gráfica
        // 1. Capturamos los valores de los sliders al momento de hacer clic
        currentS = sliderS0.value;
        currentX = sliderX0.value;
        simUmax = sliderUmax.value;
        simKs = sliderKs.value;
        simYxs = sliderYxs.value;
        timeElapsed = 0;
        tiempoAgotadoFinal = 0;
        textSustratoAgotado.text = "Sustrato agotado en: -- hrs";
        yaSeAgoto = false;

        // 2. Encendemos el motor
        isSimulating = true;
    }

    // El método Update se ejecuta una vez por cada fotograma de Unity
    void Update()
    {
        if (!isSimulating) return;

        // 1. --- AVANZAR EL TIEMPO PRIMERO ---
        // Calculamos el paso del tiempo real (dt) multiplicado por la velocidad (1x, 2x, 5x)
        double dt = Time.deltaTime * simSpeed;
        timeElapsed += dt;

        // 2. --- EL CORAZÓN MATEMÁTICO ---
        // Ecuación de Monod: Calculamos la velocidad específica actual (µ)
        double currentU = (simUmax * currentS) / (simKs + currentS);

        // Cinética de crecimiento y consumo (Euler)
        double deltaX = currentU * currentX * dt;
        double deltaS = -(deltaX / simYxs);

        // Actualizamos las concentraciones
        currentX += deltaX;
        currentS += deltaS;

        // 3. --- ACTUALIZACIÓN DE LA UI EN TIEMPO REAL ---
        // Ahora actualizamos los textos usando los datos exactos calculados en este frame
        textTiempo.text = "Tiempo transcurrido: " + timeElapsed.ToString("F2") + " hrs";

        double biomasaInicial = sliderX0.value;
        double productividad = 0;
        if (timeElapsed > 0.001)
        {
            productividad = (currentX - biomasaInicial) / timeElapsed;
            if (productividad < 0) productividad = 0; 
        }
        textProductividad.text = "Productividad: " + productividad.ToString("F2") + " g/L·h";

        topHudTextS.text = $"[S] Actual:\n{currentS:F2}";
        topHudTextX.text = $"[X] Actual:\n{currentX:F2}";
        topHudTextU.text = $"Vel. Específica (µ):\n{currentU:F4}";

        if (currentU > 0.0001)
        {
            double tDuplicacion = System.Math.Log(2.0) / currentU;
            textTiempoDuplicacion.text = $"Tiempo de Duplicación:\n{tDuplicacion:F2} hrs";
        }
        else
        {
            textTiempoDuplicacion.text = "Tiempo de Duplicación:\nIncalculable (µ → 0)";
        }

        // 4. --- ACTUALIZAR GRÁFICA ---
        timerGrafica += (float)dt;
        if (timerGrafica >= intervaloMuestreo)
        {
            bioChart.AgregarPunto((float)timeElapsed, (float)currentX, (float)currentS);
            timerGrafica = 0f;
        }

        // 5. --- CONDICIÓN DE PARO EXACTA ---
        // Usamos <= 0.01 para que detecte el final en cuanto la UI muestra 0.00
        // El candado (!yaSeAgoto) asegura que esto SOLO se ejecute una vez en toda la simulación
        if (currentS <= 0.01 && !yaSeAgoto)
        {
            currentS = 0;
            yaSeAgoto = true; // Cerramos el candado: ya no volverá a entrar aquí.
            
            topHudTextS.text = $"[S] Actual:\n0.00"; // Forzamos visualmente a 0
            
            // ¡Capturamos el tiempo EXACTO de agotamiento y lo congelamos!
            textConsole.text = $"Simulación terminada. Sustrato agotado en t={timeElapsed:F2} hrs.";
            textSustratoAgotado.text = $"Sustrato agotado en:\n{timeElapsed:F2} hrs";

            // Si quieres que las líneas azules y verdes sigan avanzando de forma plana 
            // (fase estacionaria), comenta o borra la siguiente línea. 
            // Si prefieres que el tiempo se congele por completo, déjala:
            isSimulating = false; 
        }
    }

    // Métodos extras para los otros botones
    public void SetSpeed1X() { simSpeed = 1f; }
    public void SetSpeed2X() { simSpeed = 2f; }
    public void SetSpeed5X() { simSpeed = 5f; }
    // Método vinculado al Boton_Detener de la interfaz
    public void DetenerSimulacion()
    {
        if (!isSimulating) return; // Si ya estaba detenida, ignorar

        isSimulating = false; // Detiene el avance matemático
        textConsole.text = "Simulación interrumpida por el usuario.";

        // Evaluamos el estado del sustrato en el momento exacto del botón de stop
        if (currentS > 0.05) 
        {
            // Si aún quedaba sustrato, informamos que no se agotó
            textSustratoAgotado.text = "Sustrato agotado en:\nNo agotado (Interrumpido)";
        }
        else 
        {
            // Si por casualidad ya estaba en cero, muestra el tiempo de parada
            textSustratoAgotado.text = $"Sustrato agotado en:\n{timeElapsed:F2} hrs";
        }
    }
    public void ReiniciarSimulacion() 
    { 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
        bioChart.LimpiarGrafica();
        textTiempo.text = "Tiempo transcurrido: --";
        textProductividad.text = "Productividad: --";
    }

}