using UnityEngine;
using StatsToChange = PassiveCommandSkill.StatsToChange;

[CreateAssetMenu(fileName = "PassiveCommandSkillData", menuName = "ProjectUD/PassiveCommandSkillData")]
public class PassiveCommandSkillData : CommandSkillData
{
    [Header("■ PassiveCommandSkill")]
    [SerializeField] private StatsToChange statsToChange;
    [SerializeField] private float amount;


    public StatsToChange StatsToChange => statsToChange;
    public float Amount => amount;

}
