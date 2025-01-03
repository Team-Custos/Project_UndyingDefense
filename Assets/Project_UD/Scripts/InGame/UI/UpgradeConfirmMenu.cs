using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeConfirmMenu : MonoBehaviour
{
    [SerializeField] public Button closeBtn;
    [SerializeField] public Button confirmBtn;
    [SerializeField] public Button cancelBtn;

    private void Awake()
    {
        //if (closeBtn == null)
        //{
        //    closeBtn = transform.Find("UpgradeCheckCloseBtn").GetComponent<Button>();
        //}
        //if (confirmBtn == null)
        //{
        //    confirmBtn = transform.Find("UpgradeCheckOkBtn").GetComponent<Button>();
        //}
        //if (cancelBtn == null)
        //{
        //    cancelBtn = transform.Find("UpgradeCheckCancleBtn").GetComponent<Button>();
        //}
    }
}
