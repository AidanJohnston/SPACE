using UnityEngine;

namespace Entity.Player
{
    public class PlayerLook : MonoBehaviour
    {
        
        [Header("Sensitivity")]
        [SerializeField] private float sensitivityX = 8f;
        [SerializeField] private float sensitivityY = 8f;
        [SerializeField] private float multiplier = 0.01f;
        [SerializeField] private float yClamp = 85f;
        [SerializeField] private float damp = 15f;
        
        [Header("Components")]
        [SerializeField] public Transform head;

        private float yRotation = 0f;
        private float xRotation = 0f;
        private Vector2 mouseInput;
        private Vector3 velocity = Vector3.zero;
        private Quaternion targetRotation;

        private void Awake()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void LateUpdate()
        {
            yRotation += mouseInput.x;
            xRotation -= mouseInput.y;
            xRotation = Mathf.Clamp(xRotation, -yClamp, yClamp); ;

            head.transform.localRotation = Quaternion.Lerp(head.transform.localRotation,
                Quaternion.Euler(xRotation, yRotation, 0f), Time.deltaTime * damp);
        }

        public void ReceiveMouseInput(Vector2 mouseInput)
        {
            this.mouseInput.x = mouseInput.x * sensitivityX * multiplier * Time.deltaTime;
            this.mouseInput.y = mouseInput.y * sensitivityY * multiplier * Time.deltaTime;
        }
    }
}
