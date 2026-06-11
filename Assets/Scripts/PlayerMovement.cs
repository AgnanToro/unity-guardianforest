using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float walkSpeed = 5f;
    public float runSpeed = 12f;
    public float gravity = -25f;
    public float jumpHeight = 1.5f;

    // Delay untuk jump diam
    public float standingJumpDelay = 0.35f;

    // Delay untuk jump sambil jalan / walk jump
    public float movingJumpDelay = 0.70f;

    // Delay khusus untuk run jump
    // Biasanya lebih pendek daripada walk jump
    public float runningJumpDelay = 0.25f;

    // Waktu kunci setelah landing
    public float standingLandingLockTime = 0.55f;
    public float movingLandingLockTime = 0.10f;

    // Kecepatan saat ancang-ancang WalkJump / RunJump
    public float jumpPrepareMoveMultiplier = 0.25f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public Animator animator;

    private Vector3 velocity;
    private bool isGrounded;

    private bool isPreparingJump = false;
    private bool isJumping = false;
    private bool hasLeftGround = false;
    private bool canJump = true;

    // Buat nahan input jump sebentar setelah landing
    private bool isLandingLocked = false;

    // Buat nahan movement. Dipakai cuma buat StandingJump landing
    private bool isMovementLocked = false;

    private bool jumpStartedFromWalk = false;
    private bool jumpStartedFromRun = false;

    void Update()
    {
        // Cek tanah
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Reset velocity jatuh saat menyentuh tanah
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Input gerak
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        bool isMoving = move.magnitude > 0.1f;
        bool isHoldingShift = Input.GetKey(KeyCode.LeftShift);

        bool isRunning = isMoving && isHoldingShift;
        bool isWalking = isMoving && !isHoldingShift;

        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        // Kalau sudah pernah lepas tanah lalu mendarat lagi
        if (isGrounded && hasLeftGround && velocity.y <= 0 && !isLandingLocked)
        {
            bool wasMovingJump = jumpStartedFromWalk || jumpStartedFromRun;

            isJumping = false;
            hasLeftGround = false;
            isPreparingJump = false;

            jumpStartedFromWalk = false;
            jumpStartedFromRun = false;

            if (animator != null)
            {
                animator.SetBool("isJumping", false);
            }

            float lockTime = wasMovingJump ? movingLandingLockTime : standingLandingLockTime;

            // Kalau landing dari WalkJump/RunJump, movement jangan dikunci.
            // Kalau landing dari StandingJump, movement dikunci sebentar.
            bool lockMovement = !wasMovingJump;

            StartCoroutine(LandingLockRoutine(lockTime, lockMovement));
        }

        // Movement
        if (!isMovementLocked)
        {
            if (isPreparingJump)
            {
                // Kalau jump dari walk/run, karakter tetap maju sedikit saat ancang-ancang
                if (jumpStartedFromWalk || jumpStartedFromRun)
                {
                    controller.Move(move * currentSpeed * jumpPrepareMoveMultiplier * Time.deltaTime);
                }

                // Kalau StandingJump, karakter diam saat ancang-ancang
            }
            else
            {
                controller.Move(move * currentSpeed * Time.deltaTime);
            }
        }

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded && canJump && !isPreparingJump && !isJumping && !isLandingLocked)
        {
            StartCoroutine(JumpRoutine(isWalking, isRunning));
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Deteksi karakter sudah benar-benar lepas dari tanah
        if (!isGrounded && isJumping)
        {
            hasLeftGround = true;
        }

        // Animator
        if (animator != null)
        {
            if (isJumping)
            {
                animator.SetBool("isWalking", jumpStartedFromWalk);
                animator.SetBool("isRunning", jumpStartedFromRun);
                animator.SetBool("isJumping", true);
            }
            else
            {
                animator.SetBool("isWalking", isMovementLocked ? false : isWalking);
                animator.SetBool("isRunning", isMovementLocked ? false : isRunning);
                animator.SetBool("isJumping", false);
            }
        }
    }

    IEnumerator JumpRoutine(bool wasWalking, bool wasRunning)
    {
        canJump = false;
        isPreparingJump = true;
        isJumping = true;
        hasLeftGround = false;

        jumpStartedFromWalk = wasWalking;
        jumpStartedFromRun = wasRunning;

        if (animator != null)
        {
            animator.SetBool("isJumping", false);

            if (wasRunning)
            {
                animator.Play("RunJump", 0, 0f);
            }
            else if (wasWalking)
            {
                animator.Play("WalkJump", 0, 0f);
            }
            else
            {
                animator.Play("StandingJump", 0, 0f);
            }

            animator.SetBool("isWalking", wasWalking);
            animator.SetBool("isRunning", wasRunning);
            animator.SetBool("isJumping", true);
        }

        // Delay beda untuk tiap jenis jump
        float delay;

        if (wasRunning)
        {
            delay = runningJumpDelay;
        }
        else if (wasWalking)
        {
            delay = movingJumpDelay;
        }
        else
        {
            delay = standingJumpDelay;
        }

        yield return new WaitForSeconds(delay);

        // Baru loncat secara fisik
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        isPreparingJump = false;
    }

    IEnumerator LandingLockRoutine(float lockTime, bool lockMovement)
    {
        isLandingLocked = true;
        isMovementLocked = lockMovement;

        yield return new WaitForSeconds(lockTime);

        isLandingLocked = false;
        isMovementLocked = false;
        canJump = true;
    }
}