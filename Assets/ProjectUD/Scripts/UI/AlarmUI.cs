using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlarmUI : MonoBehaviour
{
    [SerializeField] Image alarmImage;
    [SerializeField] private Sprite[] alarmSprite;

    private int currentIndex = 0;
    private float timer = 0f;
    private float switchInterval = 0.5f;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= switchInterval)
        {
            timer = 0f;

            currentIndex = (currentIndex == 0) ? 1 : 0;
            alarmImage.sprite = alarmSprite[currentIndex];
        }
    }
}
