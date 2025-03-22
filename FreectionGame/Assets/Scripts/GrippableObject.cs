using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrippableObject : MonoBehaviour
{
    [SerializeField] float hookInsideDist;
    [SerializeField] float maxDistance;
    [SerializeField] float minDistance;
    private Collider col;
    [HideInInspector] public Bounds bounds;
    [HideInInspector] public Renderer rend;

    void Awake()
    {
        col = GetComponent<Collider>();
        rend = GetComponent<Renderer>();
        bounds = rend.bounds;
    }

    private void Start()
    {
        PlayerControls.Instance.grippableColliders.Add(this);
    }


    public bool IsGrippable(Transform player, Vector3 point)
    {
        float distSphere = Vector3.Distance(player.position, point);
        return distSphere > minDistance && distSphere < maxDistance;
    }

    public Vector3 GetPreferredGrabPoint(Transform camera, Transform player)
    {
        Ray ray = new Ray();
        ray.origin = camera.position;
        ray.direction = camera.forward;

        if (col.Raycast(ray, out var hit, maxDistance - Vector3.Dot(camera.forward, (player.position - camera.position).normalized)
            * (player.position - camera.position).magnitude))
            return hit.point;
        else 
        {
            Vector3 nearPos = col.ClosestPoint(camera.position + camera.forward * Vector3.Distance(camera.position, transform.position));
            return nearPos + (transform.position - nearPos).normalized * hookInsideDist;
        }
    }
}
