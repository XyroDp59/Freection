using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrippableObject : MonoBehaviour
{
    [SerializeField] float hookInsideDist;
    [SerializeField] float maxDistance;
    [SerializeField] float minDistance;
    private Collider col;
    public Bounds bounds;

    void Awake()
    {
        col = GetComponent<Collider>();
        bounds = GetComponent<Renderer>().bounds;
    }


    public bool IsGrippable(Transform player, Vector3 point)
    {
        float distSphere = Vector3.Distance(player.position, point);
        return distSphere > minDistance && distSphere < maxDistance;
    }

    public Vector3 GetPreferredGrabPoint(Transform camera)
    {
        Ray ray = new Ray();
        ray.origin = camera.position;
        ray.direction = camera.forward;
        if (col.Raycast(ray, out var hit, maxDistance))
            return hit.point;
        else 
        {
            Vector3 nearPos = col.ClosestPoint(camera.position + camera.forward * Vector3.Distance(camera.position, transform.position));
            return nearPos + (transform.position - nearPos).normalized * hookInsideDist;
        }
    }
}
