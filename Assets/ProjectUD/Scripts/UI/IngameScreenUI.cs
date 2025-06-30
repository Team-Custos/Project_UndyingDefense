using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Specialized;
using InputEventInterface;

public class IngameScreenUI : MonoBehaviour//, IInputESC
{
    [SerializeField] private InGameManager inGameManager;
    [SerializeField] private PlayerInputEventManager inputEventManager;
    [SerializeField] private AllyUnitSpawner allyUnitSpawner;
    [SerializeField] private IngameCommandSkillManager ingameCommandSkillManager;
    [SerializeField] private SelectedUnitManager selectedUnitManager;
    [SerializeField] private UpgradeMenuUI upgradeMenuUI;

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



    public void SetWaveNumber(int waveNum, int maxWave)
    {
        waveTextUI.text = $" {waveNum} / {maxWave} 웨이브";
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
        SoundManager.Instance.PlayUIClickSFX();
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
            //SoundManager.Instance.playCancleSFX();
            settingUI.SetActive(false);
            Time.timeScale = 1.0f;
        }
        else
        {
            SoundManager.Instance.PlayUIClickSFX();
            settingUI.SetActive(true);

            allyUnitSpawner.CancelSpawn();
            ingameCommandSkillManager.CancelSkill();
            upgradeMenuUI.HideUpgradeUI();
            selectedUnitManager.DeSelecteUnit();


            Time.timeScale = 0.0f;
        }

        inputEventManager.OnESCTarget = inGameManager;
    }

    public void OnOffSettingUI()
    {
        if (settingUI.activeSelf)
        {
            //SoundManager.Instance.playCancleSFX();
            settingUI.SetActive(false);
            Time.timeScale = 1.0f;
        }
        else
        {
            SoundManager.Instance.PlayUIClickSFX();
            settingUI.SetActive(true);


            Time.timeScale = 0.0f;
        }
    }

    public void CloseSettting()
    {
        if (settingUI.activeSelf)
        {
            //.Instance.playCancleSFX();
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