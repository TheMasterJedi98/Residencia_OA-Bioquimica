using System;
using System.Linq;
using UnityEngine;

public class BiocineticsEngine : MonoBehaviour
{
    /// <summary>
    /// Calcula Vmax, Km y R^2 usando la regresión lineal de Lineweaver-Burk.
    /// </summary>
    public static void CalculateParameters(double[] S, double[] v, out double Vmax, out double Km, out double rSquared)
    {
        int n = S.Length;
        double[] x = new double[n];
        double[] y = new double[n];

        // Convertir a inversas (Lineweaver-Burk)
        for (int i = 0; i < n; i++)
        {
            x[i] = 1.0 / S[i];
            y[i] = 1.0 / v[i];
        }

        // Sumatorias para la regresión lineal (y = mx + b)
        double sumX = x.Sum();
        double sumY = y.Sum();
        double sumXY = 0;
        double sumX2 = 0;
        double sumY2 = 0;

        for (int i = 0; i < n; i++)
        {
            sumXY += x[i] * y[i];
            sumX2 += x[i] * x[i];
            sumY2 += y[i] * y[i];
        }

        // Cálculo de pendiente (m) y ordenada al origen (b)
        double m = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
        double b = (sumY - m * sumX) / n;

        // Despeje de parámetros cinéticos
        Vmax = 1.0 / b;
        Km = m * Vmax;

        // Cálculo del coeficiente de determinación R^2
        double sst = sumY2 - (sumY * sumY) / n;
        double sse = 0;
        for (int i = 0; i < n; i++)
        {
            double yPred = m * x[i] + b;
            sse += (y[i] - yPred) * (y[i] - yPred);
        }
        rSquared = 1.0 - (sse / sst);
    }

    /// <summary>
    /// Predice la velocidad utilizando la ecuación de Michaelis-Menten.
    /// </summary>
    public static double PredictVelocity(double S, double Vmax, double Km)
    {
        return (Vmax * S) / (Km + S);
    }
}