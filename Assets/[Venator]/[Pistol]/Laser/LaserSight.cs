using UnityEngine;

public class LaserSight : MonoBehaviour
{
    [Header("Configuración")]
    public float maxDistance = 100f;
    public LayerMask collisionLayers;

    [Header("Referencias")]
    public LineRenderer lineRenderer;
    public GameObject laserDot;

    void Awake()
    {
        if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2; 
    }

    void Update()
    {
        ShootLaser();
    }

    void ShootLaser()
    {
        lineRenderer.SetPosition(0, transform.position);

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance, collisionLayers))
        {
            lineRenderer.SetPosition(1, hit.point);

            if (laserDot != null)
            {
                laserDot.SetActive(true);
                laserDot.transform.position = hit.point + (hit.normal * 0.005f);
                laserDot.transform.up = hit.normal;
            }
        }
        else
        {
            lineRenderer.SetPosition(1, transform.position + (transform.forward * maxDistance));

            if (laserDot != null) laserDot.SetActive(false);
        }
    }
}