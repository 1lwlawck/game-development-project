using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class PlayerController : MonoBehaviour
    {
        public float movingSpeed;
        public float jumpForce;
        private float moveInput;
        private float horizontalInput = 0f;
        private bool jumpPressed = false;

        private bool facingRight = false;
        [HideInInspector]
        public bool deathState = false;

        private bool isGrounded;
        public Transform groundCheck;

        private Rigidbody2D rigidbody;
        private Animator animator;
        private GameManager gameManager;

        void Start()
        {
            rigidbody = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        }

        private void FixedUpdate()
        {
            CheckGround();
        }

        void Update()
        {
            // Kombinasi input dari keyboard dan UI
            float keyboardInput = Input.GetAxis("Horizontal");
            moveInput = Mathf.Abs(keyboardInput) > 0 ? keyboardInput : horizontalInput;

            if (moveInput != 0f)
            {
                Vector3 direction = transform.right * moveInput;
                transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, movingSpeed * Time.deltaTime);
                animator.SetInteger("playerState", 1); // Run
            }
            else
            {
                if (isGrounded) animator.SetInteger("playerState", 0); // Idle
            }

            // Lompat via keyboard atau tombol UI
            if ((Input.GetKeyDown(KeyCode.Space) || jumpPressed) && isGrounded)
            {
                rigidbody.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
                jumpPressed = false;
            }

            if (!isGrounded)
            {
                animator.SetInteger("playerState", 2); // Jump
            }

            // Balik arah karakter
            if (!facingRight && moveInput > 0)
            {
                Flip();
            }
            else if (facingRight && moveInput < 0)
            {
                Flip();
            }
        }

        private void Flip()
        {
            facingRight = !facingRight;
            Vector3 Scaler = transform.localScale;
            Scaler.x *= -1;
            transform.localScale = Scaler;
        }

        private void CheckGround()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.transform.position, 0.2f);
            isGrounded = colliders.Length > 1;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.tag == "Enemy")
            {
                deathState = true;
            }
            else
            {
                deathState = false;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.tag == "Coin")
            {
                gameManager.coinsCounter += 1;
                Destroy(other.gameObject);
            }
        }

        // ----------------------------
        // FUNGSI TAMBAHAN UNTUK UI
        // ----------------------------
        public void MoveLeft(bool isPressed)
        {
            horizontalInput = isPressed ? -1f : 0f;
        }

        public void MoveRight(bool isPressed)
        {
            horizontalInput = isPressed ? 1f : 0f;
        }

        public void Jump()
        {
            if (isGrounded)
                jumpPressed = true;
        }
    }
}
