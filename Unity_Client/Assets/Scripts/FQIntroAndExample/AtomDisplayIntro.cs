using UnityEngine;
using System.Collections.Generic;

public class AtomDisplayIntro : MonoBehaviour
{
    [Header("Núcleo")]
    public GameObject protonPrefab;
    public GameObject neutronPrefab;
    public Transform nucleusCenter;
    public float nucleusRadius = 0.3f;

    [Header("Electrones")]
    public GameObject electronPrefab;
    public float[] orbitRadii = { 1.2f, 2.0f, 2.8f };

    [Header("Órbitas visuales")]
    public List<GameObject> orbitRings;

    [Header("Velocidad de rotación")]
    public float rotationSpeed = 30f;

    private List<GameObject> nucleusParticles = new List<GameObject>();
    private List<List<GameObject>> orbitElectrons = new List<List<GameObject>>();
    private AtomState currentState = AtomState.Default;

    // Configuración por estado
    // Default: 3 protones, 4 neutrones, 4 electrones (Berilio aprox)
    // ShowNucleusAndElectrons: resalta núcleo y electrones
    // ShowNumbers: 6 protones, 6 neutrones, 6 electrones (Carbono)
    // ShowOrbits: muestra claramente las 3 órbitas con electrones

    private readonly int[] defaultProtons   = { 3, 3, 6, 8 };
    private readonly int[] defaultNeutrons  = { 4, 4, 6, 8 };
    private readonly int[] defaultElectrons = { 3, 3, 6, 8 };
    private Vector3 worldCenter;

    void Start()
    {
        // Convierte el centro del AtomViewport a coordenadas de mundo
        RectTransform viewport = GameObject.Find("AtomViewport")
            .GetComponent<RectTransform>();

        Vector3[] corners = new Vector3[4];
        viewport.GetWorldCorners(corners);

        worldCenter = (corners[0] + corners[2]) / 2f;
        worldCenter.z = 0;

        Debug.Log("Centro calculado: " + worldCenter);

        for (int i = 0; i < orbitRadii.Length; i++)
            orbitElectrons.Add(new List<GameObject>());

        SetState(AtomState.Default);
    }

    void Update()
    {
        for (int i = 0; i < orbitElectrons.Count; i++)
        {
            float speed = rotationSpeed * (i % 2 == 0 ? 1f : -1f);
            float angle = speed * Time.deltaTime * Mathf.Deg2Rad;

            foreach (GameObject e in orbitElectrons[i])
            {
                if (e == null) continue;

                Vector3 dir = e.transform.position - worldCenter;
                float cos = Mathf.Cos(angle);
                float sin = Mathf.Sin(angle);

                float newX = dir.x * cos - dir.y * sin;
                float newY = dir.x * sin + dir.y * cos;

                e.transform.position = worldCenter + new Vector3(newX, newY, 0);
            }
        }
    }

    public void SetState(AtomState state)
    {
        currentState = state;

        LimpiarAtomo();

        int index = (int)state;
        int protons   = defaultProtons[index];
        int neutrons  = defaultNeutrons[index];
        int electrons = defaultElectrons[index];

        SpawnNucleus(protons, neutrons);
        SpawnElectrons(electrons);
        ActualizarOrbitas(state);
    }

    void SpawnNucleus(int protons, int neutrons)
    {
        for (int i = 0; i < protons; i++)
        {
            Vector3 pos = worldCenter + (Vector3)Random.insideUnitCircle * nucleusRadius;
            GameObject p = Instantiate(protonPrefab, pos, Quaternion.identity, transform);
            DesactivarFisica(p);
            nucleusParticles.Add(p);
        }

        for (int i = 0; i < neutrons; i++)
        {
            Vector3 pos = worldCenter + (Vector3)Random.insideUnitCircle * nucleusRadius;
            GameObject n = Instantiate(neutronPrefab, pos, Quaternion.identity, transform);
            DesactivarFisica(n);
            nucleusParticles.Add(n);
        }
    }

    void SpawnElectrons(int total)
    {
        int[] capacity = { 2, 8, 18 };
        int remaining = total;

        for (int ring = 0; ring < orbitRadii.Length && remaining > 0; ring++)
        {
            int count = Mathf.Min(remaining, capacity[ring]);
            remaining -= count;

            for (int i = 0; i < count; i++)
            {
                float angle = (360f / count) * i * Mathf.Deg2Rad;
                Vector3 pos = worldCenter + new Vector3(
                    Mathf.Cos(angle) * orbitRadii[ring],
                    Mathf.Sin(angle) * orbitRadii[ring],
                    0
                );

                GameObject e = Instantiate(electronPrefab, pos, 
                    Quaternion.identity, transform);
                DesactivarFisica(e);
                orbitElectrons[ring].Add(e);
            }
        }
    }

    void ActualizarOrbitas(AtomState state)
    {
        if (orbitRings == null) return;

        foreach (GameObject ring in orbitRings)
        {
            if (ring != null)
                ring.SetActive(true);
        }
    }

    void DesactivarFisica(GameObject obj)
    {
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        Collider2D col = obj.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Particle p = obj.GetComponent<Particle>();
        if (p != null) p.enabled = false;
    }

    void LimpiarAtomo()
    {
        foreach (GameObject p in nucleusParticles)
            if (p != null) Destroy(p);
        nucleusParticles.Clear();

        foreach (List<GameObject> ring in orbitElectrons)
        {
            foreach (GameObject e in ring)
                if (e != null) Destroy(e);
            ring.Clear();
        }
    }

    public void SetStateEx(AtomStateEx state, int protons, int neutrons, int electrons)
    {
        LimpiarAtomo();

        SpawnNucleus(protons, neutrons);
        SpawnElectrons(electrons);
        ActualizarOrbitas(AtomState.Default);
    }

    public void InicializarCentro()
    {
        Canvas.ForceUpdateCanvases();

        RectTransform viewport = GameObject.Find("AtomViewport")
            .GetComponent<RectTransform>();

        Vector3[] corners = new Vector3[4];
        viewport.GetWorldCorners(corners);

        worldCenter = (corners[0] + corners[2]) / 2f;
        worldCenter.z = 0;
    }
}