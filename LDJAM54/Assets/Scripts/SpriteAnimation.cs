using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpriteAnimation : MonoBehaviour
{
    [SerializeField] AnimationScriptableObject defaultAnimation;

    [SerializeField] AnimationScriptableObject[] animations;

    AnimationScriptableObject currentAnimation;

    private Material spriteMaterial;
    


    int spriteIndex = 0;
    float previousFrameTime;

    void Start()
    {
        currentAnimation = defaultAnimation;

        previousFrameTime = Time.time;
        
        // duplicate the material on the renderer so we can change the texture
        spriteMaterial = new Material(GetComponent<Renderer>().material);
        
        GetComponent<Renderer>().material = spriteMaterial;
        
        
    }

    public void runAnimation(int index)
    {
        currentAnimation = animations[index];
        previousFrameTime = Time.time;

        spriteIndex = 0;
        UpdateToSprite(spriteIndex);

        //Debug.Log("Changed to " + currentAnimation.AnimationName + " " + spriteIndex);
    }
    

    void Update()
    {

        // The code below is the same as the code above, except it's broken and doesn't work
        while (Time.time > previousFrameTime + currentAnimation.ChangeInterval)
        {
            ++spriteIndex;
            if (spriteIndex >= currentAnimation.Sprites.Length)
            {
                if (currentAnimation.StopAtEnd) spriteIndex = currentAnimation.Sprites.Length - 1;
                else spriteIndex = 0;
            }

            UpdateToSprite(spriteIndex);

            previousFrameTime = Time.time; // Update the previousFrameTime.
        }
    }

    void UpdateToSprite(int index)
    {
        spriteMaterial.mainTexture = currentAnimation.Sprites[index];
        
    }
}
