using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    [Header("■ Result")]
    [SerializeField] private ResultUI resultUI;

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

    public void ShowNotice(string text, bool isWarning, bool timerOn) //
    {
        noticeUI.Show(text, isWarning, timerOn);
    }

    public void HideNotice()
    {
        noticeUI.Hide(true);
    }


    public void ShowPreWaveNotice()
    {
        noticeUI.Show(string.Empty, false, true);
    }

    public void SetNoticeText(string text)
    {
        noticeUI.SetText(text);
    }

    public void ShowResult(float reward, bool win)
    {
        resultUI.Show(reward, win);
    }

    public void HideResult() => resultUI.Hide();
}
