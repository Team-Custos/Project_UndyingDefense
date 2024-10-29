using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//클릭한 좌표를 나타내기위한 오브젝트를 관리하는 스크립트(삭제 예정.)
public class Ingame_ClickPosIndicatorCtrl : MonoBehaviour
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
