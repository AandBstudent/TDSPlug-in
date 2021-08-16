using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float _speed = 5f;

    public CharacterController controller;

    // Sounds
    public AudioClip shootSound;
    public AudioClip moveSound;
    private AudioSource audioSource;
    private AudioSource audioSource1;
    private float volLowRange = .45f;
    private float volHighRange = .55f;

    Animator _animator;
    float velocityZ;
    float velocityX;

    public Transform firePoint;
    public Transform mouseTarget;
    public GameObject bulletPrefab;
    public GameObject muzzleFlash;

    public float bulletForce = 20f;
    public float fireRate = 3f;

    private double lastShot = 0;

    void Awake()
    {
        // Gun Sounds
        audioSource = GetComponent<AudioSource>();
        audioSource1 = AddAudio(false, false, 1f);
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        FaceTarget();
        
        // Reading the Input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        /* Convert input into vector */
        Vector3 movement = new Vector3(horizontal, 0f, vertical).normalized;

        // Moving
        if (movement.magnitude > 0)
        {
            // Play step Sounds
            if (!audioSource1.isPlaying)
            {
                audioSource1.pitch = Random.Range(.9f, 1.1f);
                audioSource1.clip = moveSound;
                audioSource1.Play();
            }
            
            FaceTarget();
           // float targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg;

            //transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

            controller.Move(movement * _speed * Time.deltaTime);
        }

        // Shooting
        if (Input.GetButtonDown("Fire1"))
        {
            FaceTarget();
            Shoot();
        } else _animator.SetLayerWeight(1, Mathf.Lerp(_animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f)); // Go back to idle

        // Animating
        float velocityZ = Vector3.Dot(movement.normalized, transform.forward);
        float velocityX = Vector3.Dot(movement.normalized, transform.right);

        /* Set variables in animator to values of inputs */
        /* 3rd and 4th argument of SetFloat() allow animations to transition smoothly */
        _animator.SetFloat("VelocityZ", velocityZ, 0.1f, Time.deltaTime);
        _animator.SetFloat("VelocityX", velocityX, 0.1f, Time.deltaTime);
    }

    void FaceTarget()
    {
        // Get direction of target
        Vector3 direction = (mouseTarget.position - transform.position).normalized;

        // Setup rotation in order to face target
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));

        // Rotate towards target
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        //Debug
        //Debug.Log("Angle Rotation: " + Quaternion.Angle(transform.rotation, lookRotation));

        // Check the angle between the player direction and lookRotation(mouse position)
        float angleDiff = Quaternion.Angle(transform.rotation, lookRotation);

        // Check the speed of the player to determine if the players feet should move
        if (velocityX == 0 && velocityZ == 0 && angleDiff >= 10)
        {
            float velocityZ = 1;
            float velocityX = 1;
            _animator.SetFloat("VelocityZ", velocityZ, 0.1f, Time.deltaTime);
            _animator.SetFloat("VelocityX", velocityX, 0.1f, Time.deltaTime);
        }
    }

    void LateUpdate()
    {
        // Cancel Shooting Animation
       // _animator.SetBool("isShoot", false);
    }

    void Shoot()
    {
        if(Time.time > fireRate + lastShot)
        {
            // Gun Sound Pitch
            audioSource.pitch = Random.Range(volLowRange, volHighRange);
       
            //Muzzle Flash
            GameObject effect = Instantiate(muzzleFlash, firePoint.position, Quaternion.identity);
            Destroy(effect, .1f);

            // Play Gun Sounds
            audioSource.PlayOneShot(shootSound, 1F);

            // Shooting animation
            _animator.SetLayerWeight(1, Mathf.Lerp(_animator.GetLayerWeight(1), 100f, Time.deltaTime * 100f));
           // _animator.SetBool("isShoot", true);

            // Creates bullet
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            // Store bullet object
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            //Vector3 shootOffset = new Vector3(Random.Range(-.75f,.75f), 0f, Random.Range(-.75f,.75f));
            // Create spread so bullets do not shoot in the same place
            float spreadSize = 5f;
            float shotWidth = Random.Range(-.5f, .5f) * spreadSize;
            float shotHeight = Random.Range(-.5f, .5f) * spreadSize;

            // Move bullet that was created
            rb.AddForce(firePoint.forward * bulletForce + firePoint.right * shotWidth + firePoint.up * shotHeight, ForceMode.Impulse);
            //rb.MovePosition(mouseTarget.position + shootOffset * Time.deltaTime * .01f);
            // Save last time player shot
            lastShot = Time.time;
        }
       
    }

    public AudioSource AddAudio(bool loop, bool playAwake, float vol)
    {
        AudioSource newAudio = gameObject.AddComponent<AudioSource>();
        newAudio.loop = loop;
        newAudio.playOnAwake = playAwake;
        newAudio.volume = vol;

        return newAudio;
    }
}
