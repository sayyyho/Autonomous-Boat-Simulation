using UnityEngine;

public class PiPCamera : MonoBehaviour
{
    public Camera mainCamera;
    public Camera boatCamera;
    public Vector2 pipPosition = new Vector2(0.7f, 0.7f);
    public Vector2 pipSize = new Vector2(0.3f, 0.3f);

    void Start()
    {
        // 메인 카메라: 전체 화면
        mainCamera.rect = new Rect(0, 0, 1, 1);
        
        // 보트 카메라: 작은 PIP 화면
        boatCamera.rect = new Rect(pipPosition.x, pipPosition.y, pipSize.x, pipSize.y);
        
        // 두 카메라 모두 활성화
        mainCamera.enabled = true;
        boatCamera.enabled = true;
        
        // PIP 카메라에 깊이 설정 (메인 카메라보다 높게)
        boatCamera.depth = 1;
    }
}