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

    public void runAnimation(int index)
    {
        currentAnimation = animations[index];
        previousFrameTime = Time.time;

        spriteIndex = 0;
        UpdateToSprite(spriteIndex);

        Debug.Log("Changed to " + currentAnimation.AnimationName + " " + spriteIndex);
    }

    void Update()
    {
        while (Time.time > previousFrameTime + currentAnimation.ChangeInterval)
        {
            ++spriteIndex;
            if (spriteIndex >= currentAnimation.Sprites.Length)
            {
                if (currentAnimation.StopAtEnd) spriteIndex = animations.Length - 1;
                else spriteIndex = 0;
            }

            UpdateToSprite(spriteIndex);

            previousFrameTime += currentAnimation.ChangeInterval;
        }
    }

    void UpdateToSprite(int index)
    {
        GetComponent<MeshRenderer>().material = currentAnimation.Sprites[spriteIndex];
    }
}
