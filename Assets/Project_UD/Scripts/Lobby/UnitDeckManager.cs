using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UnitDeckManager : MonoBehaviour
{
    public Image[] unitDeckList = null;
    public Image[] unitList = null;

    public Button[] unitDeckButton = null;
    public Button[] unitListButton = null;

    private Sprite emptyDeckImageSprite = null;
    private Color emptyDeckImageColor;

    private bool[] isUnitDeckEmpty;
    private bool[] isUnitSelect;

    private int[] unitDeckIndex;

    public Button StageStartBtn = null;




    // Start is called before the first frame update
    void Start()
    {
        emptyDeckImageSprite = unitDeckList[0].sprite;
        emptyDeckImageColor = unitDeckList[0].color;

        isUnitDeckEmpty = new bool[unitDeckList.Length];

        unitDeckIndex = new int[unitDeckList.Length];

        for (int i = 0; i < isUnitDeckEmpty.Length; i++)
        {
            isUnitDeckEmpty[i] = true;
            unitDeckIndex[i] = -1;
        }

        isUnitSelect = new bool[unitList.Length];

        for (int i = 0; i < isUnitSelect.Length; i++)
        {
            isUnitSelect[i] = false;
        }

        for (int i = 0; i < unitListButton.Length; i++)
        {
            
            if (unitListButton[i] != null)
            {
                int unitIndex = i;
                unitListButton[i].onClick.AddListener(() => _addUnitToDeck(unitIndex));
            }
        }

        for (int i = 0; i < unitDeckButton.Length; i++)
        {

            if (unitDeckButton[i] != null)
            {
                int deckIndex = i;
                unitDeckButton[i].onClick.AddListener(() => _clearUnitDeck(deckIndex));
            }
        }

        if (StageStartBtn != null)
        {
            StageStartBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadSceneAsync("Stage 1");
            });
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void _addUnitToDeck(int unitIndex)
    {
        if (isUnitSelect[unitIndex])
        {
            Debug.Log("À¯´Ö Áßº¹");
            return;
        }


        for (int i = 0; i < isUnitDeckEmpty.Length; i++)
        {
            if (isUnitDeckEmpty[i])
            {
                unitDeckList[i].sprite = unitList[unitIndex].sprite;
                unitDeckList[i].color = unitList[unitIndex].color;
                isUnitDeckEmpty[i] = false;
                isUnitSelect[unitIndex] = true;
                unitDeckIndex[i] = unitIndex;
                break;
            }
        }
    }

    void _clearUnitDeck(int deckIndex)
    {
        int unitIndex = unitDeckIndex[deckIndex];
        if (unitIndex != -1)
        {
            unitDeckList[deckIndex].sprite = emptyDeckImageSprite;
            unitDeckList[deckIndex].color = emptyDeckImageColor;
            isUnitDeckEmpty[deckIndex] = true;
            isUnitSelect[unitIndex] = false;
            unitDeckIndex[deckIndex] = -1;
        }
    }
}
