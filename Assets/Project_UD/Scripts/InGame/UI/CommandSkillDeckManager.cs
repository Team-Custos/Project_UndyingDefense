using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class CommandSkillDeckManager : MonoBehaviour
{
    public Image commandSkillDeckImage;
    public Button commandSkillDeckBtn;

    [SerializeField] private int index;

    // Start is called before the first frame update
    void Start()
    {
        commandSkillDeckBtn.onClick.AddListener(() =>
        {
            UserDataModel.instance.skillDatas[index] = null;
            commandSkillDeckImage.sprite = null;
        });
    }

}
