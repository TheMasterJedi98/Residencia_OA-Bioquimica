using System;

[Serializable]
public class SimulationData
{
    public string studentId;
    public double[] substrateConcentrations;
    public double[] velocities;
    public double calculatedVmax;
    public double calculatedKm;
    public double rSquared;
    public string timestamp;

    public SimulationData(string id, double[] s, double[] v, double vmax, double km, double r2)
    {
        studentId = id;
        substrateConcentrations = s;
        velocities = v;
        calculatedVmax = vmax;
        calculatedKm = km;
        rSquared = r2;
        timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}