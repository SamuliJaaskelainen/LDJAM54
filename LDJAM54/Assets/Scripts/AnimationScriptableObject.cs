using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpawnAnimationScriptableObject", order = 1)]
public class AnimationScriptableObject : ScriptableObject
{
    [SerializeField] string animationName;
    public string AnimationName { get => animationName; }

    [SerializeField] Texture[] sprites;
    public Texture[] Sprites { get => sprites; }

    [SerializeField] float changeInterval;
    public float ChangeInterval { get => changeInterval; }

    [SerializeField] bool stopAtEnd;
    public bool StopAtEnd { get => stopAtEnd; }
}
