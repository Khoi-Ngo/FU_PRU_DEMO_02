using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel_TEST_01 : MonoBehaviour
{
    public string nextLevelSceneName;

    public void LoadNewScene()
    {
        SceneManager.LoadScene(nextLevelSceneName);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            LoadNewScene();
        }
    }
}
