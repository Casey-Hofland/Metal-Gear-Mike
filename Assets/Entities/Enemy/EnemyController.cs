using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Transform eyes;
    [SerializeField] [Range(0f, 360f)] private float fovAngle = 110f;
    [SerializeField] [Range(0.1f, 2f)] private float roamingSpeed = 0.5f;
    [SerializeField] [Range(0.1f, 2f)] private float chasingSpeed = 1f;
    [SerializeField] private int health = 3;
    [SerializeField] private AudioClip alert;

    [HideInInspector] public bool playerInSight = false;

    private NavMeshAgent nav;
    private Animator anim;
    private GameObject player;

    private Vector3 destination;
    private int currentHealth;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        nav = GetComponent<NavMeshAgent>();
        nav.speed = roamingSpeed;
        anim = GetComponent<Animator>();

        destination = RandomNavmeshLocation();
        nav.SetDestination(destination);

        currentHealth = health;
    }

    private void Update()
    {
        if (currentHealth <= 0)
        {
            nav.isStopped = true;
            return;
        }
        else
        {
            nav.isStopped = false;
        }

        bool tempPlayerInSight = PlayerInSight();
        if (tempPlayerInSight)
        {
            if (tempPlayerInSight != playerInSight)
            {
                AudioSource.PlayClipAtPoint(alert, transform.position, 1f);
            }

            FollowPlayer();
        }
        else if (tempPlayerInSight != playerInSight)
        {
            LosePlayer();
        }
        playerInSight = tempPlayerInSight;

        if (!playerInSight && Vector3.Distance(destination, transform.position) < nav.stoppingDistance)
        {
            destination = RandomNavmeshLocation();
            nav.SetDestination(destination);
        }
    }

    private Vector3 RandomNavmeshLocation()
    {
        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();
        Vector3 point = Vector3.zero;
        NavMeshPath path = new NavMeshPath();

        do
        {
            // Pick the first indice of a random triangle in the nav mesh
            int t = Random.Range(0, navMeshData.indices.Length - 3);

            // Select a random point on it
            point = Vector3.Lerp(navMeshData.vertices[navMeshData.indices[t]], navMeshData.vertices[navMeshData.indices[t + 1]], Random.value);
            Vector3.Lerp(point, navMeshData.vertices[navMeshData.indices[t + 2]], Random.value);
        }
        while (!nav.CalculatePath(point, path));

        return point;
    }

    private bool PlayerInSight()
    {
        Vector3 direction = player.transform.position - transform.position;
        float angle = Vector3.Angle(direction, transform.forward);

        return ((angle < fovAngle * 0.5f) &&
            (Physics.Raycast(eyes.position, direction.normalized, out RaycastHit hitInfo)) &&
            (hitInfo.transform.gameObject == player));
    }

    private void FollowPlayer()
    {
        nav.speed = chasingSpeed;
        nav.SetDestination(player.transform.position);
    }

    private void LosePlayer()
    {
        destination = RandomNavmeshLocation();
        nav.SetDestination(destination);

        nav.speed = roamingSpeed;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject == player)
        {
            player.GetComponent<InputControls>().Die();
        }
    }

    public void Hit()
    {
        currentHealth--;

        if (currentHealth <= 0)
        {
            StopCoroutine(StopMoving());
            StartCoroutine(StopMoving());
        }
    }

    IEnumerator StopMoving()
    {
        yield return new WaitForSeconds(3f);

        currentHealth = health;

        yield return null;
    }
}
