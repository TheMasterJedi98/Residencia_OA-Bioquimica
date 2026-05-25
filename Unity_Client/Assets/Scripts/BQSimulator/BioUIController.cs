using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

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
    public TMP_Text topHudTextU; 

    [Header("Panel Derecho (KPIs)")]
    public TextMeshProUGUI textTiempo;
    public TextMeshProUGUI textProductividad;

    [Header("Panel Central (Indicadores)")]
    public TextMeshProUGUI textTiempoDuplicacion;
    public TextMeshProUGUI textSustratoAgotado;

    [Header("Gráfica Central Analítica")]
    public AnalyticChart analyticChart;

    // --- ESTRUCTURA PARA EL TOOLTIP ---
    public struct PuntoSimulado
    {
        public float tiempo;
        public float biomasa;
        public float sustrato;
    }
    
    [HideInInspector] 
    public List<PuntoSimulado> historialPuntos = new List<PuntoSimulado>();

    // --- VARIABLES INTERNAS DEL SIMULADOR ---
    private bool isSimulating = false;
    private double currentS;
    private double currentX;
    private double simUmax;
    private double simKs;
    private double simYxs;
    private double timeElapsed = 0;
    private float simSpeed = 1f; 

    public BioChart bioChart;
    private float timerGrafica = 0f;
    private float intervaloMuestreo = 0.2f; 

    private bool yaSeAgoto = false;

    void Start()
    {
        sliderS0.onValueChanged.AddListener(delegate { ActualizarTextosSliders(); });
        sliderX0.onValueChanged.AddListener(delegate { ActualizarTextosSliders(); });
        sliderV0.onValueChanged.AddListener(delegate { ActualizarTextosSliders(); });
        sliderUmax.onValueChanged.AddListener(delegate { ActualizarTextosSliders(); });
        sliderKs.onValueChanged.AddListener(delegate { ActualizarTextosSliders(); });
        sliderYxs.onValueChanged.AddListener(delegate { ActualizarTextosSliders(); });

        textConsole.text = "Sistema listo. Esperando parámetros...";
        ActualizarTextosSliders(); 

        if (analyticChart != null)
        {
            analyticChart.ActualizarLineWeaverBurk(sliderUmax.value, sliderKs.value);
        }
    }

    private void ActualizarTextosSliders()
    {
        indicatorS0.text = sliderS0.value.ToString("F2");
        indicatorX0.text = sliderX0.value.ToString("F2");
        indicatorV0.text = sliderV0.value.ToString("F2");
        
        indicatorUmax.text = sliderUmax.value.ToString("F2");
        indicatorKs.text = sliderKs.value.ToString("F2");
        indicatorYxs.text = sliderYxs.value.ToString("F2");

        if (analyticChart != null)
        {
            analyticChart.ActualizarLineWeaverBurk(sliderUmax.value, sliderKs.value);
        }
    }

    public void OnIniciarSimulacionClicked()
    {
        textConsole.text = "Simulación en curso...";
        bioChart.LimpiarGrafica();
        historialPuntos.Clear(); 
        
        timerGrafica = 0f; 
        currentS = sliderS0.value;
        currentX = sliderX0.value;
        simUmax = sliderUmax.value;
        simKs = sliderKs.value;
        simYxs = sliderYxs.value;
        timeElapsed = 0;
        textSustratoAgotado.text = "Sustrato agotado en: -- hrs";
        yaSeAgoto = false;

        isSimulating = true;
    }

    void Update()
    {
        if (!isSimulating) return;

        double dt = Time.deltaTime * simSpeed;
        timeElapsed += dt;

        double currentU = (simUmax * currentS) / (simKs + currentS);

        double deltaX = currentU * currentX * dt;
        double deltaS = -(deltaX / simYxs);

        currentX += deltaX;
        currentS += deltaS;

        // NUEVO: Candado matemático para evitar que el sustrato baje a números negativos (ej. -0.05)
        if (currentS < 0) currentS = 0;

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

        timerGrafica += (float)dt;
        if (timerGrafica >= intervaloMuestreo)
        {
            bioChart.AgregarPunto((float)timeElapsed, (float)currentX, (float)currentS);
            
            PuntoSimulado nuevoPunto;
            nuevoPunto.tiempo = (float)timeElapsed;
            nuevoPunto.biomasa = (float)currentX;
            nuevoPunto.sustrato = (float)currentS;
            historialPuntos.Add(nuevoPunto);

            timerGrafica = 0f;
        }

        // --- MODIFICACIÓN CLAVE: DETENER SIMULACIÓN ---
        if (currentS <= 0.01 && !yaSeAgoto)
        {
            yaSeAgoto = true; 
            textSustratoAgotado.text = $"Sustrato agotado en:\n{timeElapsed:F2} hrs";
            
            // 1. Cambiamos el texto de la consola para dar feedback
            textConsole.text = $"Simulación finalizada: Sustrato agotado a las {timeElapsed:F2} hrs.";
            
            // 2. Apagamos el motor de simulación. Esto congela la gráfica y los cálculos al instante.
            isSimulating = false; 
        }
    }

    public void SetSpeed1X() { simSpeed = 1f; }
    public void SetSpeed2X() { simSpeed = 2f; }
    public void SetSpeed5X() { simSpeed = 5f; }

    public void DetenerSimulacion()
    {
        if (!isSimulating) return; 
        isSimulating = false; 
        textConsole.text = "Simulación interrumpida por el usuario.";

        if (!yaSeAgoto) 
        {
            textSustratoAgotado.text = "Sustrato agotado en:\nNo agotado (Interrumpido)";
        }
    }

    public void ReiniciarSimulacion() 
    { 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
        bioChart.LimpiarGrafica();
        historialPuntos.Clear();
        textTiempo.text = "Tiempo transcurrido: --";
        textProductividad.text = "Productividad: --";
    }
}