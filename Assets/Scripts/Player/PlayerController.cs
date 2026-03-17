using UnityEngine;
using TrainSurvival.Core;

namespace TrainSurvival.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerStats stats;
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private float mouseSensitivity = 2.2f;
        [SerializeField] private float gravity = -18f;

        private CharacterController characterController;
        private float verticalVelocity;
        private float pitch;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.IsGameOver())
                return;

            HandleLook();
            HandleMove();
        }

        private void HandleLook()
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            transform.Rotate(Vector3.up * mouseX);
            pitch = Mathf.Clamp(pitch - mouseY, -35f, 65f);
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        private void HandleMove()
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            Vector3 move = (transform.right * h + transform.forward * v).normalized;

            if (characterController.isGrounded && verticalVelocity < 0)
                verticalVelocity = -2f;

            verticalVelocity += gravity * Time.deltaTime;
            Vector3 velocity = move * stats.MoveSpeed + Vector3.up * verticalVelocity;
            characterController.Move(velocity * Time.deltaTime);
        }

        public Camera PlayerCamera => playerCamera;
    }
}
