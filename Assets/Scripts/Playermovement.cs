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

    public List<GameObject> breadcrumbsTrail = new List<GameObject>();

    public Transform playerCamera;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;

    private Vector3 lastBreadcrumbPosition;
    private bool canLook = true;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        health = maxHealth - 20;
        lastBreadcrumbPosition = transform.position;
    }

    private void Update()
    {
        Movement();
        if (canLook) MouseLook();
        CheckBreadcrumbSpawn();
    }

    private void Movement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        controller.Move(move * speed * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void MouseLook()
    {
        if (!canLook) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void CheckBreadcrumbSpawn()
    {
        if (breadcrumbPrefab == null) return;

        float distanceTraveled = Vector3.Distance(transform.position, lastBreadcrumbPosition);

        if (distanceTraveled >= breadcrumbDistance)
        {
            SpawnBreadcrumb();
            lastBreadcrumbPosition = transform.position;
        }
    }

    private void SpawnBreadcrumb()
    {
        Vector3 spawnPos = GetGroundedPosition(transform.position);
        GameObject breadcrumb = Instantiate(breadcrumbPrefab, spawnPos, Quaternion.identity);
        breadcrumbsTrail.Add(breadcrumb);
    }

    /// <summary>
    /// Casts a ray downward to place the breadcrumb on the ground.
    /// </summary>
    private Vector3 GetGroundedPosition(Vector3 origin)
    {
        RaycastHit hit;
        if (Physics.Raycast(origin + Vector3.up * 5f, Vector3.down, out hit, 10f))
        {
            return hit.point;
        }

        // Fallback if nothing is hit
        return origin;
    }

    public void IncreaseHealth(int amount)
    {
        health += amount;
        health = Mathf.Clamp(health, 0, maxHealth);
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
        speed -= amount;
        Debug.Log("Speed Boost Ended");
    }

    public void IncreaseScore(int points)
    {
        energy += points;
        Debug.Log("Energy: " + energy);
    }

    public void SetLookEnabled(bool enabled)
    {
        canLook = enabled;
    }

}