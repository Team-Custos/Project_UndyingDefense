using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class DollyCamera : MonoBehaviour
{
    [SerializeField] private CinemachineDollyCart dollyCart;
    [SerializeField] private IngameScreenUI ingameScreenUI;
    [SerializeField] private InGameManager inGameManager;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private EnemyUnitSpawner enemyUnitSpawner;

    
    [SerializeField] private GameObject virtualCamera;
    [SerializeField] private bool isCamPanning = true;
    public bool IsCamPanning => isCamPanning;
    [SerializeField] private float panningDuration = 9.0f;
    private int eventIndex = 0;

    void Start()
    {
        dollyCart.m_Position = 0f;
    }

    void Update()
    {
        if (isCamPanning)
        {
            panningDuration -= Time.deltaTime;
            if (panningDuration <= 7.0f && eventIndex ==0)
            {
                ingameScreenUI.ShowRegionName();
                eventIndex++;
            }
            else if(panningDuration <= 3.0f && eventIndex == 1)
            {
                //--Localize
                //ingameScreenUI.ShowNotice("전투 시작");
                ingameScreenUI.ShowNotice(LocalizationSettings.StringDatabase.
                    GetLocalizedString("IngameUI", "NTF_battleStart", LocalizationSettings.SelectedLocale));
                eventIndex++;
            }
            else if(panningDuration <= 0.0f && eventIndex == 2)
            {
                eventIndex++;
            }

            if(eventIndex == 3)
            {
                inGameManager.PlayInGameBGM();
                ingameScreenUI.OnOffInGameUI(true);
                inGameManager.StartGame();
                isCamPanning = false;
                waveManager.StartWave();
            }    

            if (dollyCart.m_Position >= dollyCart.m_Path.MaxPos && panningDuration < 3.0f)
            {
                virtualCamera.SetActive(true);
                
            }

        }

    }
}
