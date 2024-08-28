using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class UD_Ingame_WaveSystemManager : MonoBehaviour
{
    [Header("====WaveData====")]
    public int waveMax = 10;
    public UnitType[][] unitType;

    public int waveCur = 1;
    
    public float waveStartDelay = 0;
    float waveStartDelayCur = 0;

    public GameObject[] remainEnemy;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        //적이 한명도 없을경우 다음 웨이브로 진행.
        if (remainEnemy.Length != 0)
        {
            Debug.Log("Wave Complete!");

            if (waveCur >= waveMax)
            {
                Debug.Log("Stage Complete!");
                //TODO : 스테이지 완료 시퀀스 작성.
            }
            else
            {
                if (waveStartDelayCur > 0)
                {
                    waveStartDelayCur -= Time.deltaTime;
                }
                else
                {
                    waveCur++;
                    waveStartDelayCur = waveStartDelay;
                }
            }
        }



    }
}
