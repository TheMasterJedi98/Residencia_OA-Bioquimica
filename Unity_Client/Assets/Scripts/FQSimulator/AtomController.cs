using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class AtomController : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI txtElementName;

    [Header("Anillos - Transform de cada órbita")]
    public Transform orbit1;
    public Transform orbit2;
    public Transform orbit3;
    public Transform orbit4;

    [Header("Prefabs")]
    public GameObject electronVisualPrefab;
    public GameObject protonVisualPrefab;
    public GameObject neutronVisualPrefab;

    [Header("Configuración")]
    public float nucleusRadius = 0.3f;

    [Header("Radios de órbita")]
    public float[] orbitRadii = { 0.8f, 1.4f, 2.0f, 2.6f };
    // Estado del átomo
    private int protonCount   = 0;
    private int neutronCount  = 0;
    private int electronCount = 0;

    // Capacidades por anillo
    private readonly int[] orbitCapacity = { 2, 8, 18, 32 };

    // Electrones visuales por anillo
    private List<GameObject>[] orbitElectrons;
    private List<GameObject> nucleusParticles = new List<GameObject>();

    private AtomInfoPanel infoPanel;

    // Tabla periódica simplificada (número atómico → nombre)
    private readonly Dictionary<int, string> elementNames = new Dictionary<int, string>()
    {
        {0, ""}, {1, "Hidrógeno"}, {2, "Helio"}, {3, "Litio"},
        {4, "Berilio"}, {5, "Boro"}, {6, "Carbono"}, {7, "Nitrógeno"},
        {8, "Oxígeno"}, {9, "Flúor"}, {10, "Neón"}, {11, "Sodio"},
        {12, "Magnesio"}, {13, "Aluminio"}, {14, "Silicio"}, {15, "Fósforo"},
        {16, "Azufre"}, {17, "Cloro"}, {18, "Argón"}, {19, "Potasio"},
        {20, "Calcio"}, {21, "Escandio"}, {22, "Titanio"}, {23, "Vanadio"},
        {24, "Cromo"}, {25, "Manganeso"}, {26, "Hierro"}, {27, "Cobalto"},
        {28, "Níquel"}, {29, "Cobre"}, {30, "Zinc"}
    };

    void Awake()
    {
        orbitElectrons = new List<GameObject>[4];
        for (int i = 0; i < 4; i++)
            orbitElectrons[i] = new List<GameObject>();

        infoPanel = FindObjectOfType<AtomInfoPanel>();
    }

    // ── Añadir partículas al átomo ────────────────────────────

    public void AddProton()
    {
        protonCount++;
        Debug.Log($"[AtomController] AddProton llamado. ProtonCount ahora: {protonCount}");
        SpawnNucleusParticle(protonVisualPrefab);
        ActualizarNombre();
        NotificarCambio();
    }

    public void AddNeutron()
    {
        neutronCount++;
        SpawnNucleusParticle(neutronVisualPrefab);
        NotificarCambio();
    }

    public void AddElectron()
    {
        electronCount++;
        SpawnElectronVisual();
        NotificarCambio();
    }

    // ── Remover partículas del átomo ──────────────────────────

    public void RemoveProton()
    {
        if (protonCount <= 0) return;
        protonCount--;
        RemoveLastNucleusParticle(ParticleType.Proton);
        ActualizarNombre();
        NotificarCambio();
    }

    public void RemoveNeutron()
    {
        if (neutronCount <= 0) return;
        neutronCount--;
        RemoveLastNucleusParticle(ParticleType.Neutron);
        NotificarCambio();
    }

    public void RemoveElectron()
    {
        if (electronCount <= 0) return;
        electronCount--;
        RemoveLastElectronVisual();
        NotificarCambio();
    }

    // ── Núcleo ────────────────────────────────────────────────

    void SpawnNucleusParticle(GameObject prefab)
    {
        Vector3 center = transform.position;
        center += (Vector3)Random.insideUnitCircle * nucleusRadius;
        center.z = 0;

        GameObject p = Instantiate(prefab, center, Quaternion.identity);

        Rigidbody2D rb = p.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // Marca como partícula del núcleo y asigna referencias
        Particle particleScript = p.GetComponent<Particle>();
        if (particleScript != null)
        {
            particleScript.isInNucleus = true;
            particleScript.atomController = this;
            particleScript.particleSpawner = FindObjectOfType<ParticleSpawner>();
        }

        nucleusParticles.Add(p);
    }

    void RemoveLastNucleusParticle(ParticleType type)
    {
        for (int i = nucleusParticles.Count - 1; i >= 0; i--)
        {
            Particle p = nucleusParticles[i].GetComponent<Particle>();
            if (p != null && p.particleType == type)
            {
                Destroy(nucleusParticles[i]);
                nucleusParticles.RemoveAt(i);
                return;
            }
        }
    }

    // ── Electrones en órbita ──────────────────────────────────

    void SpawnElectronVisual()
    {
        for (int i = 0; i < 4; i++)
        {
            orbitElectrons[i].RemoveAll(e => e == null);

            if (orbitElectrons[i].Count < orbitCapacity[i])
            {
                Transform orbit = GetOrbit(i);
                if (orbit == null) return;

                GameObject e = Instantiate(electronVisualPrefab, transform.position, Quaternion.identity);
                
                Particle particleScript = e.GetComponent<Particle>();
                if (particleScript != null)
                {
                    particleScript.isInNucleus = true;
                    particleScript.atomController = this;
                    particleScript.particleSpawner = FindObjectOfType<ParticleSpawner>();
                }

                orbitElectrons[i].Add(e);
                ReposicionarElectrones(i);
                return;
            }
        }

        Debug.Log("Todos los anillos están llenos");
    }

    void RemoveLastElectronVisual()
    {
        // Remueve del último anillo con electrones
        for (int i = 3; i >= 0; i--)
        {
            if (orbitElectrons[i].Count > 0)
            {
                int last = orbitElectrons[i].Count - 1;
                Destroy(orbitElectrons[i][last]);
                orbitElectrons[i].RemoveAt(last);
                ReposicionarElectrones(i);
                return;
            }
        }
    }

    void ReposicionarElectrones(int orbitIndex)
    {
        // Limpiar referencias nulas antes de procesar
        orbitElectrons[orbitIndex].RemoveAll(e => e == null);

        List<GameObject> electrons = orbitElectrons[orbitIndex];
        int count = electrons.Count;
        if (count == 0) return;

        Vector3 center = transform.position;
        center.z = 0;

        float radius = GetOrbitRadius(orbitIndex);

        for (int i = 0; i < count; i++)
        {
            if (electrons[i] == null) continue;

            float angle = (360f / count) * i * Mathf.Deg2Rad;
            Vector3 pos = center + new Vector3(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius,
                0
            );
            electrons[i].transform.position = pos;

            Rigidbody2D rb = electrons[i].GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = 0f;
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
        }
    }

    public void RemoveSpecificElectron(GameObject electronObj)
    {
        for (int i = 0; i < 4; i++)
        {
            if (orbitElectrons[i].Contains(electronObj))
            {
                orbitElectrons[i].Remove(electronObj);
                electronCount--;
                ReposicionarElectrones(i);
                NotificarCambio();
                return;
            }
        }
    }

    public void RemoveSpecificProton(GameObject protonObj)
    {
        if (nucleusParticles.Contains(protonObj))
        {
            nucleusParticles.Remove(protonObj);
            protonCount--;
            NotificarCambio();
            ActualizarNombre();
        }
    }

    public void RemoveSpecificNeutron(GameObject neutronObj)
    {
        if (nucleusParticles.Contains(neutronObj))
        {
            nucleusParticles.Remove(neutronObj);
            neutronCount--;
            NotificarCambio();
        }
    }

    Transform GetOrbit(int index)
    {
        switch (index)
        {
            case 0: return orbit1;
            case 1: return orbit2;
            case 2: return orbit3;
            case 3: return orbit4;
            default: return null;
        }
    }

    float GetOrbitRadius(int index)
    {
        if (index < orbitRadii.Length)
            return orbitRadii[index];
        return 1f;
    }

    void NotificarCambio()
    {
        if (infoPanel != null)
            infoPanel.ActualizarInfo();
    }

    // ── Nombre del elemento ───────────────────────────────────

    void ActualizarNombre()
    {
        if (txtElementName == null) return;

        if (elementNames.TryGetValue(protonCount, out string nombre))
            txtElementName.text = nombre;
        else
            txtElementName.text = "Desconocido";
    }

    // ── Boton limpiar ──────────────────────────────────────
    public void LimpiarAtomo()
    {
        // Destruir partículas del núcleo
        foreach (GameObject p in nucleusParticles)
            if (p != null) Destroy(p);
        nucleusParticles.Clear();

        // Destruir electrones de todas las órbitas
        for (int i = 0; i < 4; i++)
        {
            foreach (GameObject e in orbitElectrons[i])
                if (e != null) Destroy(e);
            orbitElectrons[i].Clear();
        }

        // Resetear contadores
        protonCount   = 0;
        neutronCount  = 0;
        electronCount = 0;

        ActualizarNombre();

        NotificarCambio();
    }

    // ── Getters públicos ──────────────────────────────────────

    public int ProtonCount   => protonCount;
    public int NeutronCount  => neutronCount;
    public int ElectronCount => electronCount;
}