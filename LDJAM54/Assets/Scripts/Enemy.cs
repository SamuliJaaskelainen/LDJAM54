using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public void Die()
    {
        enabled = false;
        GetComponent<BoxCollider>().enabled = false;

        // TODO: Add death animation and leave body on the ground instead of destroying the object
        Destroy(gameObject);
    }
}
