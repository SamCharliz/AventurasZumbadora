using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject bee;
    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - bee.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = bee.transform.position + offset;
        transform.LookAt(bee.transform);
    }
}
