using UnityEngine;

public class WindSystem : MonoBehaviour
{
    [Header("Wind Speed")]
    public float averageWindSpeed = 10f;
    public float fluctuationAmplitude = 2f;
    public float changeFrequency = 0.1f;

    [Header("Wind Direction")]
    public float averageWindAngle = 0f;
    public float angleFluctuationAmplitude = 15f;
    public float angleChangeFrequency = 0.05f;

    private float _currentWindSpeedKnots;
    private float _windAngle;

    void Start()
    {
        _currentWindSpeedKnots = averageWindSpeed;
        _windAngle = averageWindAngle;
    }

    void Update()
    {
        float speedPerlinValue = Mathf.PerlinNoise(Time.time * changeFrequency, 0f);
        float speedFluctuation = Mathf.Lerp(-fluctuationAmplitude, fluctuationAmplitude, speedPerlinValue);
        _currentWindSpeedKnots = averageWindSpeed + speedFluctuation;

        float anglePerlinValue = Mathf.PerlinNoise(Time.time * angleChangeFrequency, 1.0f);
        float angleFluctuation = Mathf.Lerp(-angleFluctuationAmplitude, angleFluctuationAmplitude, anglePerlinValue);
        _windAngle = averageWindAngle + angleFluctuation;
    }

    public float GetWindSpeedMS()
    {
        return _currentWindSpeedKnots * 0.5144f;
    }

    public float GetWindSpeedKnots()
    {
        return _currentWindSpeedKnots;
    }

    public float GetWindAngle()
    {
        return _windAngle;
    }
}