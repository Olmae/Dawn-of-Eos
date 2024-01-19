using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    private float startTime;
    private bool isTimerRunning;
    public Text timerText;

    void Start()
    {
        startTime = Time.time;
        isTimerRunning = true;
            }

    void Update()
    {
        if (isTimerRunning)
        {
            float timeElapsed = Time.time - startTime;

            int hours = (int)(timeElapsed / 3600);
            int minutes = (int)((timeElapsed - hours * 3600) / 60);
            int seconds = (int)(timeElapsed - hours * 3600 - minutes * 60);

            timerText.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        }
    }
}
