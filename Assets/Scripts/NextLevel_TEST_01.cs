
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel_TEST_01 : MonoBehaviour
{
    public string nextLevelSceneName;
    public GameObject timePanel;   // Reference to the UI panel that will show the time
    public TextMeshProUGUI timeDisplayText;   // Reference to the Text component that displays the time

    private DateTime startTime;  // To store the time when the level starts

    private void Start()
    {
        if (timePanel == null)
        {
            Debug.LogError("Time Panel is not assigned in the Inspector!");
        }

        if (timeDisplayText == null)
        {
            Debug.LogError("Time Display Text is not assigned in the Inspector!");
        }

        startTime = DateTime.Now;  // Capture the start time when the level begins
        timePanel.SetActive(false);  // Hide the time panel initially
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Calculate the time taken when the player touches the gate
            TimeSpan timeTaken = DateTime.Now - startTime;

            // Show the time panel and display the time
            ShowTimePanel(timeTaken);

            // Pause the game for 5 seconds and then load the next scene
            StartCoroutine(PauseAndLoadNextLevel());
        }
    }

    // Show the time panel and display the time
    private void ShowTimePanel(TimeSpan timeTaken)
    {
        timePanel.SetActive(true);  // Display the time panel
        timeDisplayText.text = "You took: " + timeTaken.TotalSeconds.ToString("F2") + " seconds";  // Show the time on the panel
    }

    // Pause the game for 5 seconds, then load the next scene
    private IEnumerator PauseAndLoadNextLevel()
    {
        // Wait for 5 seconds
        yield return new WaitForSeconds(5);

        // Load the next scene
        SceneManager.LoadScene(nextLevelSceneName);
    }
}