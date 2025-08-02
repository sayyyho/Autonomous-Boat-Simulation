using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StadiumSceneManager : MonoBehaviour
{
    [SerializeField] private Dropdown stadiumDropdown;
    
    private void Start()
    {
        // 드롭다운 이벤트 등록
        stadiumDropdown.onValueChanged.AddListener(OnStadiumChanged);
        
        // 초기 씬 로드 (LandStadium)
        if (!IsSceneLoaded("Stadium1"))
        {
            SceneManager.LoadScene("Stadium1", LoadSceneMode.Additive);
        }
    }
    
    private void OnStadiumChanged(int index)
    {
        string[] stadiumScenes = {
            "Stadium1",
            "Stadium2"
        };
        
        // 현재 로드된 경기장 씬 찾기
        string currentStadium = GetCurrentStadiumScene();
        
        // 새 씬 로드 전에 현재 씬 언로드
        if (!string.IsNullOrEmpty(currentStadium))
        {
            SceneManager.UnloadSceneAsync(currentStadium);
        }
        
        // 선택된 씬 로드
        SceneManager.LoadScene(stadiumScenes[index], LoadSceneMode.Additive);
    }
    
    private string GetCurrentStadiumScene()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            string sceneName = SceneManager.GetSceneAt(i).name;
            if (sceneName != "_PersistentScene")
            {
                return sceneName;
            }
        }
        return "";
    }
    
    private bool IsSceneLoaded(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name == sceneName)
            {
                return true;
            }
        }
        return false;
    }
}