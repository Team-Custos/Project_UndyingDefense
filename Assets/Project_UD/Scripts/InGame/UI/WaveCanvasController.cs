using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveCanvasController : MonoBehaviour
{
    [Header("Wave UI")]
    public GameObject waveStepPanel;        // 현재 웨이브 단계 표시 판넬
    public Text waveStepText;               // 현재 웨이브 단계 표시
    public GameObject waveStartPanel;       // 웨이브가 시작을 알리는 판넬
    public Text waveStartText;              // 웨이브 시작 텍스트
    public GameObject waveCountTextPanel;   // 웨이브 카운트 다운 텍스트 판넬
    public Text waveCountText;              // 웨이브 카운트 다운 텍스트
    public Button waveCountSkipBtn;         // 웨이브 카운트 다운 스킵 버튼
    public GameObject waveDefenseSuccessPaenl;   // 웨이브 성공 판넬

    public int waveCount = 20;              // 웨이브 카운트 텍스트 판넬
    public Button waveResultPanel;          // 웨이브 결과 판넬 : 승리 or 패배

    public Button winWaveRestartBtn;
    public GameObject waveResultWinPanel;
    public Button Winlobbybtn;
    public GameObject waveResultLosePanel;
    public Button loseWaveRestartBtn;
    public Button loselobbybtn;
    public GameObject waveWarnningPanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
