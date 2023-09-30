using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public class Billboard : MonoBehaviour
{
    Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        Vector3 target = new Vector3
           (mainCamera.transform.position.x,
            transform.position.y,
            mainCamera.transform.position.z);

        transform.LookAt(target);
        transform.Rotate(0, 180, 0);
    }
}
