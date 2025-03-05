using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisableVideoAfterPlay : MonoBehaviour
{
    public Canvas VideoPlayer;
    public float VideoLength = 10;


    private void Awake()
    {
        gameObject.SetActive(true);
    }
    private void Start()
    {

        StartCoroutine(CountdownTimer());
    }

    private IEnumerator CountdownTimer()
    {
        while (true) {
            yield return new WaitForSeconds(VideoLength);
            gameObject.SetActive(false);
        }
    }
}
