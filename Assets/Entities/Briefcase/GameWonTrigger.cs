using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWonTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (other.GetComponent<InputControls>().HasBriefcase)
        {
            SceneManagerController.instance.LoadScene(3);
        }
    }
}
