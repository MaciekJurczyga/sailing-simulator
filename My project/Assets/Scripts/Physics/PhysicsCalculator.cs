﻿using UnityEngine;


/**
 * v - apparent wind attack angle
 * w - true wind attack angle
 * sOfV = s(v) speed ratio parameter without including type of sailing
 * adjustedSOfV = S(V) speed ration parameter including type of sailing
 *
 */
public class PhysicsCalculator
{
    public static int MaxLiftToDragAirRation = 5;
    private const float LiftToDragWaterRatio = 15f;
    private float S0;
    private static float _borderAngleRad = 0;
    
    private float _sOfV;
    private float _adjustedSOfV;
    private float _wRad;
    private float _wDeg;
    private float _boatSpeed;

    public void Calculate(float vDeg, float liftToDragAirRatio)
    {
        S0 = determineS0(liftToDragAirRatio);
        _borderAngleRad = CalculateBorderAngleRad(liftToDragAirRatio);
        float vRad = vDeg * Mathf.Deg2Rad;

        int deadAngleCheck1 = Mathf.Cos(_borderAngleRad - vRad) >= 0 ? 1 : 0;
        int deadAngleCheck2 = Mathf.Pow(Mathf.Tan((_borderAngleRad - vRad)) / LiftToDragWaterRatio, 2) <= 1 ? 1 : 0;

        _sOfV = CalculateSOfV(deadAngleCheck1, deadAngleCheck2, vRad);
        _adjustedSOfV = CalculateAdjustedSOfV(vRad);
        _wRad = CalculateWRad(vRad);
        _wDeg = CalculateWDegrees();
        _boatSpeed = CalculateBoatSpeedWithoutWindSpeed(vRad);
    }

    private float determineS0(float liftToDragAirRatio)
    {
        if (liftToDragAirRatio > 4.5f)
        {
            return 1.5f;
        }
        if (liftToDragAirRatio > 4.0f)
        {
            return 1.35f;
        }
        if (liftToDragAirRatio > 3.5f)
        {
            return 1.2f;
        }
        if (liftToDragAirRatio > 3.0f)
        {
            return 1.05f;
        }
        if (liftToDragAirRatio > 2.5f)
        {
            return 0.85f;
        }
        if (liftToDragAirRatio > 2.0f)
        {
            return 0.65f;
        }
        if (liftToDragAirRatio > 1.5f)
        {
            return 0.5f;
        }
        if (liftToDragAirRatio > 1.0f)
        {
            return 0.35f;
        }

        return 0f;
    }
    

    public float CalculateBorderAngleRad(float liftToDragAirRation)
    {
        return  -(Mathf.Atan(liftToDragAirRation) - Mathf.PI);
    }
    private float CalculateSOfV(int check1, int check2, float vRad)
    {
        if (check1 + check2 != 2) return 0;
        return 0.5f * S0 * S0 * Mathf.Cos(_borderAngleRad - vRad) *
               (1 + Mathf.Sqrt(1 - Mathf.Pow(Mathf.Tan(_borderAngleRad - vRad) / LiftToDragWaterRatio, 2)));
    }

    private float CalculateAdjustedSOfV(float vRad)
    {
        return (vRad < _borderAngleRad) ? _sOfV : S0 * S0;
    }

    private float CalculateWRad(float vRad)
    {
        if (_adjustedSOfV > 0)
        {
            return Mathf.Atan(Mathf.Sin(vRad) / (Mathf.Cos(vRad) - _adjustedSOfV));
        }
        return 0;
    }

    private float CalculateWDegrees()
    {
        float angleInDegrees = _wRad * Mathf.Rad2Deg;
        return (angleInDegrees >= 0) ? angleInDegrees : (180 + angleInDegrees);
    }

    private float CalculateBoatSpeedWithoutWindSpeed(float vRad)
    {
        if (_adjustedSOfV > 0)
        {
            return _adjustedSOfV / Mathf.Sqrt(1 + _adjustedSOfV * _adjustedSOfV - 2 * _adjustedSOfV * Mathf.Cos(vRad));
        }
        return 0;
    }

    public float GetBoatSpeed()
    {
        return _boatSpeed;
    }

    public float GetTrueWindAttackAngle()
    {
        return _wDeg;
    }
    

    public float GetLDWater()
    {
        return LiftToDragWaterRatio;
    }
}