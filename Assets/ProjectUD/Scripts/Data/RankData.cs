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
    public Sprite rewardCommanderProfile;
    //public List<Sprite> rewardCommanderProfile;

    public struct Portrait
    {
        public Sprite portrait;
        public string portraitID;
    }

    public Portrait[] rewardPortraits;
}
