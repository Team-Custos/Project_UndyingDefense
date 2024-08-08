using System.Collections;
using System.Collections.Generic;
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
    a,
    b,

}



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
            
        }


    }

    void UnitDebuffUpdate(UnitDebuff debuff)
    {
        if (UnitCurDebuff.Contains(debuff))
        {
            for (int idx = 0; idx < Debuffs2Manage.Length; idx++)
            {
                if (Debuffs2Manage[idx].name == debuff)
                {
                    Debuffs2Manage[idx].Cur_Time = Debuffs2Manage[idx].Time;

                    


                }
            }
        }
    }
}
