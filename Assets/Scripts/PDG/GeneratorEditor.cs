using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AbstractDG), true)]
public class GeneratorEditor : Editor
{
    AbstractDG generator;

    private void Awake() 
    {
        generator = (AbstractDG)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Generate BinarySpacePartitioning Dungeon")) 
        {
            generator.GenerateDungeon();
        }
    }
}
