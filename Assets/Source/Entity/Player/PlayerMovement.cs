using UnityEngine;

namespace Entity.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] public PlayerLook playerLook;
        [SerializeField] public Camera camera;
        [SerializeField] public Transform groundCheck;

        [Header("Speed")]
        [SerializeField] public float walkSpeed = 7f;
        [SerializeField] public float runSpeed = 10f;
        [SerializeField] public float acceleration = 10f;
        [SerializeField] public float airSpeed = 15f;
        [SerializeField] public float speed = 7f;
        public bool sprint = false;
        

        [Header("Jump")]
        public float jumpHeight = 10f;
        [SerializeField] public int maxJumps = 1;
        [SerializeField] public LayerMask ground;
        private int jumps;
        private bool jump = false;
        

        [Header("FOV")] 
        [SerializeField] public float defaultFOV = 80f;
        [SerializeField] public float speedFOVAddtion = 0.1f;
        [SerializeField] public float FOVtime = 10f;
        private float FOVvelocity = 0f;

        [Header("Velocity")] 
        [SerializeField] public float groundVelocityChange = 1.5f;
        [SerializeField] public float airVelocityChange = 0.3f;
        [SerializeField] public float airDrag = 0.99f;
        
        public Vector2 horizontalInput;
        private Rigidbody rb;
        public bool isGrounded;

        private Vector3 targetVelocity;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void LateUpdate()
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, ground);

            // Determine player speed
            if (isGrounded)
            {
                jumps = maxJumps;
                
                if (sprint)
                {
                    speed = Mathf.Lerp(speed, runSpeed, acceleration * Time.deltaTime);
                }
                else
                {
                    speed = Mathf.Lerp(speed, walkSpeed, acceleration * Time.deltaTime);
                }
            }
            else
            {
                speed = Mathf.Lerp(speed, airSpeed, acceleration * Time.deltaTime);
            }
            
            //Disable sprint
           
            
            targetVelocity = (playerLook.head.right * horizontalInput.x +
                              playerLook.head.forward * horizontalInput.y) * speed;

            if (Vector2.Dot(horizontalInput, Vector2.up) < 0.8)
            {
                sprint = false;
            }
        }

        private void FixedUpdate()
        {
            Drag();
            Move();
            CameraFOV();
        }
        
        // Cool idea; unfortunately a shit one
        private void CameraFOV()
        {

            float targetFOV = Mathf.Clamp(defaultFOV + rb.velocity.magnitude * speedFOVAddtion, defaultFOV,
                defaultFOV + 15);
            
            camera.fieldOfView = Mathf.SmoothDamp(camera.fieldOfView, targetFOV, 
                ref FOVvelocity, FOVtime * Time.fixedDeltaTime);
        }

        private void Move()
        {
            Vector3 velocityChange = targetVelocity - rb.velocity;
            velocityChange.y = 0f;
            
            velocityChange = Vector3.ClampMagnitude(velocityChange, isGrounded ? groundVelocityChange : airVelocityChange);

            rb.AddForce(velocityChange, ForceMode.VelocityChange);
        }

        public void HorizontalInput(Vector2 horizontalInput)
        {
            this.horizontalInput = horizontalInput;
        }

        public void OnJumpPressed()
        {
            if (jumps > 0)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(transform.up * jumpHeight, ForceMode.Impulse);
                jumps--;
            }
            
        }

        public void OnSprintPressed()
        {
            sprint = true;
        }
        
        private void Drag()
        {
            if (!isGrounded && targetVelocity == Vector3.zero)
            {
                rb.velocity = new Vector3(rb.velocity.x * airDrag, rb.velocity.y, rb.velocity.z * airDrag);
            }
        }
    }

}
