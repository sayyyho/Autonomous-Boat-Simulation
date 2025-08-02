using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ObstacleUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider countSlider;
    [SerializeField] private Text countValueText; // Legacy Text 사용
    [SerializeField] private Button generateButton;
    [SerializeField] private GameObject controlPanel;
    [SerializeField] private Dropdown sceneDropdown; // 씬 선택 드롭다운 참조

    [Header("Settings")]
    [SerializeField] private int minObstacles = 1;
    [SerializeField] private int maxObstacles = 50;
    [SerializeField] private int defaultObstacles = 10;

    private int currentObstacleCount;
    private ObstacleManager currentObstacleManager;

    private void Start()
    {
        // UI 초기화
        countSlider.minValue = minObstacles;
        countSlider.maxValue = maxObstacles;
        countSlider.value = defaultObstacles;
        currentObstacleCount = defaultObstacles;
        UpdateCountText();

        // 이벤트 리스너 등록
        countSlider.onValueChanged.AddListener(OnSliderChanged);
        generateButton.onClick.AddListener(OnGenerateClicked);
        controlPanel.SetActive(false); // 초기에는 패널 비활성화

        // 씬 변경 감지
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void OnSceneChanged(Scene previous, Scene current)
    {
        FindObstacleManager();
    }

    private void FindObstacleManager()
    {
        currentObstacleManager = FindObjectOfType<ObstacleManager>();
    }

    private void OnSliderChanged(float value)
    {
        currentObstacleCount = Mathf.RoundToInt(value);
        UpdateCountText();
    }

    private void UpdateCountText()
    {
        countValueText.text = currentObstacleCount.ToString();
    }

    private void OnGenerateClicked()
    {
        if (currentObstacleManager != null)
        {
            currentObstacleManager.GenerateObstacles(currentObstacleCount);
        }
    }

    // UI 패널 토글 메서드 추가
    public void ToggleControlPanel()
    {
        bool isActive = !controlPanel.activeSelf;
        controlPanel.SetActive(isActive);
        
        // 패널 활성화 상태에 따라 씬 드롭다운 상호작용 제어
        if(sceneDropdown != null)
        {
            sceneDropdown.interactable = !isActive;
        }
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }
}