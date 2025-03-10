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

    public void SetGold(float gold)
    {
        goldTextUI.text = ((int)gold).ToString();
    }

    public void SetHP(float hp, float maxHp)
    {
        hpBarUI.fillAmount = hp / maxHp;
        hpTextUI.text = $"{(int)hp} / {(int)maxHp}";
    }

    public void ShowNotice(string text, bool isWarning = false)
    {
        noticeUI.Show(text, isWarning, true);
    }

    public void HideNotice() => noticeUI.Hide();

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
