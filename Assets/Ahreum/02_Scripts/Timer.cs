using System.Collections;
using UnityEngine;

public class Timer : MonoBehaviour {
    [SerializeField] private int runningTime;

    [SerializeField] public static bool TimeisDone = false;
    public bool time = false;

    private void Awake()
    {
        TimeisDone = false;
    }

    private void Start() {
        StartCoroutine(RoutinePlayCountDown());
    }

    IEnumerator RoutinePlayCountDown() {
        while (runningTime > 0) {
            Debug.Log(runningTime);
            runningTime -= 1;

            yield return new WaitForSeconds(1f);
        }

        TimeisDone = true;
        time = true;
    }
}
