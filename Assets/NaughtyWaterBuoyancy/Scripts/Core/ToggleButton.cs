// ObstacleToggleButton에 추가할 스크립트
using UnityEngine;
using UnityEngine.UI;

public class TogglePanelButton : MonoBehaviour
{
    [SerializeField] private ObstacleUI obstacleUI;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(TogglePanel);
    }

    private void TogglePanel()
    {
        obstacleUI.ToggleControlPanel();
    }
}