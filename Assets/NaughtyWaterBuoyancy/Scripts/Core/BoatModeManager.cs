using UnityEngine;

public class BoatModeManager : MonoBehaviour
{
    public MonoBehaviour manualControl;     // BoatController.cs
    public MonoBehaviour autonomousControl; // SimpleAutonomousBoat.cs

    public void SetManualMode()
    {
        manualControl.enabled = true;
        autonomousControl.enabled = false;
        Debug.Log("수동 모드 활성화");
    }

    public void SetAutoMode()
    {
        manualControl.enabled = false;
        autonomousControl.enabled = true;
        Debug.Log("자율 운항 모드 활성화");
    }

    void Start()
    {
        SetManualMode(); // 기본값 자율운항
    }
}
