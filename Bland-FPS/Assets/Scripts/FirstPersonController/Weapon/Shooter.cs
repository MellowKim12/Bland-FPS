using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Animator))]
public class Shooter : NetworkBehaviour
{
    [SerializeField]
    private bool AddBulletSpread = false;
    [SerializeField]
    private Vector3 BulletSpreadVariance = new Vector3(0.1f, 0.1f, 0.1f);
    [SerializeField]
    private ParticleSystem ShootingSystem;
    [SerializeField]
    private Transform BulletSpawnPoint;
    [SerializeField]
    private ParticleSystem ImpactParticleSystem;
    [SerializeField]
    private TrailRenderer BulletTrail;
    [SerializeField]
    private float ShootDelay = 0.5f;
    [SerializeField]
    private LayerMask Mask;
    [SerializeField]
    private float BulletSpeed = 100;

    

    private Animator Animator;
    private float LastShootTime;

    private void Awake()
    {
        //Animator = GetComponent<Animator>();
    }

    public void Shoot()
    {
        if (LastShootTime + ShootDelay < Time.time)
        {
            // Use an object pool instead for these! To keep this tutorial focused, we'll skip implementing one.
            // For more details you can see: https://youtu.be/fsDE_mO4RZM or if using Unity 2021+: https://youtu.be/zyzqA_CPz2E

            //Animator.SetBool("IsShooting", true);
            ShootingSystem.Play();
            Vector3 direction = GetDirection();

            if (Physics.Raycast(BulletSpawnPoint.position, direction, out RaycastHit hit, float.MaxValue, Mask))
            {
                TrailRenderer trail = Instantiate(BulletTrail, BulletSpawnPoint.position, Quaternion.identity);
                //trail.GetComponent<NetworkObject>().Spawn();
                Debug.Log("Meow");

                if (hit.collider.gameObject.tag == "Player")
                {
                    Debug.Log("hit a payer and did damage");
                    hit.collider.gameObject.GetComponent<PlayerHealth>().Damage(1);
                }


                StartCoroutine(SpawnTrail(trail, hit.point, hit.normal, true));

                LastShootTime = Time.time;
            }
            // this has been updated to fix a commonly reported problem that you cannot fire if you would not hit anything
            else
            {
                TrailRenderer trail = Instantiate(BulletTrail, BulletSpawnPoint.position, Quaternion.identity);
                

                StartCoroutine(SpawnTrail(trail, BulletSpawnPoint.position + GetDirection() * 100, Vector3.zero, false));

                LastShootTime = Time.time;
            }
        }
    }

    private Vector3 GetDirection()
    {
        Vector3 direction = this.gameObject.transform.forward;
        Debug.Log(direction);

        if (AddBulletSpread)
        {
            direction += new Vector3(
                Random.Range(-BulletSpreadVariance.x, BulletSpreadVariance.x),
                Random.Range(-BulletSpreadVariance.y, BulletSpreadVariance.y),
                Random.Range(-BulletSpreadVariance.z, BulletSpreadVariance.z)
            );

            direction.Normalize();
        }

        return direction;
    }

    private IEnumerator SpawnTrail(TrailRenderer Trail, Vector3 HitPoint, Vector3 HitNormal, bool MadeImpact)
    {
        // This has been updated from the video implementation to fix a commonly raised issue about the bullet trails
        // moving slowly when hitting something close, and not
        Vector3 startPosition = Trail.transform.position;
        float distance = Vector3.Distance(Trail.transform.position, HitPoint);
        float remainingDistance = distance;

        while (remainingDistance > 0)
        {
            Trail.transform.position = Vector3.Lerp(startPosition, HitPoint, 1 - (remainingDistance / distance));

            remainingDistance -= BulletSpeed * Time.deltaTime;

            yield return null;
        }
        //Animator.SetBool("IsShooting", false);
        Trail.transform.position = HitPoint;
        if (MadeImpact)
        {
            InstantiateHitParticleServerRpc(HitPoint, HitNormal);
            //ParticleSystem trail = Instantiate(ImpactParticleSystem, HitPoint, Quaternion.LookRotation(HitNormal));
            //trail.GetComponent<NetworkObject>().Spawn();
        }

        //DestroyParticlesServerRpc(Trail.gameObject);
        Destroy(Trail.gameObject, Trail.time);
    }

    [Rpc(SendTo.Everyone)]
    private void InstantiateHitParticleServerRpc(Vector3 HitPoint, Vector3 HitNormal)
    {
        ParticleSystem trail = Instantiate(ImpactParticleSystem, HitPoint, Quaternion.LookRotation(HitNormal));
        //trail.GetComponent<NetworkObject>().Spawn();
    }

    /*
    [ServerRpc(RequireOwnership = false)]
    private void DestroyParticlesServerRpc()
    {

        if (trail.TryGet(out NetworkObject trailObject))
            Debug.Log("Error deleting Object");
        trailObject.GetComponent<NetworkObject>().Despawn();
        //trail.GetComponent<NetworkObject>().Despawn();
        Destroy(trailObject, trailObject.GetComponent<TrailRenderer>().time);
    }*/

}