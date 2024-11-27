using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CONSTANT
{
    //이 클래스는 자주 사용하는 상수를 정리한 클래스입니다.

    public static string TAG_UNIT = "Unit"; //아군 병사 태그
    public static string TAG_ENEMY = "Enemy"; //적군 태그
    public static string TAG_TILE = "Tile"; //그리드 타일 태그
    public static string TAG_GROUND = "Ground"; //지형 태그
    public static string TAG_BASE = "Base"; //성 태그
    public static string TAG_ATTACK = "Attack"; //공격 판정 태그
    public static string TAG_DEAD_UNIT = "DeadUnit";//유닛이 죽음 상태를 나타내기위한 태그.

    public static string LAYER_DRAW_OUTLINE = "DrawOutline";
    public static string LAYER_IGNORE_OUTLINE = "IgnoreOutline";

    public static string ANITRIGGER_DEAD = "IsDead";
    public static string ANIBOOL_RUN = "IsRunning";
    public static string ANITRIGGER_ATTACK = "AttackTrigger";
    public static string ANITRIGGER_SPECIAL = "SpecialTrigger";


}

