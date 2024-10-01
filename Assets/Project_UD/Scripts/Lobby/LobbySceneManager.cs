using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LbbySceneManager : MonoBehaviour
{
    public Button unitDeckButton = null;
    public GameObject unitDeckPanel = null;

    // Start is called before the first frame update
    void Start()
    {
        unitDeckPanel.SetActive(false);

        if (unitDeckButton != null)
        {
            unitDeckButton.onClick.AddListener(_OnOffUnitDeckPanel);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void _OnOffUnitDeckPanel()
    {
        if (unitDeckPanel != null)
        {
            bool isActive = unitDeckPanel.activeSelf;
            unitDeckPanel.SetActive(!isActive);
        }
    }
}