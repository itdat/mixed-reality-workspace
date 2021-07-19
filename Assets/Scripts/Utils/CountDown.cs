using TMPro;
using UnityEngine;

public class CountDown : MonoBehaviour {
    public float timeRemaining;
    public bool timerIsRunning;
    public TextMeshPro timeText;
    private Transform mainCam;

    private void OnEnable() {
        mainCam = Camera.main.transform;
    }

    private void Update() {
        if (!timerIsRunning) return;
        if (mainCam) {
            var rotation = mainCam.rotation;
            var position = mainCam.position + rotation * Vector3.forward * 3;
            transform.position = position;
            transform.rotation = new Quaternion(0.0f, rotation.y, 0.0f, rotation.w);
        }

        if (timeRemaining > 0) {
            timeRemaining -= Time.deltaTime;
            DisplayTime(timeRemaining);
        }
        else {
            Debug.Log("Time has run out!");
            timeRemaining = 0;
            timerIsRunning = false;
        }
    }

    public void StartCountDown(int time) {
        timeRemaining = time;
        timerIsRunning = true;
    }

    void DisplayTime(float timeToDisplay) {
        if (timeText == null) return;

        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timeText.text = minutes > 0 ? $"{minutes:00}:{seconds:00}" : $"{seconds}";
    }
}
