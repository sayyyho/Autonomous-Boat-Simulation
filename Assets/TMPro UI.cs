using UnityEngine;
using TMPro;

public class CollisionCountUI : MonoBehaviour
{
    public PathFollower pathFollower;       // Inspector���� PathFollower �巡��
    public TMP_Text countText;              // Inspector���� UI �ؽ�Ʈ �巡��

    void Update()
    {
        if (pathFollower != null && countText != null)
        {
            countText.text = $"충돌 횟수 :{pathFollower.CollisionCount}";
        }
    }
}
