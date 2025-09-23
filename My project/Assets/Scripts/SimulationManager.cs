using UnityEngine;

public class SimulationManager : MonoBehaviour
{

    [SerializeField] private WindSystem _windSystem; 
    
    public BoatController boatController;
    public SailController sailController;
    public GraphDrawer graphDrawer;
    
    public WindIndicatorController windIndicatorController;
    
    private PhysicsModel _physicsModel;
    private GraphPointsWrapper _graphPointsWrapper;
    private PhysicsCalculator _physicsCalculator;

    void Awake()
    {

        if (_windSystem == null)
        {
            Debug.LogError("Referencja do WindSystem nie jest przypisana w SimulationManager!", this);
            return;
        }
        

        _physicsCalculator = new PhysicsCalculator();
        
        _physicsModel = new PhysicsModel(_windSystem, _physicsCalculator);
        _physicsModel.LoadModel();

        _graphPointsWrapper = new GraphPointsWrapper();
        _graphPointsWrapper.LoadPoints(_physicsModel.GetBoatDataForBestLD());
    }

    void Start()
    {
        if (boatController != null)
        {
            boatController.Initialize(_physicsModel, _windSystem, _graphPointsWrapper);
        }
        else
        {
            Debug.LogError("Boat controller is not assigned to SimulationManager!", this);
        }

        if (sailController != null)
        {
            sailController.Initialize(boatController);
        }

        if (graphDrawer != null)
        {
            graphDrawer.Initialize(_graphPointsWrapper);
        }

        if (windIndicatorController != null)
        {
            windIndicatorController.Initialize(_windSystem);
        }
        else
        {
            Debug.LogError("WindIndicatorController is not assigned to SimulationManager!", this);
        }
    }
    
    public WindSystem GetWindSystem() => _windSystem;
    public PhysicsModel GetPhysicsModel() => _physicsModel;
    public GraphPointsWrapper GetGraphPointsWrapper() => _graphPointsWrapper;
}