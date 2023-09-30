using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpriteAnimation : MonoBehaviour
{
    [SerializeField] Material[] sprites;
    [SerializeField] float      changeInterval;

    int spriteIndex = 0;
    float previousFrameTime;

    void Start()
    {
        previousFrameTime = Time.time;
    }

    void Update()
    {
        while (Time.time > previousFrameTime + changeInterval)
        {
            ++spriteIndex;
            if (spriteIndex >= sprites.Length) spriteIndex = 0;

            GetComponent<MeshRenderer>().material = sprites[spriteIndex];

            previousFrameTime += changeInterval;
        }
    }
}
