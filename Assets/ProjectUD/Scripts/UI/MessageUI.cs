using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI message;
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private AudioClip sfx;
    [SerializeField] private float duration;
    private float showTimer;
    [SerializeField] private Animator animator;
    public List<string> messages = new List<string>();


    private void Start()
    {
        //animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if(showTimer > 0f)
        {
            showTimer -= Time.deltaTime;

            if (showTimer <= 0f)
            {
                animator.SetTrigger("FadeOut"); // 끄는 애니메이션 호출 Fadeout
                RemoveMessage();
            }
        }
    }

    public void AddMessage(string message)  // 버튼 클릭 메서드용
    {
        if (!messages.Contains(message))
        {
            messages.Add(message);

            if (messages.Count == 1)
            {
                ShowNextMessage();
            }
        }
    }
    private void ShowNextMessage()
    {
        showTimer = duration;
        string str = messages[0];
        message.text = str;
        messagePanel.SetActive(true);
        message.gameObject.SetActive(true);
        animator.SetTrigger("FadeIn");
        SoundManager.Instance.PlaySFX(sfx);
    }

    private void RemoveMessage()
    {
        messages.RemoveAt(0);
        messagePanel.SetActive(false);
        if (messages.Count > 0)
        {
            ShowNextMessage();
        }
    }
}
