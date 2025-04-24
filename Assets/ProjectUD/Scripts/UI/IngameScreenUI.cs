using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Specialized;

public class IngameScreenUI : MonoBehaviour
{
    [Header("■ UI")]
    [SerializeField] private TextMeshProUGUI waveTextUI;
    [SerializeField] private TextMeshProUGUI goldTextUI;

    [Header("■ HP Bar")]
    [SerializeField] private Image hpBarUI;
    [SerializeField] private TextMeshProUGUI hpTextUI;

    [Header("■ Notice")]
    [SerializeField] private NoticeUI noticeUI;
    [SerializeField] private NoticeUI noticeTimerUI;

    [Header("■ Result")]
    [SerializeField] private ResultUI resultUI;

    [SerializeField] private Animator animator;

    public void SetWaveNumber(int waveNum)
    {
        waveTextUI.text = $"웨이브 {waveNum}";
    }

    public void SetGoldTextUI(float gold)
    {
        goldTextUI.text = ((int)gold).ToString();
    }

    public void SetHP(float hp, float maxHp) // 성 HP
    {
        hpBarUI.fillAmount = hp / maxHp;
        hpTextUI.text = $"{(int)hp} / {(int)maxHp}";
    }

    public void ShowNotice(string text) //
    {
        noticeUI.SetText(text ,false);
        Fade();
    }

    public void ShowTimer()
    {
        TimerFade();
        //noticeTimerUI.gameObject.SetActive(true);
    }

    public void HideNotice()
    {
        noticeTimerUI.Hide();
    }


    public void ShowPreWaveNotice()
    {
        Fade();
        noticeUI.SetText("성이 현재 공겨받고있습니다!", true);
    }

    public void SetNoticeText(string text)
    {
        noticeTimerUI.SetText(text, false);
    }

    public void ShowResult(float reward, bool win)
    {
        resultUI.Show(reward, win);
    }

    public void HideResult() => resultUI.Hide();

    public void Fade()
    {
        animator.SetTrigger("FadeTrigger");
    }

    public void TimerFade()
    {
        animator.SetTrigger("TimerFadeTrigger");
    }

    public void TimerEnd()
    {
        animator.SetTrigger("TimerEndTrigger");
    }
}