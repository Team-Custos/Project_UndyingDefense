using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitCurDebuff
{
    public UnitDebuff name;      // 디버프 이름
    public int stack;            // 디버프 스택
    public float duration;       // 디버프 전체 지속 시간
    public float currentTime;    // 디버프 현재 남은 시간
}

public class UnitDebuffManager : MonoBehaviour
{
    UnitDebuffData[] debuffData;

    [SerializeField]
    public List<UnitCurDebuff> activeDebuffs = new List<UnitCurDebuff>(); // 현재 유닛의 디버프 목록

    private void Awake()
    {
        debuffData = InGameManager.inst.unitStatusCtrl.debuffDatas;
    }

    // 매 프레임 디버프 업데이트
    void Update()
    {
        for (int i = activeDebuffs.Count - 1; i >= 0; i--) // 리스트 역순으로 순회 (삭제 시 오류 방지)
        {
            activeDebuffs[i].currentTime -= Time.deltaTime;

            // 디버프 시간이 끝난 경우
            if (activeDebuffs[i].currentTime <= 0)
            {
                RemoveDebuff(activeDebuffs[i]);
            }
        }

        //디버프 실행 함수 구현해야함.
    }

    // 디버프 추가 함수
    public void AddDebuff(UnitDebuff debuff)
    {
        if (debuffData == null)
        {
            Debug.LogError("debuffData is null. Check the source of debuffData.");
            return;
        }


        for (int i = 0; i < debuffData.Length; i++)
        {
            if (debuff == debuffData[i].name) // 이미 해당 디버프가 있는지 확인
            {
                UnitCurDebuff existingDebuff = activeDebuffs.Find(d => d.name == debuffData[i].name);

                if (existingDebuff != null) // 디버프가 이미 있으면 시간 초기화 및 스택 증가
                {
                    existingDebuff.currentTime = debuffData[i].duration;
                    if (existingDebuff.stack < debuffData[i].stackLimit)
                    {
                        existingDebuff.stack++;
                    }
                }
                else //없을 경우 새로운 디버프 추가
                {
                    activeDebuffs.Add(new UnitCurDebuff
                    {
                        name = debuffData[i].name,
                        stack = 1,
                        duration = debuffData[i].duration,
                        currentTime = debuffData[i].duration
                    });

                }
            }
        }
    }

    // 디버프 제거
    private void RemoveDebuff(UnitCurDebuff debuff)
    {
        activeDebuffs.Remove(debuff);
    }

    // 특정 디버프 찾기 함수
    public bool HasDebuff(UnitDebuff debuff)
    {
        return activeDebuffs.Exists(d => d.name == debuff);
    }
}