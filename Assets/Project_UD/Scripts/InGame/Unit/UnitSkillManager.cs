using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSkillManager : MonoBehaviour
{
    public Dictionary<int, bool> TargetCellPlaceable = new Dictionary<int, bool>
    { };

    public List<int> TargetCellIdx = new List<int>();

    public GameObject Bow;
    public GameObject Sword;

    public GameObject Trap;

    float weaponCooldown_Cur = 0;
    float skillCooldown_Cur = 0;

    public Ingame_UnitCtrl UnitCtrl;
    GridManager GridManager;

    public GameObject SetObject = null;

    int TargetCellIdxFinal = 0;

    private void Awake()
    {
        GridManager = GridManager.inst;
    }

    public void UnitGeneralSkill(int SkillCode, GameObject TargetEnemy, float weaponCooldown, bool isEnemyAttack)
    {
        Vector3 TargetPos = TargetEnemy.transform.position;

        //���� ���� ���� (������, ġ��Ÿ Ȯ��, ���� Ÿ��, �����(�ʿ��� ���))
        int damage = 0;
        AttackType attackType = AttackType.UnKnown;
        UnitDebuff debuff = UnitDebuff.None;

        if (weaponCooldown_Cur <= 0)
        {
            switch (SkillCode)
            {
                //�� ����
                case 101:
                    //������ �տ� �ִ� �� ���� ���� ���� 5 �������� ���� ������ ���Ѵ�. ġ��Ÿ �ߵ� �� ���� ȿ��
                    damage = 5;
                    attackType = AttackType.Slash;
                    debuff = UnitDebuff.Bleed;
                    break;
                //Ȱ ���
                case 102:
                    //ȭ���� ��� �� ���� ������ 5 �������� ���� ������ ���Ѵ�. ġ��Ÿ �ߵ� �� ���� ȿ��
                    if (Bow != null && Bow.GetComponent<BowCtrl>() != null)
                    {
                        Bow.transform.LookAt(TargetPos);
                        Bow.GetComponent<BowCtrl>().ArrowShoot(isEnemyAttack);
                        if (TargetEnemy.gameObject.CompareTag(CONSTANT.TAG_ENEMY))
                        {
                            Ingame_UnitCtrl EnemyCtrl = TargetEnemy.GetComponent<Ingame_UnitCtrl>();
                            EnemyCtrl.ReceivePhysicalDamage(UnitCtrl.unitData.attackPoint, UnitCtrl.unitData.critChanceRate, AttackType.Pierce, UnitDebuff.Bleed);
                        }
                        else
                        {
                            BaseStatus BaseCtrl = TargetEnemy.GetComponent<BaseStatus>();
                            BaseCtrl.ReceiveDamage(UnitCtrl.unitData.attackPoint);
                        }
                    }
                    break;
                //â ���
                case 201:
                    damage = 7;
                    attackType = AttackType.Pierce;
                    debuff = UnitDebuff.Bleed;
                    break;
                //��ġ ����ġ��
                case 202:
                    damage = 7;
                    attackType = AttackType.Crush;
                    debuff = UnitDebuff.Dizzy;
                    break;
                //���� ���
                case 203:
                    damage = 7;
                    attackType = AttackType.Pierce;
                    debuff = UnitDebuff.Bleed;
                    break;
            }

            if (TargetEnemy.gameObject.CompareTag(CONSTANT.TAG_ENEMY))
            {
                Ingame_UnitCtrl EnemyCtrl = TargetEnemy.GetComponent<Ingame_UnitCtrl>();
                int HitSoundRandomNum = Random.Range(0, 2);
                AudioClip SFX2Play = UnitCtrl.unitData.attackSound[HitSoundRandomNum];

                UnitCtrl.soundManager.PlaySFX(UnitCtrl.soundManager.ATTACK_SFX ,SFX2Play);
                EnemyCtrl.ReceivePhysicalDamage(damage, UnitCtrl.unitData.critChanceRate, attackType, debuff);
            }
            else
            {
                BaseStatus BaseCtrl = TargetEnemy.GetComponent<BaseStatus>();
                BaseCtrl.ReceiveDamage(UnitCtrl.unitData.attackPoint);
            }
            weaponCooldown_Cur = weaponCooldown;
        }
        else
        {
            weaponCooldown_Cur -= Time.deltaTime;
        }
    }

    int SkillDamageInit()
    {
        //��ų ������ ������ ��������.
        return 1;
    }


    public void UnitSpecialSkill(int SkillCode, float skillCooldown)
    {
        int SkillDamage = SkillDamageInit();
        Ingame_UnitCtrl TargetEnemy = null;

        if (UnitCtrl.targetEnemy != null)
        {
            TargetEnemy = UnitCtrl.targetEnemy.GetComponent<Ingame_UnitCtrl>();
        }

        if (skillCooldown_Cur <= 0)
        {
            skillCooldown_Cur = skillCooldown;
            switch (SkillCode)
            {
                //�� ���
                case 101:
                    //������ �տ� �ִ� �� ���� ���� ���� ��� 5 �������� ���� ������ ���Ѵ�. ġ��Ÿ�� 5% ����. ġ��Ÿ �ߵ� �� ��� ȿ��
                    // �˺���� ���� ó��. ���ȸ� �ٸ���.
                    if (TargetEnemy == null)
                    {
                        return;
                    }
                    TargetEnemy.ReceivePhysicalDamage(SkillDamage, UnitCtrl.unitData.critChanceRate + 5, AttackType.Pierce, UnitDebuff.Dizzy);
                    break;

                //����
                case 102:
                    if (TargetEnemy == null)
                    {
                        return;
                    }

                    StartCoroutine(DoubleShot());

                    IEnumerator DoubleShot() // 2���� �ڷ�ƾ
                    {
                        Debug.Log("Double shot 1");
                        Bow.GetComponent<BowCtrl>().ArrowShoot(false);
                        TargetEnemy.ReceivePhysicalDamage(SkillDamage, UnitCtrl.unitData.critChanceRate + 5, AttackType.Pierce, UnitDebuff.Bleed);
                        // ���� �ð� �� �� ��° �Ѿ� �߻�
                        yield return new WaitForSeconds(0.5f);
                        Debug.Log("Double shot 2");
                        Bow.GetComponent<BowCtrl>().ArrowShoot(false);
                        TargetEnemy.ReceivePhysicalDamage(SkillDamage, UnitCtrl.unitData.critChanceRate + 5, AttackType.Pierce, UnitDebuff.Bleed);
                    }
                    break;
                //�ָ�����
                case 201:
                    //â�� �޸� ������ �ָ� �ִ� ���� ���� 7 �������� ���� ������ ���Ѵ�. ġ��Ÿ�� 10% ����. ġ��Ÿ �ߵ� �� ���� ȿ��
                    //��ų�� ����Ҷ��� �Ÿ� ������ ���� �ؾ��Ұ� ����. -> �þ߹���? ��ȹ�ʰ� �̾߱Ⱑ �ʿ��Ұ� ����.

                    TargetEnemy.ReceivePhysicalDamage(SkillDamage, UnitCtrl.unitData.critChanceRate + 10, AttackType.Slash, UnitDebuff.Bleed);

                    break;
                //����ġ��
                case 202:
                    TargetEnemy.ReceivePhysicalDamage(SkillDamage, UnitCtrl.unitData.critChanceRate + 10, AttackType.Crush, UnitDebuff.Stun);
                    break;
                //����ź ��ô
                case 203:
                    //����ź�� ���� ���� ���� �ִ� ��� ������ 10 �������� �м� ������ ���Ѵ�. ġ��Ÿ�� 10% ����. ġ��Ÿ �ߵ� �� ���� ȿ��
                    //�� ��ġ�� ����ϰ� ����. ���ִ� ĭ�� �������� �ٶ󺸴� ���� ��ĭ �տ� ���� Ʈ���� ������Ʈ�� ��ġ�ϴ� ������.
                    //��, �̸� ��ġ �Ǿ��ִ����� �������� ����.
                    TargetEnemy.ReceivePhysicalDamage(SkillDamage, UnitCtrl.unitData.critChanceRate + 10, AttackType.Crush, UnitDebuff.Stun);
                    break;
                //���� ��ġ
                case 204:
                    //���ִ� ĭ�� �������� �ٶ󺸴� ���� 2ĭ �տ� �浣�� ��ġ�Ͽ� �������� �� ���� ������ 5 �������� ���� ������ ���Ѵ�. ġ��Ÿ���� 5% ����. ġ��Ÿ �ߵ� �� �ӹ� ȿ��.
                    //�̹� ��ġ�� �浣�� �ߵ��Ǳ� ���� �ش� ��ų�� �ٽ� ����Ѵٸ�, ������ ��ġ�� �浣�� �μ����� �ش� ��ų�� �ߵ��Ǿ� ���� �浣�� ��ġ�Ѵ�.
                    //-> �� ��ġ �����ؼ� ��ȹ���̶� �̾߱⸦ �ؾ��� �ʿ䰡 �־��.

                    if (SetObject != null)
                    {
                        GridManager.SetTilePlaceable(SetObject.transform.position, true, true);
                        Destroy(SetObject);
                        SetObject = null;
                    }

                    float CurAngle = gameObject.transform.eulerAngles.y;
                    Debug.Log("���� ����: " + CurAngle);
                    Vector2 CurCellPos = UnitCtrl.unitPos;
                    Vector2[] TargetCells = new Vector2[3];

                    // ������ ���� x, y �������� ����
                    Vector2[] directionOffsets;
                    if (CurAngle <= 45 || CurAngle > 315)       // ����
                    {
                        directionOffsets = new Vector2[] { new Vector2(-1, 2), new Vector2(0, 2), new Vector2(1, 2) };
                    }
                    else if (CurAngle <= 135 && CurAngle > 45)   // ����
                    {
                        directionOffsets = new Vector2[] { new Vector2(2, 1), new Vector2(2, 0), new Vector2(2, -1) };
                    }
                    else if (CurAngle <= 225 && CurAngle > 135)  // ����
                    {
                        directionOffsets = new Vector2[] { new Vector2(1, -2), new Vector2(0, -2), new Vector2(-1, -2) };
                    }
                    else if (CurAngle > 225 && CurAngle <= 315) // ����
                    {
                        directionOffsets = new Vector2[] { new Vector2(-2, -1), new Vector2(-2, 0), new Vector2(-2, 1) };
                    }
                    else
                    {
                        Debug.LogError("�� ��ġ ���� ���� ��� ����.");
                        directionOffsets = new Vector2[] { new Vector2(-1, 2), new Vector2(0, 2), new Vector2(1, 2) };
                    }

                    // TargetCells�� ������ �����Ͽ� ��ǥ ���
                    for (int i = 0; i < 3; i++)
                    {
                        TargetCells[i] = CurCellPos + directionOffsets[i];
                    }


                    Vector3 TargetCellWorldPos = Vector3.zero;

                    for (int i = 0; i < 3; i++)
                    {
                        if (!TargetCellPlaceable.ContainsKey(i))
                        {
                            TargetCellPlaceable.Add(i, GridManager.GetTilePlaceable(TargetCells[i]));
                        }
                        if (!TargetCellIdx.Contains(i))
                        {
                            TargetCellIdx.Add(i);
                        }
                    }

                    if (TargetCellIdx.Count > 0)
                    {
                        TargetCellIdxFinal = Random.Range(0, TargetCellIdx.Count - 1);
                        Debug.Log("���� : " + TargetCellIdxFinal);
                        int TargetCellFinal = TargetCellIdx[TargetCellIdxFinal];
                        Vector3 TargetCellFinalWorldPos = GridManager.mapGrid.CellToWorld(new Vector3Int((int)TargetCells[TargetCellFinal].x, (int)TargetCells[TargetCellFinal].y, 1));

                        while (TargetCellIdx.Count != 0)
                        {
                            TargetCellFinalWorldPos = GridManager.mapGrid.CellToWorld(new Vector3Int((int)TargetCells[TargetCellFinal].x, (int)TargetCells[TargetCellFinal].y, 1));

                            Debug.Log(TargetCellFinal + "�� ���� ��ġ�� �õ��մϴ�.");
                            if (GridManager.GetTilePlaceable(TargetCellFinalWorldPos))
                            {
                                Debug.Log("�ش� ���� ��ġ�� ���� �մϴ�.");
                                TargetCellWorldPos = TargetCellFinalWorldPos;
                                break;
                            }
                            else
                            {
                                Debug.Log("�ش� ���� ��ġ�� �Ұ��� �մϴ�.");
                                TargetCellIdx.RemoveAt(TargetCellIdxFinal);
                                if (TargetCellIdx.Count == 0)
                                {
                                    Debug.Log("��� ��ġ �Ұ�. ��ų ��Ÿ���� �ʱ�ȭ�մϴ�.");
                                    skillCooldown_Cur = 0;
                                    return;
                                }
                                else
                                {
                                    TargetCellIdxFinal = Random.Range(0, TargetCellIdx.Count - 1);
                                    Debug.Log("���ο� ���� : " + TargetCellIdxFinal);
                                    TargetCellFinal = TargetCellIdx[TargetCellIdxFinal];
                                    continue;
                                }
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("��� ��ġ �Ұ�. ��ų ��Ÿ���� �ʱ�ȭ�մϴ�.");
                        skillCooldown_Cur = 0;
                        return;
                    }

                    if (GridManager.GetTilePlaceable(TargetCellWorldPos))
                    {
                        Vector3 CellWorldPosFinal = TargetCellWorldPos + new Vector3(GridManager.mapGrid.cellSize.x * 0.5f, 0, GridManager.mapGrid.cellSize.y * 0.5f);

                        //GridManager.mapGrid.GetCellCenterWorld(new Vector3Int((int)TargetCells[TargetCellFinal].x, 1, (int)TargetCells[TargetCellFinal].y));
                        SetObject = Instantiate(Trap);
                        SetObject.transform.position = new Vector3(CellWorldPosFinal.x, -1, CellWorldPosFinal.z);
                        GridManager.SetTilePlaceable(TargetCellWorldPos, true, false);
                        TargetCellPlaceable.Clear();
                        TargetCellIdx.Clear();
                    }
                    break;
            }
        }
        else
        {
            skillCooldown_Cur -= Time.deltaTime;
        }


    }

    public void EnemyGeneralSkill(int SkillCode, Vector3 TargetPos)
    {
        switch (SkillCode)
        {
            //�� ����
            case 101:
                break;
            //Ȱ ���
            case 102:
                if (Bow != null)
                {
                    Bow.transform.LookAt(TargetPos);
                    Bow.GetComponent<BowCtrl>().ArrowShoot(true);
                }
                break;
        }
    }
}