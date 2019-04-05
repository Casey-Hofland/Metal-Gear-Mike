using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Briefcase : MonoBehaviour
{
    [SerializeField] private GameObject[] enableOnPickup;
    [SerializeField] private AudioClip pickupSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<InputControls>().SetBriefcase();
            EnableEnemies();
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            Destroy(gameObject);
        }
    }

    private void EnableEnemies()
    {
        foreach(GameObject enable in enableOnPickup)
        {
            enable.SetActive(true);
        }
    }
}
