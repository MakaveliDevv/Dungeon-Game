using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomWalkGenerator : AbstractDG
{
    [SerializeField] protected Dungeon dungeon;

    protected override void RunProceduralGeneration() {}


    // THIS METHOD GENERATES A RANDOM PATH
    protected HashSet<Vector2Int> StartRandomGeneration(Dungeon _dungeonParam, Vector2Int _position) 
    {
        var currentPosition = _position;
        HashSet<Vector2Int> floorPosition = new();

        for (int i = 0; i < _dungeonParam.iterations; i++)
        {
            // Generate for each iteration a random path
            var path = GeneratorAlgorithms.RandomPathGeneration(currentPosition, _dungeonParam.walkRadius);
            floorPosition.UnionWith(path);

            // Check if the random iteration button is checked
            if(_dungeonParam.startRandomlyEachIteration) 
            {   
                currentPosition = floorPosition.ElementAt(Random.Range(0, floorPosition.Count));
            }
        }
        return floorPosition;
    }
}
