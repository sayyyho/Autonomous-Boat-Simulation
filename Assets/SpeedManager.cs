using UnityEngine;
using TMPro;

public class SpeedManager : MonoBehaviour
{
    public NewBoatController boatController;
    public TMP_Text speedText;

    void Update()
    {
        if (boatController != null && speedText != null)
        {
            float speed = boatController.CurrentSpeed;
            speedText.text = $"속도: {speed:F2} m/s";

            // 콘솔에 속도 출력
            Debug.Log($"[SpeedManager] 현재 속도: {speed:F2} m/s");
        }
    }
}