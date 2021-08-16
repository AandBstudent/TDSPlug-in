using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float lookRadius;

    public float health = 100f;

    public Transform target;
    public NavMeshAgent agent;
    public Animator _animator;

    Collider collision;

    private double hitTime = 0.8f;
    public double lastHit = 0;

    public void TakeDamage(float damage)
    {
        health -= damage;

        if(health <= 0)
        {
            Die();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        collision = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(target.position, transform.position);
        Vector3 destination = agent.destination;

        // Check for player
        if (distance <= lookRadius)
        {
            if(agent.isStopped != true)
            {
                //Debug.Log("Player Detected");
                // Walking animation
                _animator.SetBool("isLook", true);
                // Move towards where player was detected
                agent.SetDestination(target.position);

                //Debug.Log(target.position);
                // Get and hold the destination for AI 
            }

            if(Time.time > lastHit + hitTime)
            {
                agent.isStopped = false;
            }
                
        }

        // Go back to being idle
        if (Vector3.Distance(destination, transform.position) <= 2)
        {
            //Debug.Log("transform position: " + transform.position);
            _animator.SetBool("isLook", false);
            
            //agent.SetDestination(destination);
        }

        //Debug.Log("Time: " + Time.time);
       // _animator.SetBool("isShot", false);
    }

    void LateUpdate()
    {
        _animator.SetBool("isShot", false);
    }

    void Die()
    {

        // Destroy(gameObject);
        collision.enabled = false;
        _animator.SetBool("isDead", true);
        lookRadius = 0;
        agent.destination = transform.position;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
