using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UD_Ingame_ClickPosIndicatorCtrl : MonoBehaviour
{
    public float showHideTime = 0;
    float showHideTime_Cur = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.activeSelf)
        {
            if (showHideTime_Cur < showHideTime)
            {
                showHideTime_Cur += Time.deltaTime;
            }
            else
            {
                this.gameObject.SetActive(false);
                showHideTime_Cur = 0;
            }
        }

        
    }
}
