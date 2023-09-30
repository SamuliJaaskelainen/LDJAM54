using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpawnAnimationScriptableObject", order = 1)]
public class AnimationScriptableObject : ScriptableObject
{
    public string prefabName;

    [SerializeField] Material[] sprites;
    public Material[] Sprites { get => sprites; }

    [SerializeField] float changeInterval;
    public float ChangeInterval { get => changeInterval; }
}
