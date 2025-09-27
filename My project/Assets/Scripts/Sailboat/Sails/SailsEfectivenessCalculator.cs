using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SailsEfectivenessCalculator
{
    private static float optimalSailsAttackAngle = 15;
    
    private static readonly Dictionary<int, int> EffectivenessMap;
    private static readonly Dictionary<int, float> ldairtoS0scaler;

    static SailsEfectivenessCalculator()
    {
        EffectivenessMap = new Dictionary<int, int>
        {
            { -11, 1 }, 
            { -10, 5 }, 
            { -9, 12 }, 
            { -8, 18 }, 
            { -7, 25 },
            { -6, 32 }, 
            { -5, 38 }, 
            { -4, 43 }, 
            { -3, 46 }, 
            { -2, 48 },
            { -1, 49 }, 
            { 0, 50 }, 
            { 1, 47 }, 
            { 2, 44 }, 
            { 3, 41 },
            { 5, 40 }, 
            { 7, 38 }, 
            { 9, 37 }, 
            { 11, 35 }, 
            { 13, 33 },
            { 15, 32 }, 
            { 18, 30 }, 
            { 21, 30 }, 
            { 24, 30 }, 
            { 27, 28 },
            { 30, 26 }, 
            { 33, 24 }, 
            { 36, 22 }, 
            { 39, 20 }, 
            { 42, 18 },
            { 45, 16 }, 
            { 48, 15 },
            { 51, 14 }, 
            { 53, 13 }, 
            { 55, 12 },
            { 57, 11 }, 
            { 60, 10 }, 
            { 62, 9 }, 
            { 64, 8 }, 
            { 66, 7 },
            { 68, 6 }, 
            { 70, 5 }, 
            { 72, 4 }, 
            { 74, 4 }, 
            { 76, 3 },
            { 78, 3 }, 
            { 80, 2 }, 
            { 81, 2 }, 
            { 82, 1 }, 
            { 83, 1 },
        };
        
    }

    public static int findLdAir(BoatData perfectBoatData, float sailsAngle)
    {
        float normalizedV = normalizeVDeg(perfectBoatData);
        float normalizedSailsAngle = normalizeSailsAngle(sailsAngle);
        float perfectSailsAngle = findPerfectSailsPosition(normalizedV);
        
        float difference = compare(perfectSailsAngle, normalizedSailsAngle);
        
        int foundLdAir = GetEffectiveness(difference);
        
        return foundLdAir;
    }

    private static int GetEffectiveness(float angleDifference)
    {
        int closestKey = EffectivenessMap.Keys
            .OrderBy(key => Math.Abs(key - angleDifference))
            .First();
        
        return EffectivenessMap[closestKey];
    }
    
    private static float compare(float perfect, float actual)
    {
        return perfect - actual;
    }
    
    private static float findPerfectSailsPosition(float normalizedV)
    {
        if (normalizedV - optimalSailsAttackAngle > 0 && normalizedV - optimalSailsAttackAngle < 83)
        {
            return normalizedV - optimalSailsAttackAngle;
        }

        if (normalizedV - optimalSailsAttackAngle < 0)
        {
            return 0;
        }

        return 83;
    }

    private static float normalizeSailsAngle(float sailsAngle)
    {
        if (sailsAngle >= 0 && sailsAngle <= 83)
        {
            return sailsAngle;
        }

        return Math.Abs(sailsAngle);
    }
    
    private static float normalizeVDeg(BoatData perfectBoatData)
    {
        float v = perfectBoatData.vDeg;

        if (v >= 0 && v <= 180)
        {
            return v;
        }

        return 360 - v;
    }
}
