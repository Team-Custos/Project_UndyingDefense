using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Settings;
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
    //ayo_0117
    [SerializeField] private CommandSkillTargetingController cSkillTargetingCtrl;

    [Header("■ UI")]
    [SerializeField] private TextMeshProUGUI waveTextUI;
    [SerializeField] private TextMeshProUGUI goldTextUI;
    [SerializeField] private GameObject settingUI;
    [SerializeField] private TextMeshProUGUI recordTextUI;
    [SerializeField] private GameObject newReorcUI;
    [SerializeField] private Image portraitImg;

    [Header("■ HP Bar")]    // 성 HP UI
    [SerializeField] private Image hpBarUI;
    [SerializeField] private TextMeshProUGUI hpTextUI;
    [SerializeField] private Image hitUI;       // 성 피격시 나타나는 이미지
    [SerializeField] private Animator animator;

    [Header("■ 배속 기능")]
    [SerializeField] private Button speedBtn;
    [SerializeField] private Animator speedBtnAnim;

    [Header("■ Notice")]
    [SerializeField] private NoticeUI noticeUI;
    [SerializeField] private NoticeUI noticeTimerUI;

    [SerializeField] private GameObject errorPanel;
    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private MessageUI messageUI;

    [SerializeField] private Image regionNameUI;

    [Header("■ Result")]
    [SerializeField] private ResultUI resultUI;

    [SerializeField] private TextMeshProUGUI[] spawnBtnPriceText;
    [SerializeField] private Image[] spawnBtnsImages;
    [SerializeField] private int[] spawnCosts;

    [SerializeField] private GameObject fortressPanel;
    [SerializeField] private GameObject goldPanel;
    [SerializeField] private GameObject wavePanel;
    [SerializeField] private GameObject settingBtn;
    [SerializeField] private GameObject commandSkillPanel;
    [SerializeField] private GameObject spawnPanel;

    private void Start()
    {
        LoadPortraitInGame();
    }

    private void LoadPortraitInGame()
    {
        int savedID = PlayerPrefs.GetInt("SelectedPortraitID");

        // Resources에서 전체 로드 후 ID로 찾기
        PortraitData[] allData = Resources.LoadAll<PortraitData>("Data/PortraitData");
        PortraitData portrait = System.Array.Find(allData, p => p.portraitID == savedID);
        if (portrait == null)
        {
            // ID가 없는 경우 기본값으로 설정 (예: 첫 번째 데이터)
            portrait = allData.Length > 0 ? allData[0] : null;
        }

        else    //(portrait != null)
        { 
            portraitImg.sprite = portrait.portrait;
        }
    }

    public void SetWaveNumber(int waveNum, int maxWave, bool infinite)
    {
        if(infinite)
            waveTextUI.text = $" {waveNum} / ∞ 웨이브";
        else
            waveTextUI.text = $" {waveNum} / {maxWave} 웨이브";
    }

    public void UpdateGoldTextUI(float gold)
    {
        goldTextUI.text = ((int)gold).ToString();
    }

    public void SetHP(float hp, float maxHp) // 성 HP
    {
        hpBarUI.fillAmount = hp / maxHp;
        hpTextUI.text = $"{(int)hp} / {(int)maxHp}";
    }

    public void ShowFortressHitUI()
    {
        animator.SetTrigger("Hit");
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


    public void ShowPerWaveNotice()
    {
        //--Localize
        noticeUI.SetText(LocalizationSettings.StringDatabase.
            GetLocalizedString("IngameUI", "NTF_battleAttacked", LocalizationSettings.SelectedLocale), true);
        //noticeUI.SetText("성이 현재 공격받고있습니다!", true);
    }

    public void SetNoticeText(string text)
    {
        noticeTimerUI.SetText(text, false);
    }

    public void ShowResult(float reward, bool win, string record)
    {
        resultUI.Show(reward, win, record);
    }

    public void HideResult() => resultUI.Hide();

    public void OnOffSetting()
    {
        if (settingUI.activeSelf)
        {
            settingUI.SetActive(false);
            SoundManager.Instance.PlayCancelUISFX();
            Time.timeScale = 1.0f;
        }
        else
        {
            SoundManager.Instance.PlayUIClickSFX();
            settingUI.SetActive(true);

            allyUnitSpawner.CancelSpawn();
            ingameCommandSkillManager.CancelSkill();
            //ayo_0117
            //cSkillTargetingCtrl.CancelTargeting();
            upgradeMenuUI.HideUpgradeUI();
            selectedUnitManager.DeSelecteUnit();


            Time.timeScale = 0.0f;
        }

        inputEventManager.OnESCTarget = inGameManager;
    }

    public void OnOffSettingUI(bool isGamePause)
    {
        if (isGamePause)
        {
            //SoundManager.Instance.playCancleSFX();
            settingUI.SetActive(true);
            SoundManager.Instance.PlayCancelUISFX();
            //Time.timeScale = 1.0f;
        }
        else
        {
            SoundManager.Instance.PlayUIClickSFX();
            settingUI.SetActive(false);
        }
    }

    public void CloseSettting() // 인게임 설정창 버튼 용
    {
        if (settingUI.activeSelf)
        {
            SoundManager.Instance.PlayCancelUISFX();
            settingUI.SetActive(false);
            //Time.timeScale = 1.0f;
        }

    }

    // -- Localization 수정
    public void ShowError(string table, string id)
    {
        messageUI.AddMessage(LocalizationSettings.StringDatabase.GetLocalizedString(table, id,
                 LocalizationSettings.SelectedLocale));
        errorPanel.SetActive(true);
        //errorText.text = id;
        //errorText.text = LocalizationSettings.StringDatabase.GetLocalizedString(table, id,
        //         LocalizationSettings.SelectedLocale);
    }

    public void SetspawnBtnPriceTextColor()
    {
        for (int i = 0; i < spawnBtnPriceText.Length; i++)
        {
            if (spawnCosts[i] > inGameManager.inGameGold)
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

    public void OnOffInGameUI(bool on)
    {
        fortressPanel.SetActive(on);
        goldPanel.SetActive(on);
        wavePanel.SetActive(on);
        settingBtn.SetActive(on);
        commandSkillPanel.SetActive(on);
        spawnPanel.SetActive(on);
        recordTextUI.gameObject.SetActive(on);
        speedBtn.gameObject.SetActive(on);
    }

    public void SetRecordTextUI(string text)
    {
        recordTextUI.text = text;
    }

    public void ShowNewRecordUI(bool newRecord)
    {
        if(newRecord)
        {
            newReorcUI.SetActive(true);
        }
        else
        {
            newReorcUI.SetActive(false);
        }
    }

    public void UpdateSpeedButtonAni(bool isFastForward)
    {
        speedBtnAnim.SetBool("VideoSpeedDouble", isFastForward);
    }
}