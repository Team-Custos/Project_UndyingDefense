using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UD_UnitDeckManager : MonoBehaviour
{
    public Image[] unitDeckList = null;
    public Image[] unitList = null;

    public Button[] unitDeckButton = null;
    public Button[] unitListButton = null;

    private Sprite emptyDeckImageSprite = null;
    private Color emptyDeckImageColor;

    private bool[] isDeckEmpty;
    private bool[] isUnitSelect;



    // Start is called before the first frame update
    void Start()
    {
        emptyDeckImageSprite = unitDeckList[0].sprite;
        emptyDeckImageColor = unitDeckList[0].color;

        isDeckEmpty = new bool[unitDeckList.Length];

        for (int i = 0; i < isDeckEmpty.Length; i++)
        {
            isDeckEmpty[i] = true;
        }

        for (int i = 0; i < unitListButton.Length; i++)
        {
            int unitIndex = i;
            if (unitListButton[i] != null)
            {
                unitListButton[i].onClick.AddListener(() => _addUnitToDeck(unitIndex));
            }
        }



    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void _addUnitToDeck(int unitIndex)
    {
        // 유닛 중복 추가 불가능 구현 중
        //for (int i = 0; i < unitDeckList.Length; i++)
        //{
        //    if (!isDeckEmpty[i] && unitDeckList[i].sprite == unitList[unitIndex].sprite)
        //    {
        //        return;
        //    }
        //}

        for (int i = 0; i < unitDeckList.Length; i++)
        {
            if (isDeckEmpty[i])
            {
                unitDeckList[i].sprite = unitList[unitIndex].sprite;
                unitDeckList[i].color = unitList[unitIndex].color;
                isDeckEmpty[i] = false;
                break;
            }
        }
    }

    void _clearUnitDeck(int unitIndex)
    {

    }
}
