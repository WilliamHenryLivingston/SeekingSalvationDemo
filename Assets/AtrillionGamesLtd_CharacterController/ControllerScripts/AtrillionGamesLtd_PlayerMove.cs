using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AtrillionGamesLtd
{

    public class AtrillionGamesLtd_PlayerMove : MonoBehaviour
    {
        private GameObject wherePlayerLastStood; //Used for tracking moving parts and moving platforms
        private ATG_PlayerControls inputActions;

        [SerializeField] private Transform playerCamera;
        [SerializeField] private Transform playerCameraHolder;
        [Space]
        // These are visible in the editor to help debug
        [Header("Debug Fields")]
        [SerializeField] private LayerMask floorlayers;
        [SerializeField] private Vector3 playerVelocity;
        [SerializeField] private Vector3 prevPlayerPos;
        [SerializeField] private Vector3 absolutePlayerVelocity;
        [SerializeField] private Vector2 movementInput;
        [SerializeField] private Vector2 lookMovementInput;
        [Space]
        [Header("Player Conditions")]
        [SerializeField] private bool isGrounded;
        [SerializeField] private float headDistance;
        [SerializeField] private bool isSprinting;
        [SerializeField] private bool isCrouching;
        [SerializeField] private bool isJumping;
        [SerializeField] private bool isSliding;
        [SerializeField] private bool isWallRunning;
        [SerializeField] private bool isJumpDisabled;
        [Space]
        [Header("Player Details")]
        [SerializeField] private float playerRadius = 0.5f;
        [SerializeField] private float playerStandingHeight = 1.5f;
        [SerializeField] private float playerCrouchHeight = 1f;
        [Space]
        [Header("Player Properties")]
        [SerializeField] private float sensitivity = 0.3f;
        [Space]
        [SerializeField] private float maxWalkableSlope = 45.0f;
        [SerializeField] private float airtimeSpeedCap = 50;
        [SerializeField] private float playerSpeed = 5.0f;
        [SerializeField] private float sprintSpeedMultiplier = 2.0f;
        [SerializeField] private float crouchSpeedMultiplier = 0.5f;
        [SerializeField] private float jumpHeight = 10.0f;
        [SerializeField] private float stepHeight = 0.5f;
        [SerializeField] private float stairAndStepClimbRate = 10.0f;
        [SerializeField] private float rotateTowardsGravityRate = 10f;
        [SerializeField] private float gravityValue = 9.81f;
        [SerializeField] private Vector3 gravityDirection = new Vector3(0f,-1f,0f);
        [Space]

        [SerializeField] private float headOffset;
        [Space]
        
        [SerializeField] private float playerAirControl = 1.5f;
        [SerializeField] private float playerAirResistance = 0.01f;
        [SerializeField] private float playerSlideControl = 2.5f;
        [SerializeField] private float playerWallRunControl = 5f;
        [SerializeField] private float playerWallRunFallOff = 2f; // At what rate does the player lose grip with the wall
        [SerializeField] private float playerWallRunJumpPower = 10f;
        [Space]

        private float pointInWallRun = 0f;
        private float groundFriction = 0f;
        private Vector3 slopeNormal;
        private Vector3 slopeDirection;
        private Vector3 slideDirection;
        private Transform lastSurface;

        private float localAirSpeedCap;

        private CapsuleCollider playerBodyCollider;
        private Rigidbody playerBodyRigidBody;

        void Awake()
        {
            inputActions = new ATG_PlayerControls();
        }

        void OnEnable()
        {
            inputActions.Enable();
        }

        void OnDisable()
        {
            inputActions.Disable();
        }

        void setHeadPosition(float headHeight){
            headOffset = headHeight;
        }

        // change gravity direction towards new direction at a specific rate (1 is instant, 0 means not at all)
        // used for either instant swapping or gradual changing for smoother transitions
        public void setPlayerGravityDirection(Vector3 _gravityDirection, float rateOfChange = 1f){
            gravityDirection = Vector3.Lerp(gravityDirection, _gravityDirection.normalized, rateOfChange);
        }

        // Sets the player camera's Field Of View
        public void setPlayerFOV(float _FOV){
            playerCamera.GetComponent<Camera>().fieldOfView = _FOV;
        }

        // Sets the player camera's Sensitivity
        public void setCameraSensitivity(float _Sensitivity){
            sensitivity = _Sensitivity;
        }

        // On game initiation this ensures that all the required components are present and correctly initialised in order for the character controller to work
        void AddScriptsIfMissing(){

            playerBodyCollider = GetComponent<CapsuleCollider>();
            if (playerBodyCollider == null)
            {
                playerBodyCollider = gameObject.AddComponent<CapsuleCollider>();
            }

            playerBodyRigidBody = GetComponent<Rigidbody>();
            if (playerBodyRigidBody == null)
            {
                playerBodyRigidBody = gameObject.AddComponent<Rigidbody>();
            }


            // Create a new PhysicMaterial and set properties so that the player movement is consistent and accurate
            // The physics are calculated internally so we don't want friction to be applied externally by the unity physics system
            PhysicMaterial physMaterial = new PhysicMaterial();
            physMaterial.dynamicFriction = 0f;
            physMaterial.staticFriction = 0f;
            physMaterial.bounciness = 0f;
            physMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
            physMaterial.bounceCombine = PhysicMaterialCombine.Minimum;

            // Assign the material to the collider
            playerBodyCollider.material = physMaterial;

            // Make sure the player body and player head don't roll or use gravity since that will also be calculated internally
            playerBodyRigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            playerBodyRigidBody.useGravity = false;
            playerBodyRigidBody.interpolation = RigidbodyInterpolation.Interpolate;

            if(playerCamera == null){
                playerCamera = Camera.main.transform; // If no camera is assigned it will just use whatever the main scene camera is as the player camera
            }

            // Check if the parent of the camera is the CameraHolder
            playerCameraHolder = playerCamera.parent;
            if (playerCameraHolder == null || playerCameraHolder.name != "CameraHolder") // if it's not the CameraHolder it creates and sets the parent to the CameraHolder
            {
                GameObject playerCameraHolderGameObject = new GameObject("CameraHolder");
                playerCameraHolderGameObject.transform.SetParent(transform);  // Set null as parent
                playerCamera.transform.SetParent(playerCameraHolderGameObject.transform);  // Set null as parent
                playerCameraHolder = playerCameraHolderGameObject.transform;
            }else{
                playerCameraHolder.transform.SetParent(transform);  // Set null as parent
                playerCamera.transform.SetParent(playerCameraHolder);  // Set null as parent
            }

            // Reset the transform of characterHolder
            playerCameraHolder.localPosition = Vector3.zero;
            playerCameraHolder.localRotation = Quaternion.identity;
            playerCameraHolder.localScale = Vector3.one;

            // Reset the transform of characterHolder
            playerCamera.localPosition = Vector3.zero;
            playerCamera.localRotation = Quaternion.identity;
            playerCamera.localScale = Vector3.one;

            // uncomment if you want the player to stat with a default FOV
            //setPlayerFOV(90f); // sets the initial player FOV
        }

        private void Start()
        {
            AddScriptsIfMissing();

            wherePlayerLastStood = new GameObject("wherePlayerLastStood"); // create the player holder (used for tracking moving platforms that the player might be standing on)
            wherePlayerLastStood.transform.SetParent(null);  // Set null as parent

            Cursor.lockState = CursorLockMode.Locked;

            playerBodyCollider.height = playerCrouchHeight;
            playerBodyCollider.radius = playerRadius;

            if(playerStandingHeight-playerCrouchHeight < stepHeight){
                Debug.LogWarning("Step height exceeds player leg length. (playerStandingHeight - playerCrouchHeight) is the leg length");
            }
            stepHeight = Mathf.Min(stepHeight, playerStandingHeight-playerCrouchHeight);

            setHeadPosition(playerStandingHeight);
        }

        void RotateTransformToDirection(Transform _transform, Vector3 targetDirection, float rotationSpeed = 10f)
        {
            // the angle between the player down vector and the expected down vector
            float angleToTarget = Vector3.Angle(-_transform.up, targetDirection);
            if(angleToTarget > 0f){
                // Calculates the direction to rotate towards to ensure the player is alinged with gravity
                Vector3 stepTargetDirection = Vector3.Slerp(-_transform.up, targetDirection, Mathf.Clamp01((rotationSpeed*10f)/(angleToTarget)));
                // Calculate the rotation that aligns the transform that points down with stepTargetDirection
                Quaternion targetRotation = Quaternion.FromToRotation(-_transform.up, stepTargetDirection);

                // uncomment for debugging gravity alignment issues, It shows 2 rays. Blue for where the gravity is pointing and green for where the player's "down" vector is pointing
                //Debug.DrawRay(_transform.position, targetDirection, Color.blue, Time.fixedDeltaTime);
                //Debug.DrawRay(_transform.position, -_transform.up, Color.green, Time.fixedDeltaTime);

                // Apply the rotation to the transform
                transform.rotation = targetRotation * transform.rotation;
            }
        }

        void playerLookAround()
        {
            float mouseX = lookMovementInput.x;
            float mouseY = lookMovementInput.y;

            float yRotation = playerCamera.localEulerAngles.x - (mouseY * sensitivity);
            float xRotation = playerCamera.localEulerAngles.y + (mouseX * sensitivity);

            // Clamp the vertical rotation between -90 and 90 degrees (straight down and straight up)
            if (yRotation > 180) // Because Euler angles wrap around at 360 degrees
                yRotation -= 360;

            yRotation = Mathf.Clamp(yRotation, -89.9f, 89.9f);

            // Apply the rotation
            RotateTransformToDirection(playerCameraHolder, gravityDirection, rotateTowardsGravityRate * Time.deltaTime);

            // Calculate the horizontal (yaw) rotation in the perpendicular plane to gravity (mouseX input)
            float verticalRotation = mouseY * sensitivity;
            float horizontalRotation = mouseX * sensitivity;

            playerCamera.localRotation = Quaternion.Euler(yRotation, xRotation, 0f);

            //Debug.DrawRay(playerCamera.position, playerCamera.forward*10f, Color.blue, 5f);
        }

        void standingOnMovingSurface(Transform Surface, Vector3 position){
            if(lastSurface && Surface){ // Ground To Ground transition
                if(lastSurface != Surface){
                    transform.SetParent(null);
                    wherePlayerLastStood.transform.position = Surface.position;
                    wherePlayerLastStood.transform.rotation = Surface.rotation;
                    transform.SetParent(wherePlayerLastStood.transform);

                    //print("NewFloor " + Surface.name);
                }else{
                    wherePlayerLastStood.transform.position = Surface.position;
                    wherePlayerLastStood.transform.rotation = Surface.rotation;
                    //print("Still on NewFloor " + Surface.name);
                }
            }else{
                if(lastSurface){ // Ground to Air transition
                    localAirSpeedCap = Mathf.Min(absolutePlayerVelocity.magnitude, airtimeSpeedCap);
                    playerVelocity += absolutePlayerVelocity;
                    //print("OffFloor " + absolutePlayerVelocity);
                }
                if(Surface){ // Air to Ground transition
                    transform.SetParent(null);
                    wherePlayerLastStood.transform.position = Surface.position;
                    wherePlayerLastStood.transform.rotation = Surface.rotation;
                    transform.SetParent(wherePlayerLastStood.transform);
                    //print("Air to NewFloor " + Surface.name);
                }
            }
            lastSurface = Surface;
        }

        void whatIsPlayerStoodOn(ref Vector3 slopeDirection, ref Vector3 slideDirection, ref float groundFriction){
            //Debug.DrawRay(playerCamera.position, playerCamera.forward*10f, Color.green, 5f);

            isGrounded = false;
            RaycastHit directlyGrounded;
            Debug.DrawRay(transform.position, gravityDirection * playerStandingHeight, Color.cyan, 5f);
            if (Physics.Raycast(transform.position, gravityDirection, out directlyGrounded, (headOffset/2)+stepHeight+0.01f, floorlayers))
            {
                isGrounded = true;
                standingOnMovingSurface(directlyGrounded.collider.transform, directlyGrounded.point);
                headDistance = ((headOffset/2)+stepHeight) - directlyGrounded.distance;

                float angle = Vector3.Angle(directlyGrounded.normal, -gravityDirection);

                isGrounded = angle < maxWalkableSlope || isGrounded;

                if(angle < maxWalkableSlope){ // If this specific contact point is a grounded point
                    slopeNormal = directlyGrounded.normal;
                    slopeDirection = Vector3.ProjectOnPlane(gravityDirection, directlyGrounded.normal); // slope direction
                    slideDirection = slopeDirection * Vector3.Dot(gravityDirection, slopeDirection);

                    // Get the PhysicMaterial from the hit object
                    Collider hitCollider = directlyGrounded.collider;
                    if (hitCollider != null && hitCollider.material != null)
                    {
                        PhysicMaterial hitMaterial = hitCollider.material;
                        groundFriction = hitMaterial.dynamicFriction;
                    }
                }
            }else{
                standingOnMovingSurface(null, Vector3.zero);
            }
        }

        // Move in the direction the player is facing
        Vector3 getPlayerMoveDirection(Vector3 slopeDirection){
            Vector3 move = playerCamera.forward * movementInput.y + playerCamera.right * movementInput.x;
            if(isGrounded){
                move = Vector3.ProjectOnPlane(move, gravityDirection);
                if(!isSliding){ // move down slopes or stairs rather than falling down them
                    move = Vector3.ProjectOnPlane(move, slopeNormal);
                }
                move = move.normalized; // Normalize to prevent diagonal speed boost (Respects real max speed rather than getting sqrt(2)*max speed as the max speed when moving diagonally)
                pointInWallRun = 0f;
            }else{
                Vector3 flattenedMove = Vector3.ProjectOnPlane(move, gravityDirection).normalized;

                if(isWallRunning){
                    handleWallRunning(ref move, flattenedMove);
                }else
                {
                    pointInWallRun = 0f;
                    move = Vector3.ProjectOnPlane(move, gravityDirection);
                }
            }
            return move;
        }

        void handleWallRunning(ref Vector3 move, Vector3 flattenedMove){
            move = Vector3.ProjectOnPlane(move, slopeNormal); // slope direction
            move = Vector3.Lerp(move, flattenedMove, 0.1f);
            move = move.normalized;
            pointInWallRun += Time.fixedDeltaTime;
        }

        void handleCrouching(ref float playerMovementSpeed){
            if (isCrouching)
            {
                setHeadPosition(playerCrouchHeight); // move the player head down to crouched height
                if (playerVelocity.magnitude > playerSpeed) // is the player crouches while moving faster than the players default walking speed it initiaites a slide
                {
                    isSliding = true;
                }
                else
                {
                    playerMovementSpeed *= crouchSpeedMultiplier;
                    if (playerVelocity.magnitude < playerMovementSpeed) // if the player's slide slows down to being slower than the crouched movement speed it stops sliding
                    {
                        isSliding = false;
                    }
                }
            }
            else
            {
                setHeadPosition(playerStandingHeight); // move the player head back up to standing height
                isSliding = false;
            }
        }

        void handleSprinting(ref float playerMovementSpeed){
            // add sprint speed boost
            if (isSprinting && isGrounded && !isCrouching) // only give sprint speed boost when standing on the ground
            {
                playerMovementSpeed *= sprintSpeedMultiplier;
            }
        }

        void handleMovement(float playerMovementSpeed, Vector3 move){
            // If the player is grounded, apply movement grounded movement
            if (isGrounded)
            {
                if (isSliding) // If player is grounded but is not in full control (i.e. sliding) then their movement is scaled down by the player slide control
                {
                    playerVelocity += move * playerSlideControl * Time.fixedDeltaTime;
                }
                else // If player is actually in full control then their movement is the player input
                {
                    playerVelocity += (move * playerMovementSpeed);
                    playerVelocity /= 2f;
                    Debug.DrawRay(transform.position, move, Color.cyan, 5f);
                }
            }
            else if(isWallRunning){
                float wallRunControl = playerWallRunControl * Mathf.Clamp01(1-pointInWallRun*playerWallRunFallOff) * Mathf.Clamp01(Vector3.Dot(playerVelocity.normalized, move));
                playerVelocity += move * playerMovementSpeed * wallRunControl * Time.fixedDeltaTime;
                playerVelocity += gravityDirection * gravityValue * Time.fixedDeltaTime; // Add gravity
            }
            else{ // If the player isn't touching the ground then their movement is dictated by the air control amount
                playerVelocity += move * playerMovementSpeed * playerAirControl * Time.fixedDeltaTime;
                playerVelocity += gravityDirection * gravityValue * Time.fixedDeltaTime; // Add gravity
                playerVelocity *= 1f - playerAirResistance; // Add Air Resistance
                if(playerVelocity.magnitude > localAirSpeedCap && localAirSpeedCap > 0f) // caps the player to a maximum air velocity
                {
                    float dot = Mathf.Max(Vector3.Dot(playerVelocity, gravityDirection), 0f);
                    Vector3 newVelocity = (playerVelocity.normalized * localAirSpeedCap);
                    Vector3 oldGravitySpeed = (dot * gravityDirection);
                    playerVelocity = new Vector3(
                        Mathf.Abs(newVelocity.x) > Mathf.Abs(oldGravitySpeed.x) ? newVelocity.x : oldGravitySpeed.x,
                        Mathf.Abs(newVelocity.y) > Mathf.Abs(oldGravitySpeed.y) ? newVelocity.y : oldGravitySpeed.y,
                        Mathf.Abs(newVelocity.z) > Mathf.Abs(oldGravitySpeed.z) ? newVelocity.z : oldGravitySpeed.z
                    );
                }
            }
        }

        void handleJumping(){
            // Jumping logic
            if (isJumping && !isJumpDisabled && (isGrounded || isWallRunning) && !isCrouching) // Player jumps if they are on the ground and are not crouching (and jumping is enabled)
            {
                if(isWallRunning){
                    playerVelocity += (slopeNormal + transform.up)/2 * playerWallRunJumpPower; // Apply upward velocity for jumping
                }else{
                    playerVelocity += (slopeNormal + transform.up)/2 * jumpHeight; // Apply upward velocity for jumping
                }
                isJumpDisabled = true;
                isGrounded = false;
            }
            
            if (!isJumping){ // re-enable jumping once the player stops holding down the jump key (used to stop the player from jumping right after vaulting or climbing over ledges)
                isJumpDisabled = false;
            }
        }

        void handleSliding(Vector3 slideDirection, float groundFriction){
            if (isSliding)
            {
                playerVelocity += slideDirection * gravityValue * Time.fixedDeltaTime; // Add force downhill if there is slope (otherwise the slide direction will be Vector3.zero)
                playerVelocity -= playerVelocity * groundFriction * Time.fixedDeltaTime; // Apply some friction to sliding
            }
        }

        void handleStairs()
        {
            if (isGrounded)
            {
                transform.position -= gravityDirection * headDistance * stairAndStepClimbRate * Time.fixedDeltaTime;
            }
        }

        void playerMove()
        {
            playerVelocity = playerBodyRigidBody.velocity;
            absolutePlayerVelocity = (transform.position - prevPlayerPos)/Time.fixedDeltaTime;
            //Debug.DrawRay(playerCameraHolder.position, ((transform.position + headOffset) - playerCameraHolder.position), Color.red, 5f);
            prevPlayerPos = transform.position;
            // ---------------- Determine what is being stood on --------------------
            RotateTransformToDirection(transform, gravityDirection, rotateTowardsGravityRate * Time.fixedDeltaTime); // rotate the character's down transform to point in the direction of gravity

            whatIsPlayerStoodOn(ref slopeDirection, ref slideDirection, ref groundFriction);


            // ----------------------------------------------------------------------   

            // ------------------ Player movement input handling --------------------
            Vector3 move = getPlayerMoveDirection(slopeDirection);

            // ------- Apply movement speed modifiers (sprint, crouch, sliding etc.) --------

            float playerMovementSpeed = playerSpeed;

            handleCrouching(ref playerMovementSpeed);

            handleSprinting(ref playerMovementSpeed);

            handleMovement(playerMovementSpeed, move);

            handleJumping();

            handleSliding(slideDirection, groundFriction);

            // ----------------------------------------------------------------------

            //Debug.DrawRay(transform.position, gravityDirection, Color.cyan, 20f);

            playerBodyRigidBody.velocity = (playerVelocity);

            handleStairs();
            //Debug.DrawRay(transform.position, playerVelocity, Color.green, 20f);

            //isGrounded = false;
            isWallRunning = false;
            slopeDirection = Vector3.zero;
            slideDirection = Vector3.zero;
            slopeNormal = Vector3.zero;
            groundFriction = 0f;
        }

        // This event is triggered when the sphere first collides with another object
        private void OnCollisionStay(Collision collision)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                float angle = Vector3.Angle(contact.normal, -gravityDirection);

                isWallRunning = ((angle > maxWalkableSlope && angle < 120f) || isWallRunning) && !isGrounded; // Less than 120 to prevent the player from wallrunning on ceilings or overhangs

                if(isWallRunning && !isGrounded){ // If this specific contact point is a wallRunning point
                    slopeNormal = contact.normal; // This will get overwritten if one of the later contact points shows it is grounded
                }
            }

        }

        void OnCollisionExit(Collision collisionInfo)
        {
            isWallRunning = false;
        }

        void playerInputs(){
            isSprinting = Input.GetButton("Sprint");
            isCrouching = Input.GetButton("Crouch");
            isJumping = Input.GetButton("Jump");
            movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            lookMovementInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        }

        void GetPlayerInputs()
        {
            movementInput = inputActions.Player.Move.ReadValue<Vector2>();
            lookMovementInput = inputActions.Player.Look.ReadValue<Vector2>();
            isSprinting = inputActions.Player.Sprint.IsPressed();
            isCrouching = inputActions.Player.Crouch.IsPressed();
            isJumping = inputActions.Player.Jump.IsPressed();
        }

        void Update()
        {
            playerLookAround();
            //playerInputs(); // uncomment this to use the old input system and
            GetPlayerInputs(); // comment this out
        }

        void FixedUpdate()
        {
            playerMove();
        }
    }
}