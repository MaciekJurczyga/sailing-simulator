using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PhysicsModel
{

    private PhysicsCalculator _physics;
    private List<BoatData> _boatDataList = new List<BoatData>();
    private Dictionary<int, List<BoatData>> _sortedBoatDataListByLDAirMap = new Dictionary<int, List<BoatData>>();
    private int LDairScaler = 10;
    private WindSystem _windSystem;

    public PhysicsModel(WindSystem windSystem, PhysicsCalculator physicsCalculator)
    {
        _windSystem = windSystem;
        _physics = physicsCalculator;
    }
    

    public List<BoatData> GetBoatDataForBestLD()
    {
        return _sortedBoatDataListByLDAirMap[PhysicsCalculator.MaxLiftToDragAirRation * LDairScaler];
    }
    public void LoadModel()
    {
        for (float ldAir = 1; ldAir <= PhysicsCalculator.MaxLiftToDragAirRation*LDairScaler; ldAir++)
        {
            int steps = 18000; // 180 / 0.001
            for (int i = 0; i < steps; i++)
            {
                float vDeg = i * 0.01f;
                _physics.Calculate(vDeg, ldAir/LDairScaler);
                float wDeg = _physics.GetTrueWindAttackAngle();
                float boatSpeed = _physics.GetBoatSpeed();

                _boatDataList.Add(new BoatData(vDeg, wDeg, boatSpeed));
            }
            FillMissingWDegValues();
            for (int i = 0; i < steps; i++)
            {
                var original = _boatDataList[i];
                _boatDataList.Add(new BoatData(
                    360f - original.vDeg,
                    360f - original.wDeg,
                    original.CalculatedBoatSpeedWithoutWindSpeed
                ));
            }

            _sortedBoatDataListByLDAirMap[(int)ldAir] = _boatDataList.OrderBy(data => data.wDeg).ToList();  
            _boatDataList.Clear();
        }
    }

    private void FillMissingWDegValues()
    {
        int firstNonZeroIndex = _boatDataList.FindIndex(data => data.wDeg != 0);

        if (firstNonZeroIndex <= 0)
            return; 

        float delta = _boatDataList[firstNonZeroIndex].wDeg / firstNonZeroIndex;

        for (int i = 1; i < firstNonZeroIndex; i++)
        {
            _boatDataList[i].wDeg = _boatDataList[i - 1].wDeg + delta;
        }
    }
    
    public float FindLeewayAngle(BoatData boatData)
    {
        float vDeg = boatData.vDeg;

        // bagsztag/fordewind — brak dryfu
        if (vDeg > 135f && vDeg < 225f)
        {
            return 0f;
        }

        // TODO: zapytac się o wzór bo z arctg(LDWody), bo wychodzi 86 stopni
        float baseLeeway = 5f;

        // Lewy hals: vDeg < 180 → dryf w prawo → dodatni kąt dryfu
        // Prawy hals: vDeg > 180 → dryf w lewo → ujemny kąt dryfu
        
        float signedLeeway = (vDeg < 180f) ? baseLeeway : -baseLeeway;

        return signedLeeway;
    }


    public BoatData getBoatData(float targetAttackAngle)
    {
        int foundLDAir = 50; // TODO: add code to calculate this value
     //   float targetAttackAngle = CalculateAttackAngle(boatAngle);
        List<BoatData> sortedBoatDataList = _sortedBoatDataListByLDAirMap[foundLDAir];

        if (sortedBoatDataList == null || sortedBoatDataList.Count == 0)
            return new BoatData(0, 0, 0);

        int left = 0;
        int right = sortedBoatDataList.Count - 1;
        BoatData closest = sortedBoatDataList[0];
        float smallestDiff = float.MaxValue;

        while (left <= right)
        {
            int mid = (left + right) / 2;
            var midData = sortedBoatDataList[mid];
            float diff = Mathf.Abs(midData.wDeg - targetAttackAngle);

            if (diff < smallestDiff)
            {
                smallestDiff = diff;
                closest = midData;
            }

            if (midData.wDeg < targetAttackAngle)
                left = mid + 1;
            else
                right = mid - 1;
        }

        return closest;
    }
}
