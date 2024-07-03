    using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour {
    public void GoToTitle() {
        SceneManager.LoadScene("Chapter");
    }

    public void GoToGame()
    {
        SceneManager.LoadScene(1);
    }
}
