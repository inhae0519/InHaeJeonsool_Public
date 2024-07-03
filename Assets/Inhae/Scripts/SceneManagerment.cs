using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerment : MonoBehaviour
{
    public static SceneManagerment Instance;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public void SceneChange(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
    }
    
    public void SceneChange(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
