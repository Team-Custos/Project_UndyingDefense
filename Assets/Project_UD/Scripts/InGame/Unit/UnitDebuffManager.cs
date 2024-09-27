using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


[System.Serializable]
public class UnitCurDebuff
{
    public UnitDebuff name;
    public int stack;
    public float Time;
    public float Cur_Time;
}

public enum UnitDebuff
{
    Bleed,
    Dizzy,
    Stun,
    Tied,
    Burn,
    Inferno
}

public class UnitDebuffManager : MonoBehaviour
{
    public Dictionary<int, UnitDebuff> GeneralSkillCodeToDebuff = new Dictionary<int, UnitDebuff>()
    {
        {101 , UnitDebuff.Bleed},
        {102 , UnitDebuff.Bleed},
    };

    public Dictionary<int, UnitDebuff> SpecialSkillCodeToDebuff = new Dictionary<int, UnitDebuff>()
    {
        {101 , UnitDebuff.Dizzy},
        {102 , UnitDebuff.Tied},
    };

    public UnitCurDebuff[] Debuffs2Manage; // 배열로 유지

    void Update()
    {
        for (int idx = 0; idx < Debuffs2Manage.Length; idx++)
        {
            UpdateDebuff(Debuffs2Manage[idx], idx);
        }
    }

    // 디버프 추가 함수
    public void AddUnitDebuff(UnitDebuff debuffToAdd)
    {
        UnitDebuffData debuffData = new UnitDebuffData();
        UnitCurDebuff existingDebuff = FindDebuff(debuffToAdd);

        if (existingDebuff != null)
        {
            // 이미 있는 디버프일 경우 시간 초기화 및 스택 관리
            existingDebuff.Cur_Time = debuffData.Time;
            if (existingDebuff.stack < debuffData.stackLimit)
            {
                existingDebuff.stack++;
            }
        }
        else
        {
            // 새로운 디버프 추가
            if (Debuffs2Manage.Length == 0)
            {
                Debuffs2Manage[0] = new UnitCurDebuff
                {
                    name = debuffToAdd,
                    Cur_Time = debuffData.Time,
                    stack = 1
                };
            }
            else
            {
                
            }

            

            for (int idx = 0; idx < Debuffs2Manage.Length; idx++)
            {
                if (Debuffs2Manage[idx] == null) // 빈 자리에 추가
                {
                    Debuffs2Manage[idx] = new UnitCurDebuff
                    {
                        name = debuffToAdd,
                        Cur_Time = debuffData.Time,
                        stack = 1
                    };
                    UnitCurDebuff[] resizedArray = new UnitCurDebuff[Debuffs2Manage.Length + 1];
                    Array.Copy(Debuffs2Manage, resizedArray, Debuffs2Manage.Length);
                    Debuffs2Manage = resizedArray;
                    break;
                }
            }
        }
    }

    // 디버프 업데이트 함수
    void UpdateDebuff(UnitCurDebuff debuff, int idx)
    {
        if (debuff.Cur_Time <= 0)
        {
            // 시간이 다된 디버프 삭제
            Debuffs2Manage[idx] = null; // 배열에서 해당 요소 제거
            Debuffs2Manage = Debuffs2Manage.Where(x => x != null).ToArray(); // null 제거
        }
        else
        {
            debuff.Cur_Time -= Time.deltaTime; // 시간 감소
        }
    }

    // 특정 디버프 찾는 함수
    UnitCurDebuff FindDebuff(UnitDebuff debuffToFind)
    {
        return Debuffs2Manage.FirstOrDefault(d => d != null && d.name == debuffToFind);
    }
}


/*
[System.Serializable]
public class UnitCurDebuff
{
    public UnitDebuff name;
    public int stack;
    public float Time;
    public float Cur_Time;
}

public enum UnitDebuff
{
    Bleed,
    Dizzy,
    Stun,
    Tied,
    Burn,
    Inferno
}

//-> 효과들을 따로 데이터로 분리. -> 현재 데이터(배열의 요소들을 체크.)에 따라서 효과 적용.

//지금 구현해야 할 효과들
//- 출혈
//- 충격 -> 기절
//- 속박 


public class UD_Ingame_UnitDebuffManager : MonoBehaviour
{
    public List<UnitDebuff> UnitCurDebuff = new List<UnitDebuff>();

    public UnitCurDebuff[] Debuffs2Manage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int idx = 0; idx < UnitCurDebuff.Count; idx++)
        {
            UnitDebuffUpdate(UnitCurDebuff[idx]);
        }
    }
    
    //디버프 추가
    public void AddUnitDebuff(UnitDebuff debuff2Add)
    {
        UnitDebuffData debuffData = new UnitDebuffData();

        if (UnitCurDebuff.Contains(debuff2Add))//이미 있는 디버프일경우 쿨타임 초기화. 스택 할수 있다면 스택 추가. 
        {
            for (int idx = 0; idx < Debuffs2Manage.Length; idx++)
            {
                if (Debuffs2Manage[idx].name == debuff2Add)
                {
                    Debuffs2Manage[idx].Cur_Time = debuffData.Time;
                    if (Debuffs2Manage[idx].stack < debuffData.stackLimit)
                    {
                        Debuffs2Manage[idx].stack++;
                    }
                    else
                    {
                       //다른 상태이상으로 변화하거나 그냥 놔둠.
                    }
                    
                }
            }
        }
        else
        {
            UnitCurDebuff.Add(debuff2Add);
            for (int idx = 0; idx < Debuffs2Manage.Length; idx++)
            {
                if (Debuffs2Manage[idx].name == debuff2Add)
                {
                    Debuffs2Manage[idx].Cur_Time = debuffData.Time;
                    Debuffs2Manage[idx].stack = 1;
                }
            }
        }
    }

    //디버프 업데이트 (남은 시간 체크)
    void UnitDebuffUpdate(UnitDebuff debuff)
    {
        if (UnitCurDebuff.Contains(debuff))
        {
            for (int idx = 0; idx < Debuffs2Manage.Length; idx++)
            {
                if (Debuffs2Manage[idx].name == debuff)
                {
                    if (Debuffs2Manage[idx].Cur_Time <= 0)
                    {
                        //디버프 삭제.
                        UnitCurDebuff.Remove(debuff);
                        Debuffs2Manage = Debuffs2Manage
                        .Where((item, index) => index != idx)
                        .ToArray();
                        break;
                    }
                    else
                    {
                        Debuffs2Manage[idx].Cur_Time -= Time.deltaTime;
                    }
                }
            }
        }
    }
}
*/
