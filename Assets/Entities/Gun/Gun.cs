using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private Transform instances;

    [SerializeField] private Transform muzzle;
    [SerializeField] private AudioClip gunShotClip;
    [SerializeField] private GameObject hitDecal;
    [SerializeField] private GameObject sparks;
    [SerializeField] private GameObject blood;

    private new Animation animation;
    private ParticleSystem fireParticle;

    private void Awake()
    {
        animation = GetComponent<Animation>();
        foreach (AnimationState state in animation) state.wrapMode = WrapMode.Once;
        fireParticle = muzzle.GetComponent<ParticleSystem>();
    }

    public void Fire(Ray fireRay)
    {
        if (animation.isPlaying) return;

        animation.Play();
        AudioSource.PlayClipAtPoint(gunShotClip, muzzle.position);
        fireParticle.Play();

        fireRay = new Ray(muzzle.position, muzzle.forward);

        if (Physics.Raycast(fireRay, out RaycastHit hitInfo, float.PositiveInfinity))
        {
            EnemyController enemy = hitInfo.transform.GetComponent<EnemyController>();
            if (enemy)
            {
                enemy.Hit();
                GameObject bloodParticle = Instantiate(blood, hitInfo.point, Quaternion.FromToRotation(Vector3.forward, hitInfo.normal), null);
                Destroy(bloodParticle, 1f);
            }
            else
            {
                GameObject decal = Instantiate(hitDecal, hitInfo.point, Quaternion.FromToRotation(Vector3.up, hitInfo.normal), instances);
                Destroy(decal, 60f);
                GameObject sparksParticle = Instantiate(sparks, hitInfo.point, Quaternion.FromToRotation(Vector3.forward, hitInfo.normal), null);
                Destroy(sparksParticle, 0.2f);
            }
        }
    }
}
