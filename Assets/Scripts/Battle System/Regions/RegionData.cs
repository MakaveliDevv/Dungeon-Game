using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionData : MonoBehaviour
{
    public int maxEnemiesEncounter = 4;
    public string BattleScene;
    public List<GameObject> possibleEnemies = new();
}
