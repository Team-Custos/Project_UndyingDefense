using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI message;
    [SerializeField] private AudioClip sfx;
    private Animator animator;
    private List<string> messages = new List<string>();

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    public void Show(string str)
    {
        message.text = str;
        message.gameObject.SetActive(true);
        animator.SetTrigger("Show");
        SoundManager.Instance.PlaySFX(sfx);

    }
}
