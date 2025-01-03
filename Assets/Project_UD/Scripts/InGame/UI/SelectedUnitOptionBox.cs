using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedUnitOptionBox : MonoBehaviour
{
    [SerializeField] private Button unitStateChangeBtn;
    [SerializeField] private Button unitUpgradeBtn;

    public Button UnitStateChangeBtn => unitStateChangeBtn;
    public Button UnitUpgradeBtn => unitUpgradeBtn;

    private void Awake()
    {
        // null시 실행되도록
        if (unitStateChangeBtn == null)
        {
            unitStateChangeBtn = transform.Find("ChangeStateBtn").GetComponent<Button>();
        }
        if (unitUpgradeBtn == null)
        {
            unitUpgradeBtn = transform.Find("UnitUpgradeBtn").GetComponent<Button>();
        }
    }

}
