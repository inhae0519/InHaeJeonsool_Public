using System.Collections;
using TMPro;
using UnityEngine;

public class Dialogue : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private string[] lines;
    [SerializeField] private float textSpeed;

    private int index;

    private void Start() {
        text.text = string .Empty;
        StartDialogue();
    }
    private void Update() {
        if (Input.GetMouseButton(0)) {
            if (text.text == lines[index]) {
                NextLine();
            }
            else {
                StopAllCoroutines();
                text.text = lines[index];
            }
        }
    }
    private void StartDialogue() {
        index = 0;
        StartCoroutine(TypeLine());
    }
    IEnumerator TypeLine() {
        foreach(char c in lines[index].ToCharArray()) {
            text.text += c;

            yield return new WaitForSeconds(textSpeed);
        }
    }

    private void NextLine() {
        if(index < lines.Length - 1) {
            index++;
            text.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else {
            gameObject.SetActive(false);
        }
    }
}
