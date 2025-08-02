using TMPro;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    public TMP_Text[] timerTexts;  // 여러 타이머 UI 지원
    private float timer = 0f;
    private bool isRunning = false;

    void Update()
    {
        if (isRunning)
        {
            timer += Time.deltaTime;
            foreach (TMP_Text text in timerTexts)
            {
                if (text != null)
                    text.text = timer.ToString("F2") + " s";
            }
        }
    }

    public void StartTimer()
    {
        timer = 0f;
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public float GetElapsedTime()
    {
        return timer;
    }
}
