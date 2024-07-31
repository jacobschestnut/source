using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;

public class EnemyMovementController : MonoBehaviour
{
    public bool CanMove { get; private set; } = true;
    public bool CanSeePlayer { get; private set; } = true;

    public FieldOfView fieldOfView;

    [Header("Functional Parameters")]
    [SerializeField] private float minDistance = 1.0f;

    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 3.0f;

    [Header("Look Parameters")]
    [SerializeField, Range(1,10)] private float lookSpeed = 5.0f;

    private GameObject player;
    private Transform target;
    private Rigidbody rb;

    // Awake is called before Start()
    void Awake()
    {   
        player = GameObject.FindWithTag("Player");
        rb = GetComponent<Rigidbody>();

        fieldOfView = GetComponent<FieldOfView>();
    }

    // Update is called once per frame
    void Update()
    {
        target = player.transform;

        CanSeePlayer = fieldOfView.canSeePlayer;

        if(CanSeePlayer)
        { 
            HandleLookAt();

            if(CanMove)
            {
                HandleMovement();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject == player)
        {
           rb.isKinematic = true; 
        }
    }

    private void OnCollisionExit()
    {
        rb.isKinematic = false; 
    }

    private void HandleLookAt()
    {   
        if(target != null)
        {
            Vector3 relativePos = target.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(relativePos);

            Quaternion current = transform.localRotation;

            transform.localRotation = Quaternion.Slerp(current, rotation, Time.deltaTime * lookSpeed);
        }
    }

    private void HandleMovement()
    {
        float distance = Vector3.Distance(transform.position, target.position);

        if(distance > minDistance)
        {
            transform.position += transform.forward * walkSpeed * Time.deltaTime;
        }
    }
}
