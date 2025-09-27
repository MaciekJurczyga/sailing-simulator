using UnityEngine;

public class SailController : MonoBehaviour
{
    public float sailsRotationSpeed = 50f;
    public float tackingSpeedMultiplier = 1.5f; 
    public float tackingAngleThreshold = 1f; 

    [Header("Ustawienia Obrotu Pionowego (Bom)")]
    public Transform bomTransform;

    private BoatController _boatController;
    private float currentFokAngle = 0f;
    private Quaternion fokStartRotation;
    private float currentBomAngle = 0f;
    private Quaternion bomStartRotation;

    private SailPosition currentSailPosition;
    private SailControlState sailControlState = SailControlState.Manual;
  
    private const float _fullMinAngle = -83f;
    private const float _fullMaxAngle = 83f;
    
    private const float _leftSideMinAngle = 0f;
    private const float _leftSideMaxAngle = 83f;
    
    private const float _rightSideMinAngle = -83f;
    private const float _rightSideMaxAngle = 0f;

    private float _tackTargetAngle;
    private Tack currentTack;

    public void Initialize()
    {
    }

    void Start()
    {
        if (bomTransform == null)
        {
            Debug.LogError("Pole 'Bom Transform' nie zostało przypisane w Inspektorze! Skrypt nie będzie działał poprawnie.");
            this.enabled = false;
            return;
        }

        fokStartRotation = transform.localRotation;
        bomStartRotation = bomTransform.localRotation;
        
        currentBomAngle = 0f;
        currentFokAngle = 0f;
    }

    void Update()
    {
        
        SailPosition requiredSailPosition = GetRequiredSailPositionForTack(currentTack);
        
        UpdateCurrentSailPosition(requiredSailPosition);

        if (sailControlState == SailControlState.Tacking)
        {
            HandleAutomaticTacking();
        }
        else
        {
            if (currentSailPosition != requiredSailPosition)
            {
                StartTacking();
            }
            else
            {
                HandleManualSailControl();
            }
        }

        ApplySailRotations();
    }

    private void HandleManualSailControl()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            if (currentSailPosition == SailPosition.Left)
            {
                currentFokAngle += sailsRotationSpeed * Time.deltaTime;
                currentBomAngle += sailsRotationSpeed * Time.deltaTime;
            }
            else
            {
                currentFokAngle -= sailsRotationSpeed * Time.deltaTime;
                currentBomAngle -= sailsRotationSpeed * Time.deltaTime;
            }
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            if (currentSailPosition == SailPosition.Left)
            {
                currentFokAngle -= sailsRotationSpeed * Time.deltaTime;
                currentBomAngle -= sailsRotationSpeed * Time.deltaTime;
            }
            else
            {
                currentFokAngle += sailsRotationSpeed * Time.deltaTime;
                currentBomAngle += sailsRotationSpeed * Time.deltaTime;
            }
        }
        
        ClampSailsToActiveSide();
    }

    private void HandleAutomaticTacking()
    {
        float step = sailsRotationSpeed * tackingSpeedMultiplier * Time.deltaTime;
        currentFokAngle = Mathf.MoveTowards(currentFokAngle, _tackTargetAngle, step);
        currentBomAngle = Mathf.MoveTowards(currentBomAngle, _tackTargetAngle, step);

        if (Mathf.Abs(currentBomAngle - _tackTargetAngle) < tackingAngleThreshold)
        {
            sailControlState = SailControlState.Manual;
            currentBomAngle = _tackTargetAngle;
            currentFokAngle = _tackTargetAngle;
            SailPosition requiredSailPosition = GetRequiredSailPositionForTack(currentTack);
            UpdateCurrentSailPosition(requiredSailPosition);
            ClampSailsToActiveSide();
        }
    }

    private void StartTacking()
    {
        sailControlState = SailControlState.Tacking;

        _tackTargetAngle = -currentBomAngle;

        if (Mathf.Abs(_tackTargetAngle) < tackingAngleThreshold * 2)
        {
            
            SailPosition requiredPosition = GetRequiredSailPositionForTack(currentTack);

            if (requiredPosition == SailPosition.Left)
            {
                _tackTargetAngle = (_leftSideMinAngle + _leftSideMaxAngle) / 2f; 
            }
            else
            {
                _tackTargetAngle = (_rightSideMinAngle + _rightSideMaxAngle) / 2f;
            }
        }
        
        _tackTargetAngle = Mathf.Clamp(_tackTargetAngle, _fullMinAngle, _fullMaxAngle);
    }
    
    private void UpdateCurrentSailPosition(SailPosition requiredSailPosition)
    {
        if (currentBomAngle > 0f + tackingAngleThreshold / 2f)
        {
            currentSailPosition = SailPosition.Left;
        }
        else if (currentBomAngle < 0f - tackingAngleThreshold / 2f)
        {
            currentSailPosition = SailPosition.Right;
        }
        else
        {
            currentSailPosition = requiredSailPosition;
        }
    }

    private SailPosition GetRequiredSailPositionForTack(Tack tack)
    {
        return (tack == Tack.Left) ? SailPosition.Right : SailPosition.Left;
    }

    private void ClampSailsToActiveSide()
    {
        if (currentSailPosition == SailPosition.Left)
        {
            currentFokAngle = Mathf.Clamp(currentFokAngle, _leftSideMinAngle, _leftSideMaxAngle);
            currentBomAngle = Mathf.Clamp(currentBomAngle, _leftSideMinAngle, _leftSideMaxAngle);
        }
        else
        {
            currentFokAngle = Mathf.Clamp(currentFokAngle, _rightSideMinAngle, _rightSideMaxAngle);
            currentBomAngle = Mathf.Clamp(currentBomAngle, _rightSideMinAngle, _rightSideMaxAngle);
        }
    }

    private void ApplySailRotations()
    {
        Quaternion fokRotation = Quaternion.Euler(0, currentFokAngle, 0);
        transform.localRotation = fokStartRotation * fokRotation;

        Quaternion bomRotation = Quaternion.Euler(0, 0, currentBomAngle);
        bomTransform.localRotation = bomStartRotation * bomRotation;
    }

    public void setCurrentTack(Tack tack)
    {
        currentTack = tack;
    }

    public float getSailsAngle()
    {
        return currentBomAngle;
    }
}
