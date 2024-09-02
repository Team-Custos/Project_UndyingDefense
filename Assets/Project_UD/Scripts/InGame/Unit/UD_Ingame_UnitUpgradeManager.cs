using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UD_UnitDataManager;

public class UD_Ingame_UnitUpgradeManager : MonoBehaviour
{
    [System.Serializable]
    public class UnitUpgrade
    {
        public string CurrentUnitName;
        public string UpgradeOption1; 
        public string UpgradeOption2; 

        public UnitUpgrade(string currentUnit, string option1, string option2)
        {
            CurrentUnitName = currentUnit;
            UpgradeOption1 = option1;
            UpgradeOption2 = option2;
        }
    }


    private UD_Ingame_UIManager uiManager; // UI와의 상호작용을 위해
    private Dictionary<string, UnitUpgrade> upgradeTree = new Dictionary<string, UnitUpgrade>();


    // Start is called before the first frame update
    void Start()
    {
        uiManager = UD_Ingame_UIManager.instance;

        // 민병
        upgradeTree.Add("민병", new UnitUpgrade("민병", "창병", "척후병"));  // 1티어

        upgradeTree.Add("창병", new UnitUpgrade("창병", "장창무사", "대낫무사")); // 2티어
        upgradeTree.Add("척후병", new UnitUpgrade("척후병", "도끼전사", "선봉기사"));

        upgradeTree.Add("대낫무사", new UnitUpgrade("대낫무사", "대낫사신", null)); // 3티어
        upgradeTree.Add("장창무사", new UnitUpgrade("장창무사", "장창선인", null));
        upgradeTree.Add("도끼전사", new UnitUpgrade("도끼전사", "돌격대장", null));
        upgradeTree.Add("선봉기사", new UnitUpgrade("선봉기사", "도끼대장", null));

        // 사냥꾼
        upgradeTree.Add("사냥꾼", new UnitUpgrade("사냥꾼", "한량", "포수"));

        upgradeTree.Add("한량", new UnitUpgrade("한량", "명궁", "맹독 사냥꾼"));
        upgradeTree.Add("포수", new UnitUpgrade("포수", "산포수", "화포수"));

        upgradeTree.Add("명궁", new UnitUpgrade("명궁", "신궁", null));
        upgradeTree.Add("맹독 사냥꾼", new UnitUpgrade("맹독 사냥꾼", "사냥의 달인", null));
        upgradeTree.Add("산포수", new UnitUpgrade("산포수", "착호갑사", null));
        upgradeTree.Add("화포수", new UnitUpgrade("화포수", "화약마", null));

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public UnitUpgrade GetUpgradeOptions(string unitName)
    {
        if (string.IsNullOrEmpty(unitName))
        {
            return null;
        }


        if (upgradeTree.ContainsKey(unitName))
        {
            return upgradeTree[unitName];
        }
        return null;
    }

    public void PerformUpgrade(UD_Ingame_UnitCtrl selectedUnit, string newUnitName)
    {
        UnitData newUnitData = UD_UnitDataManager.inst.GetUnitData(newUnitName);
        if (newUnitData != null)
        {
            selectedUnit.UnitInit(newUnitData);
            Debug.Log("업그레이드");

            // UI 업데이트
            uiManager.UpdateUnitInfoPanel(selectedUnit);
        }
        else
        {
            Debug.LogError("유닛 데이터 없음");
        }
    }
}
