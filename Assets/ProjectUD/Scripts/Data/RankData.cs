using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Rank/RankData")]
public class RankData : ScriptableObject
{
    public int rank;
    public float requirePoint;
    public string commanderID;
    public List<string> rewardCommandSkillID;
    public List<Sprite> rewardCommanderProfile;
}
