using UnityEngine;

namespace Entity.Player
{
    public class PlayerInputManager : MonoBehaviour
    {
        private PlayerControls playerControls;

        [SerializeField] private PlayerMovement _playerMovement;
        [SerializeField] private PlayerLook _playerLook;

        private Vector2 horizontalInput;
        private bool jump;
        private Vector2 mouseInput;

        private void Awake()
        {
            playerControls = new PlayerControls();

            playerControls.Gameplay.Movement.performed += context => horizontalInput = context.ReadValue<Vector2>();
            playerControls.Gameplay.Jump.performed += _ => _playerMovement.OnJumpPressed();
            playerControls.Gameplay.Sprint.performed += _ => _playerMovement.OnSprintPressed();
            playerControls.Gameplay.Crouch.started += _ => _playerMovement.OnCrouchPressed();
            playerControls.Gameplay.Crouch.canceled += _ => _playerMovement.OnCrouchReleased();
            
            playerControls.Gameplay.LookX.performed += context => mouseInput.x = context.ReadValue<float>();
            playerControls.Gameplay.LookY.performed += context => mouseInput.y = context.ReadValue<float>();
        }

        private void Update()
        {
            _playerMovement.HorizontalInput(horizontalInput);
            _playerLook.ReceiveMouseInput(mouseInput);
        }

        private void OnEnable()
        {
            playerControls.Enable();
        }

        private void OnDisable()
        {
            playerControls.Disable();
        }
    }
}
