using System.Collections;
using UnityEngine;
using TMPro;

public class CountDown : MonoBehaviour
{
    public float delay = 3;
    TMP_Text text;
    private void Start()
    {
        text = GetComponent<TMP_Text>();
        text.text = delay.ToString();
        StartCoroutine(StartCountDown());
    }

    IEnumerator StartCountDown()
    {
        while (delay >= 0)
        {
            yield return null;
            delay -= Time.deltaTime;
            text.text = string.Format("{0:.##}", delay);
        }
    }
}
