using UnityEngine;

namespace Entity.Player
{
    public class PlayerLook : MonoBehaviour
    {
        
        [Header("Sensitivity")]
        [SerializeField] private float sensitivityX = 8f;
        [SerializeField] private float sensitivityY = 8f;
        [SerializeField] private float xClamp = 89f;
        
        [Header("Components")]
        [SerializeField] public Transform head;
        [SerializeField] public Transform camera;

        private float xRotation = 0f;
        private Vector2 mouseInput;
        private Vector3 velocity = Vector3.zero;
        private Quaternion targetRotation;

        private void Awake()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            xRotation = head.transform.localRotation.x;

        }

        private void LateUpdate()
        {
            float yRotation = mouseInput.x * sensitivityX * Time.deltaTime;
            xRotation -= mouseInput.y * sensitivityY  * Time.deltaTime;
            xRotation = Mathf.Clamp(xRotation, -xClamp, xClamp);
            
            head.transform.rotation *= Quaternion.Euler(yRotation * Vector3.up);
            camera.transform.localRotation = Quaternion.Euler(xRotation * Vector3.right);
        }

        public void ReceiveMouseInput(Vector2 mouseInput)
        {
            this.mouseInput.x = mouseInput.x; 
            this.mouseInput.y = mouseInput.y;
        }
    }
}
