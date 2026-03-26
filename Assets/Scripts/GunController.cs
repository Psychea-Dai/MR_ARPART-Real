using UnityEngine;
using System.Collections;

public class GunController : MonoBehaviour
{
    [Header("References")]
    public Transform muzzlePoint;
    public GameObject bulletTrailPrefab; // visual only, no collider
    
    [Header("Settings")]
    public float fireRate = 0.25f;
    public int maxAmmo = 8;
    public float range = 100f;
    public OVRInput.Controller controller = OVRInput.Controller.RTouch;
    public LayerMask enemylayer;

    private float nextFireTime;
    private int currentAmmo;
    private bool isReloading;

    void Start() { currentAmmo = maxAmmo; }

    void Update()
    {
        bool trigger = OVRInput.Get(
            OVRInput.Button.PrimaryIndexTrigger, controller);

        if (trigger && Time.time >= nextFireTime 
            && currentAmmo > 0 && !isReloading)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }

        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, controller))
            if (!isReloading) StartCoroutine(Reload());
    }

    void Fire()
    {
        currentAmmo--;
        
        // Raycast from muzzle forward
        RaycastHit hit;
        if (Physics.Raycast(muzzlePoint.position, muzzlePoint.forward, out hit, range, enemylayer))
        {
            Debug.Log("Hit: " + hit.collider.gameObject.name);
            Debug.Log("Hit: " + hit.collider.gameObject.name);
            Debug.Log("Has ShootingTarget: " + (hit.collider.GetComponent<ShootingTarget>() != null));
            
            ShootingTarget target = hit.collider.GetComponent<ShootingTarget>();
            if (target != null) target.TakeHit();

            // Spawn trail to hit point
            if (bulletTrailPrefab != null)
                StartCoroutine(SpawnTrail(muzzlePoint.position, hit.point));
        }
        else
        {
            // Nothing hit — trail goes into distance
            if (bulletTrailPrefab != null)
                StartCoroutine(SpawnTrail(muzzlePoint.position, 
                    muzzlePoint.position + muzzlePoint.forward * range));
        }

        // Haptic
        OVRInput.SetControllerVibration(0.4f, 0.4f, controller);
        Invoke(nameof(StopHaptics), 0.08f);
    }

    IEnumerator SpawnTrail(Vector3 from, Vector3 to)
    {
        GameObject trail = Instantiate(bulletTrailPrefab, from, Quaternion.identity);
        LineRenderer lr = trail.GetComponent<LineRenderer>();
        if (lr != null)
        {
            lr.SetPosition(0, from);
            lr.SetPosition(1, to);
        }
        yield return new WaitForSeconds(0.1f);
        Destroy(trail);
    }

    IEnumerator Reload()
    {
        isReloading = true;
        OVRInput.SetControllerVibration(0.2f, 0.2f, controller);
        yield return new WaitForSeconds(1.5f);
        OVRInput.SetControllerVibration(0f, 0f, controller);
        currentAmmo = maxAmmo;
        isReloading = false;
    }

    void StopHaptics() =>
        OVRInput.SetControllerVibration(0f, 0f, controller);
}