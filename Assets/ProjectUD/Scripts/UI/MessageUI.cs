using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI message;
    [SerializeField] private Image messageBG;
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

    private void OnEnable()
    {
        ResetState();
    }

    private void OnDisable()
    {
        ResetState();
    }

    private void ResetState()
    {
        showTimer = 0f;
        messages.Clear();

        if (message != null)
            message.text = string.Empty;

        if (messagePanel != null)
            messagePanel.SetActive(false);

        if(messageBG != null)
            messageBG.color = new Color(1f, 1f, 1f, 0f); // 배경 이미지 투명하게 초기화

        if (animator != null)
        {
            animator.Rebind();  // 애니메이터를 기본 상태로 되돌림 (트리거 값도 초기화됨)
            animator.Update(0f);
        }
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
        SoundManager.Instance.PlayUISFX(sfx);
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
