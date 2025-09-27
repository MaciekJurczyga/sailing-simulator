using TMPro;
using UnityEngine;


public class BoatStatistics:MonoBehaviour
{
    
    private float _trueWindAttackAngle = 0f;
    private float _apparentWindAttackAngle = 0f;
    private float _currentBoatSpeed = 0f;
    private float _windSpeed = 0f;
    
    public TextMeshProUGUI boatSpeedText;
    public TextMeshProUGUI apparentWindAngleText;
    public TextMeshProUGUI trueWindAngleText;
    public TextMeshProUGUI windSpeedText;
    
    public void UpdateStats(BoatData foundBoatData, float currentBoatSpeed, float windSpeed)
    {
        if (foundBoatData.wDeg == 0)
        {
           
            _trueWindAttackAngle = foundBoatData.vDeg;
        }

        _trueWindAttackAngle = foundBoatData.wDeg;
        _apparentWindAttackAngle = foundBoatData.vDeg;
        _currentBoatSpeed = currentBoatSpeed;
        _windSpeed = windSpeed;
        UpdateText();
        
    }

    private void UpdateText()
    {
        windSpeedText.text = $"Wind Speed [knots]: {_windSpeed:F1}";
        trueWindAngleText.text = $"True Wind Attack Angle [°]: {_trueWindAttackAngle:F1}";
        apparentWindAngleText.text = $"Apparent Wind Attack Angle [°]: {_apparentWindAttackAngle:F1}";
        boatSpeedText.text = $"Sailboat Speed [knots]: {_currentBoatSpeed:F1}";
    }


}