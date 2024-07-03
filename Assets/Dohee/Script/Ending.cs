using Dohee;
using UnityEngine;
using UnityEngine.SceneManagement;
using YSCore;

public class Ending : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out FishSingleton singleton))
        {
            if (Timer.TimeisDone)
            {
                if (singleton.GetComponent<FishScale>().Scale > 5.2f)
                {
                    StageManager.Instance.SetPoint(2);
                    SoundManager.Instance.Clear();
                    SceneManager.LoadScene("C_TimeEnding");
                }
                else
                {
                    StageManager.Instance.SetPoint(1);
                    SoundManager.Instance.Clear();
                    SceneManager.LoadScene("C_TimeSizeEnding");
                }
            }
            else
            {
                if (singleton.GetComponent<FishScale>().Scale > 5.2f)
                {
                    StageManager.Instance.SetPoint(3);
                    SoundManager.Instance.Clear();
                    SceneManager.LoadScene("C_TrueEnding");
                }
                else
                {
                    StageManager.Instance.SetPoint(2);
                    SoundManager.Instance.Clear();
                    SceneManager.LoadScene("C_SizeEnding");
                }
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SoundManager.Instance.Clear();
            SceneManager.LoadScene(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SoundManager.Instance.Clear();
            SceneManager.LoadScene(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SoundManager.Instance.Clear();
            SceneManager.LoadScene(4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SoundManager.Instance.Clear();
            SceneManager.LoadScene(5);
        }
    }
}
