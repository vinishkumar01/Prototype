using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scene_Manager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown sceneDropdown;

    // Scenes to exclude
    private HashSet<string> excludedScenes = new HashSet<string> { "Loading Screen", "Learning"};

    private void Start()
    {
        sceneDropdown.ClearOptions();
        var options = new List<string>();

        // Get all scenes from build settings
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);

            if(!excludedScenes.Contains(name))
            {
                options.Add(name);
            }
            
        }

        sceneDropdown.AddOptions(options);
    }

    public string GetSelectedScene()
    {
        return sceneDropdown.options[sceneDropdown.value].text;
    }

    public void OnPlayButtonPressed()
    {
        string selectedScene = sceneDropdown.options[sceneDropdown.value].text;

        Debug.Log("Selected scene: " + selectedScene); // Debug check

        // Save the selected scene name (so the transition scene knows what to load next)
        PlayerPrefs.SetString("NextScene", selectedScene);
        PlayerPrefs.Save();

        // Load transition (black screen) scene
        SceneManager.LoadScene("Loading Screen");
    }

}
