using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public LayerMask hitLayers;
    public int damage;
    public float speed;
    public float falloff;
    [HideInInspector] public Vector3 direction;
    RaycastHit hit;

    void Update()
    {
        Vector3 nextPosition = transform.position + (direction * speed + Vector3.down * falloff) * Time.deltaTime;

        if (Physics.Linecast(transform.position, nextPosition, out hit, hitLayers))
        {
            //Debug.Log("Bullet hits: " + hit.transform.name);
            if (hit.transform.tag == "Player")
            {
                hit.transform.SendMessage("Hurt", damage);
            }
            Destroy(gameObject);
        }

        transform.position = nextPosition;
    }
}
