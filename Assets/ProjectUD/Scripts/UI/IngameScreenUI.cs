using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Specialized;
using InputEventInterface;

public class IngameScreenUI : MonoBehaviour
{
    [SerializeField] private InGameManager inGameManager;

    [Header("■ UI")]
    [SerializeField] private TextMeshProUGUI waveTextUI;
    [SerializeField] private TextMeshProUGUI goldTextUI;
    [SerializeField] private GameObject settingUI;

    [Header("■ HP Bar")]
    [SerializeField] private Image hpBarUI;
    [SerializeField] private TextMeshProUGUI hpTextUI;

    [Header("■ Notice")]
    [SerializeField] private NoticeUI noticeUI;
    [SerializeField] private NoticeUI noticeTimerUI;
    [SerializeField] private GameObject errorPanel;
    [SerializeField] private Text errorText;
    [SerializeField] private Image regionNameUI;

    [Header("■ Result")]
    [SerializeField] private ResultUI resultUI;

    [SerializeField] private Animator animator;

    [SerializeField] private TextMeshProUGUI[] spawnBtnPriceText;
    [SerializeField] private Image[] spawnBtnsImages;
    [SerializeField] private int spawnCost;



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
        noticeUI.SetText(text, false);
    }

    public void ShowTimer()
    {
        noticeTimerUI.gameObject.SetActive(true);
    }

    public void HideTimer()
    {
        noticeTimerUI.gameObject.SetActive(false);
    }

    public void HideNotice()
    {
        noticeTimerUI.Hide();
    }


    public void ShowPreWaveNotice()
    {
        noticeUI.SetText("성이 현재 공격받고있습니다!", true);
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

    public void OnOffSetting()
    {
        if (settingUI.activeSelf)
        {
            settingUI.SetActive(false);
            Time.timeScale = 1.0f;
        }
        else
        {
            settingUI.SetActive(true);
            Time.timeScale = 0.0f;
        }
    }

    public void CloseSettting()
    {
        if (settingUI.activeSelf)
        {
            settingUI.SetActive(false);
            Time.timeScale = 1.0f;
        }

    }

    public void ShowError(string text)
    {
        errorPanel.SetActive(true);
        errorText.text = text;
    }

    public void SetspawnBtnPriceTextColor()
    {
        for (int i = 0; i < spawnBtnPriceText.Length; i++)
        {
            if (spawnCost > inGameManager.inGameGold)
            {
                spawnBtnPriceText[i].color = Color.red;
                spawnBtnsImages[i].tag = "UnInteractiveUi";
            }
            else
            {
                spawnBtnPriceText[i].color = Color.white;
                spawnBtnsImages[i].tag = "InteractiveUi";
            }
        }
    }

    public void ShowRegionName()
    {
        regionNameUI.gameObject.SetActive(true);
    }
}