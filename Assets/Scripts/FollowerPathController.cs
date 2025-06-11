using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class FollowerPathController : MonoBehaviour
{
    public PathCreator pathCreator;
    public float speed = 1;
    float distanceTravelled;
    public Rigidbody bee;

    public PathCreator bezierPath;
    public int puntoControlIndex;
    public Vector3 nuevaPosicion;

    // Update is called once per frame
    void Update()
    {
        distanceTravelled += speed * Time.deltaTime;
        transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled);
        transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled);
    }
}