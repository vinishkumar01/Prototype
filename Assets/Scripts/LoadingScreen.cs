using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(LoadNextScene());
    }

    private IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(1f); // wait 2�3 seconds for transition

        string nextScene = PlayerPrefs.GetString("NextScene", "");
        SceneManager.LoadScene(nextScene);
    }
}
