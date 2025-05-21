using InputEventInterface;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

//이 스크립트는 인게임 내에서 이루어진 전반적인 것을 관리하기 위한 스크립트입니다.


//public static InGameManager inst;
//public GridManager gridManager;//그리드 관리
//public InGame_VirtualCamManager camManager;//카메라 관리

//public int gold = 0;//현재 가지고 있는 골드의 양.

//public CommanderSkillData[] CurCommanderSkill;//현재 지휘간 스킬.

//public GameObject Base;//성 오브젝트.
//public NavMeshSurface NavMeshSurface;

////디버그용으로 사용할 유닛 설치가능 상태.
//public bool UnitSetMode = false;
//public bool AllyUnitSetMode = false;
//public bool EnemyUnitSetMode = false;

//public bool FasterTimeScale = false;
//public bool isGamePause;

//private void Awake()
//{
//    inst = this;
//}

//// Update is called once per frame
//void Update()
//{
//    if (Input.GetKeyDown(KeyCode.R))
//    {
//        UnitSetMode = !UnitSetMode;
//        EnemyUnitSetMode = !EnemyUnitSetMode;
//    }

//    if (FasterTimeScale)
//    {
//        Time.timeScale = 3f;
//    }
//    else
//    {
//        if (isGamePause)
//        {
//            Time.timeScale = 0.0f;
//        }
//        else
//        {
//            Time.timeScale = 1f;
//        }
//    }




//    if (UnitSetMode && AllyUnitSetMode)
//    {
//        // 타일 색상 업데이트
//        GridTile[] allTiles = FindObjectsOfType<GridTile>();
//        foreach (var tile in allTiles)
//        {
//            tile.ShowPlacementColors(true);
//        }
//    }
//    else
//    {
//        // 타일 색상 업데이트
//        GridTile[] allTiles = FindObjectsOfType<GridTile>();
//        foreach (var tile in allTiles)
//        {
//            tile.ShowPlacementColors(false);
//        }
//    }


//}

//public void UpdateNavMeshSurface()
//{
//    NavMeshSurface.UpdateNavMesh(NavMeshSurface.navMeshData);
//}


//public void AllUnitSelectOff()//모든 유닛들의 선택 해제.
//{
//    Ingame_UnitCtrl[] allUnit = FindObjectsOfType<Ingame_UnitCtrl>();
//    foreach (var unit in allUnit)
//    {
//        unit.isSelected = false;
//    }
//}

//public void AllTileSelectOff()//모든 타일의 선택 해제.
//{
//    GridTile[] allTiles = FindObjectsOfType<GridTile>();
//    foreach (var tile in allTiles)
//    {
//        tile.Selected = false;
//    }
//}


//// 저장된 커맨더 스킬 불러오기
//public Dictionary<string, string> LoadCommandSkillList()
//{
//    Dictionary<string, string> loadedSkills = new Dictionary<string, string>();

//    string skillIDList = PlayerPrefs.GetString("SkillIDList", "");

//    if (!string.IsNullOrEmpty(skillIDList))
//    {
//        string[] skillIDs = skillIDList.Split(',');

//        foreach (string skillID in skillIDs)
//        {
//            string skillName = PlayerPrefs.GetString(skillID, "이름 없음");
//            loadedSkills.Add(skillID, skillName);
//        }
//    }

//    if (loadedSkills.Count == 0)
//    {
//        Debug.LogWarning("불러온 스킬이 없습니다.");
//    }

//    return loadedSkills;
//}

public class InGameManager : MonoBehaviour, IInputESC
{
    public float inGameGold { set; get; }
    [SerializeField] private IngameScreenUI ingameScreenUI;
    [SerializeField] private PlayerInputEventManager inputEventManager;

    [SerializeField] private AudioClip inGameIntro;

    // public float ingameGold => InGameGold;

    private bool isGamePause = false;

    protected static AudioClip coinDropSFX;
    protected static AudioClip CoinDropSFX
    {
        get
        {
            if (coinDropSFX == null)
            {
                coinDropSFX = Resources.Load<AudioClip>("Sound/SFX/효과음/캐릭터/DeathSFX/sfx_coinDrop");
            }
            return coinDropSFX;
        }
    }


    private void Start()
    {
        inGameGold = 500;
        ingameScreenUI.SetGoldTextUI(inGameGold);

        SoundManager.Instance.PlaySFX(inGameIntro);

        inputEventManager.OnESCTarget = this;
    }

    public void SetGold(float gold, bool plus)
    {
        if (plus)
        {
            inGameGold += gold;
            SoundManager.Instance.PlaySFX(CoinDropSFX);
        }
        else
        {
            {
                inGameGold -= gold;
            }
        }

        ingameScreenUI.SetGoldTextUI(inGameGold);
    }

    public void ReLoadeCurrentScene()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadLobbyScene()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("LobbyScene_LoPol");
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }

    public void PauseGame()
    {
        if (!isGamePause)
        {
            Time.timeScale = 0f;
            isGamePause = true;
        }
        else
        {
            Time.timeScale = 1f;
            isGamePause = false;
        }
    }

    public void OnESC(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ingameScreenUI.OnOffSetting();
        }
    }


}