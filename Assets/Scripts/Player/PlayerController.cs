using System.Collections;
using UnityEngine;
using LD54.Game;

namespace LD54.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody2D _rb;
        public Rigidbody2D RB => _rb;

        [SerializeField]
        private CircleCollider2D _col;

        [SerializeField]
        private LayerMask _playerFloeLayer;

        [SerializeField]
        private float speed = 3f;

        [field: SerializeField]
        public float Weight { get; private set; } = 1f;

        [field: SerializeField] public bool IsDrowning { get; private set; }

        private int _sunglassesCount;
        public int SunglassesCount
        {
            get => _sunglassesCount;
            set
            {
                _sunglassesCount = value;
                animController.SetFloat("hasSunglasses", value > 0 ? 1.0f : 0.0f);
            }
        }

        public bool HasSunglasses { get => _sunglassesCount > 0; }

        private Vector2 moveInput;
        public Vector2 MoveInput { get { return moveInput; } }

        private (bool r, bool l, bool u, bool d) _movementRestrictions = new();

        private Animator animController;

        private void Start()
        {
            animController = GetComponent<Animator>();
            GameManager.Instance.onGameStarted += AttachInput;
            GameManager.Instance.onGameEnded += DetachInput;
        }

        private void OnDestroy()
        {
            GameManager.Instance.onGameStarted -= AttachInput;
            GameManager.Instance.onGameEnded -= DetachInput;
        }

        private void AttachInput()
        {
            InputManager.onMoveInput += OnMove;
        }

        private void DetachInput(bool win)
        {
            InputManager.onMoveInput -= OnMove;
        }

        private void Update()
        {
            if (IsDrowning) return;

            _movementRestrictions.r = !Physics2D.Raycast((Vector2)_col.bounds.center + Vector2.right * _col.radius, Vector2.right, 0.001f, _playerFloeLayer).collider;
            _movementRestrictions.l = !Physics2D.Raycast((Vector2)_col.bounds.center + Vector2.left * _col.radius, Vector2.left, 0.001f, _playerFloeLayer).collider;
            _movementRestrictions.u = !Physics2D.Raycast((Vector2)_col.bounds.center + Vector2.up * _col.radius, Vector2.up, 0.001f, _playerFloeLayer).collider;
            _movementRestrictions.d = !Physics2D.Raycast((Vector2)_col.bounds.center + Vector2.down * _col.radius, Vector2.down, 0.001f, _playerFloeLayer).collider;

            if (_movementRestrictions.r && _movementRestrictions.l && _movementRestrictions.u && _movementRestrictions.d) Drown();
        }

        private void OnMove(Vector2 rawInput)
        {
            if (IsDrowning) return;

            moveInput = rawInput;

            animController.SetFloat("moveX", moveInput.x);
            animController.SetFloat("moveY", moveInput.y);

            /*
            if (rbController.velocity.magnitude < maxVelocity)
                rbController.AddForce(rawInput * acceleration * 200f * Time.deltaTime);     // 200f = factor, so acceleration doesn't need to be 1000 but can be 5 instead
            */

            Vector3 movement = new Vector3(moveInput.x, moveInput.y, 0f).normalized * speed * Time.deltaTime;

            // Check floe bounds
            if (_movementRestrictions.r) movement.x = Mathf.Min(movement.x, 0f);
            if (_movementRestrictions.l) movement.x = Mathf.Max(movement.x, 0f);
            if (_movementRestrictions.u) movement.y = Mathf.Min(movement.y, 0f);
            if (_movementRestrictions.d) movement.y = Mathf.Max(movement.y, 0f);

            transform.position += movement;
        }

        [ContextMenu("Drown")]
        private void Drown()
        {
            if (IsDrowning) return;
            IsDrowning = true;
            GameManager.Instance.EndGame(false);

            Animator anim = GetComponent<Animator>();
            anim.SetBool("drowning", true);

            StartCoroutine(DrowningWavesRoutine());
        }

        private IEnumerator DrowningWavesRoutine()
        {
            while (IsDrowning)
            {
                GameManager.Instance.Ocean.CreateWave(transform.position + new Vector3(-1f, 0.5f, 0f), Random.Range(0f, 0.1f), Random.Range(2f, 6f), Random.Range(1f, 3f), 1f, Ocean.Ocean.Shape.Circular);
                yield return new WaitForSeconds(Random.Range(0.1f, 0.2f));
            }
        }
    }
}
