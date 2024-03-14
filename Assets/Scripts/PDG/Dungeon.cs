using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dungeon_", menuName = "Dungeons")]
public class Dungeon : ScriptableObject
{
    public int iterations = 10, walkRadius = 10;
    public bool startRandomlyEachIteration = true;
    public int width;
    public int height;
    public int minRoomWidth;
    public int minRoomHeight;
    public int amountOfRooms;
    public int corridorLength;
    [Range(0, 10)] public int offset = 1;
}
