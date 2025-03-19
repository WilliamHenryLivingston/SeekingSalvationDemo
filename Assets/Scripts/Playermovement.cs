using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float mouseSensitivity = 100f;

    [Header("Player Stats")]

    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int health;
    [SerializeField] private float speed = 2f;
    [SerializeField] private int energy;

    [Header("Breadcrumb Settings")]
    [SerializeField] private GameObject breadcrumbPrefab;
    [SerializeField] private float breadcrumbDistance = 10f;


  public Transform playerCamera;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;

    private Vector3 lastBreadcrumbPosition;

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen and hide it
        health = maxHealth - 20;

        lastBreadcrumbPosition = transform.position;
    }

    private void Update()
    {
        Movement();
        MouseLook();
        CheckBreadcrumbSpawn();
    }

    private void Movement()
    {
        // Get input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Move player
        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        controller.Move(move * speed * Time.deltaTime);

        // Apply gravity
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Jump
        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void MouseLook()
    {
        //mouse Input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        //Adjust xRotation to rotate the camera down and up
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Rotate the camera and player
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void CheckBreadcrumbSpawn()
    {
        if(breadcrumbPrefab == null) return; //Saftey valve 

        float distanceTraveled = Vector3.Distance(transform.position, lastBreadcrumbPosition);

        if (distanceTraveled >= breadcrumbDistance)
        {
            SpawnBreadcrumb();
            lastBreadcrumbPosition = transform.position; //Update spawn position
        }

    }

    private void SpawnBreadcrumb()
    {
        Instantiate(breadcrumbPrefab, transform.position, Quaternion.identity);
    }
    
    
    //Pickup Handling Methods
    public void IncreaseHealth(int amount) 
    {
        health += amount;
        health = Mathf.Clamp(health, 0, maxHealth); // Prevent overhealing
        Debug.Log("Health: " + health);
    }

    public void IncreaseSpeed(float amount, float duration)
    {
        StartCoroutine(SpeedBoost(amount, duration));
    }

    private IEnumerator SpeedBoost(float amount, float duration)
    {
        speed += amount;
        Debug.Log("Speed Boost Activated!");
        yield return new WaitForSeconds(duration);
        speed -= amount; // Reset speed after duration
        Debug.Log("Speed Boost Ended");
    }

    public void IncreaseScore(int points)
    {
        energy += points;
        Debug.Log("Energy: " + energy);
    }
}

