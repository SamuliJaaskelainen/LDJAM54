using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpriteAnimation : MonoBehaviour
{
    [SerializeField] AnimationScriptableObject defaultAnimation;

    [SerializeField] AnimationScriptableObject[] animations;

    AnimationScriptableObject currentAnimation;


    int spriteIndex = 0;
    float previousFrameTime;

    void Start()
    {
        currentAnimation = defaultAnimation;

        previousFrameTime = Time.time;
    }

    void runAnimation(int index)
    {
        currentAnimation = animations[index];
    }

    void Update()
    {
        while (Time.time > previousFrameTime + currentAnimation.ChangeInterval)
        {
            ++spriteIndex;
            if (spriteIndex >= currentAnimation.Sprites.Length) spriteIndex = 0;

            GetComponent<MeshRenderer>().material = currentAnimation.Sprites[spriteIndex];

            previousFrameTime += currentAnimation.ChangeInterval;
        }
    }
}
