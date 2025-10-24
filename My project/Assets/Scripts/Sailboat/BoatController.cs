using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    private Rigidbody _rb;
    private WindIndicatorController _windIndicatorController;
    private WindSystem _windSystem;
    private PhysicsModel _physicsModel;
    private FokMaterialSwapper _fokMaterialSwapper;
    private GrotMaterialSwapper _grotMaterialSwapper;
    private GraphPointsWrapper _graphPointsWrapper;
    public GraphDrawer graphDrawer;
    private SailController _sailController;
    public TextMeshProUGUI endOfMapText;
    public Material red;
    public Material orange;
    public Material defaultMaterial;
    
    public float turnSpeed = 25f;
    public float tau = 2.5f;
    
    private float currentSpeed;
    
    public void Initialize(PhysicsModel physicsModel,
        WindSystem windSystem,
        GraphPointsWrapper graphPointsWrapper,
        SailController sailController,
        FokMaterialSwapper fokMaterialSwapper,
        GrotMaterialSwapper grotMaterialSwapper
        )
    {
        _sailController = sailController;
        _physicsModel = physicsModel;
        _windSystem = windSystem;
        _graphPointsWrapper = graphPointsWrapper;
        _fokMaterialSwapper = fokMaterialSwapper;
        _grotMaterialSwapper = grotMaterialSwapper;
    }
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _windIndicatorController = GetComponent<WindIndicatorController>();
        _rb.isKinematic = false;
    }

    void FixedUpdate()
    {
        float windSpeed = _windSystem.GetWindSpeedKnots();
        float realWindAttackAngle = CalculateAttackAngle(transform.eulerAngles.y);
        _sailController.setCurrentTack(GetCurrentTack(realWindAttackAngle));
        BoatData perfectBoatData = _physicsModel.getBoatDataForGivenLdAir(realWindAttackAngle, 50);
        int foundLdAir = SailsEfectivenessCalculator.findLdAir(perfectBoatData, _sailController.getSailsAngle());
        BoatData foundBoatData = _physicsModel.getBoatDataForGivenLdAir(realWindAttackAngle, foundLdAir);
        if (foundBoatData.CalculatedBoatSpeedWithoutWindSpeed * windSpeed == 0)
        {
            foundBoatData = new BoatData(realWindAttackAngle, realWindAttackAngle, 0);
        }
        changeSailsColor(foundLdAir);
        float leewayAngle = _physicsModel.FindLeewayAngle(foundBoatData);
        MoveBoat(foundBoatData.CalculatedBoatSpeedWithoutWindSpeed * windSpeed, leewayAngle);
        TurnBoat();
        _windIndicatorController.SetWindAngle(foundBoatData);
        graphDrawer.DrawUserPoint(foundBoatData, windSpeed);
        graphDrawer.UpdateGraphView(windSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WaterBorderTag"))
        {
            transform.Rotate(Vector3.up, 180f);

            StartCoroutine(ShowTextForSeconds(2));
        }
    }

    private IEnumerator ShowTextForSeconds(float seconds)
    {
        endOfMapText.gameObject.SetActive(true); 
        yield return new WaitForSeconds(seconds); 
        endOfMapText.gameObject.SetActive(false); 
    }
    
    private void MoveBoat(float targetSpeed, float leewayAngle)
    {
        // apply boat acceleration to target speed
        // if boat is in dead angle and its speed is 0, increase tau for more realistic slowing down
        tau = targetSpeed == 0 ? 5f : 2.5f;
        currentSpeed += (-1 / tau) * (currentSpeed - targetSpeed) * Time.deltaTime;
        Vector3 driftDirection = Quaternion.Euler(0, leewayAngle, 0) * transform.forward;

        _rb.MovePosition(_rb.position + Time.deltaTime * currentSpeed * driftDirection.normalized);
    }

    private Tack GetCurrentTack(float realWindAttackAngle)
    {
        if (realWindAttackAngle >= 0 && realWindAttackAngle <= 180)
        {
            return Tack.Left;
        }

        return Tack.Right;
    }

    private void TurnBoat()
    {
        var turnInput = Input.GetAxis("Horizontal");
        var rotationAmount = turnInput * turnSpeed * Time.deltaTime;

        Quaternion turnRotation = Quaternion.Euler(0, rotationAmount, 0);
        _rb.MoveRotation(_rb.rotation * turnRotation);
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
    
    
    public float CalculateAttackAngle(float boatAngle)
    {
        // Calculates true wind attack angle:
        // 0-180 left tack pl: hals
        // 180-360 right tack
        if (_windSystem == null) return 0f;

        var trueWindAngle = _windSystem.GetWindAngle();

        var diff = boatAngle - trueWindAngle;

        // diff is in range -180:180, return is 0-360
        return diff >= 0 ? diff : 360 + diff;
    }

    private void changeSailsColor(float foundLdAir)
    {
        if (foundLdAir >= 25 && foundLdAir < 38)
        {
            _grotMaterialSwapper.SetMaterial(orange);
            _fokMaterialSwapper.SetMaterial(orange);
        }

       else if (foundLdAir < 25)
        {
            _grotMaterialSwapper.SetMaterial(red);
            _fokMaterialSwapper.SetMaterial(red);
        }
        else
        {
            _grotMaterialSwapper.SetMaterial(defaultMaterial);
            _fokMaterialSwapper.SetMaterial(defaultMaterial);
        }
    }
}