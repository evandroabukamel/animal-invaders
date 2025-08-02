using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RuleSet", menuName = "ScriptableObjects/Values/RuleSet", order = 1)]
public class RuleSet : ScriptableObject
{
    [SerializeField] public List<float> SpawnIntervalByLevel;
    [SerializeField] public int LevelDuration;
    [SerializeField] public int TimeForBossAppearance;
    [SerializeField] public List<int> EnemiesKilledByLevelToReleasePowerUp;
}
