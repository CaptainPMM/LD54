using UnityEngine;

namespace LD54.Player {
    public class PlayerController : MonoBehaviour {

        [SerializeField]
        private float acceleration = 15f;

        [SerializeField]
        private float deceleration = 20f;

        [SerializeField]
        private float maxVelocity = 4;

        [SerializeField]
        private float velocityPower = 2f;

        private Rigidbody2D rbController;
        private Animator animController;
        private Vector2 moveInput;

        private void OnEnable() {

            rbController = GetComponent<Rigidbody2D>();
            animController = GetComponent<Animator>();
            InputManager.onMoveInput += OnMove;
        }

        private void OnDisable() {
            InputManager.onMoveInput -= OnMove;
        }

        private void OnMove(Vector2 rawInput) {
            moveInput = rawInput;

            animController.SetFloat("moveX", moveInput.x);
            animController.SetFloat("moveY", moveInput.y);

            if (rbController.velocity.magnitude < maxVelocity)
                rbController.AddForce(rawInput * acceleration * 200f * Time.deltaTime);     // 200f = factor, so acceleration doesn't need to be 1000 but can be 5 instead
            
        }

        public void SetPlayerSunglasses(bool visible) {
            animController.SetBool("hasSunglasses", visible);
        }

        private void FixedUpdate() {
          /*
            Vector2 targetSpeed = moveInput * maxVelocity;
            Vector2 speedDif = targetSpeed - rbController.velocity;

            float accelerationRate = (targetSpeed.magnitude > 0.01f) ? acceleration : deceleration;

            float movementX = Mathf.Pow(Mathf.Abs(speedDif.x) * accelerationRate, velocityPower) * Mathf.Sign(speedDif.x);
            float movementY = Mathf.Pow(Mathf.Abs(speedDif.y) * accelerationRate, velocityPower) * Mathf.Sign(speedDif.y);

            rbController.AddForce(new Vector2(movementX, movementY));
          */
        }
    }
}
