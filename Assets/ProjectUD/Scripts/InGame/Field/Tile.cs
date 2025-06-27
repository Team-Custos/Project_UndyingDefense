using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Tile : MonoBehaviour
{
    // 타일마다 가지고 있는 스크립트
    // 타일이 유닛(시즈 모드인 상태)이 위치하고 있는걸 확인

    [SerializeField] BoxCollider boxCollider;
    private AllyUnit tileAllyUnit; // 현재 위치된 유닛
    public AllyUnit TileAllyUnit => tileAllyUnit;

    // 타일에 유닛이 있는지 반환 
    public AllyUnit SetAllyUnit(AllyUnit allyUnit)
    {
        if (this.tileAllyUnit == null)
        {
            this.tileAllyUnit = allyUnit;
            return allyUnit; // 새로 배치 성공
        }

        if (this.tileAllyUnit == allyUnit)
        {
            return allyUnit; // 이미 배치된 유닛이면 OK
        }

        return null; // 다른 유닛이 이미 있음
    }


    public void ClearUnit()
    {
        tileAllyUnit = null;
    }
}
