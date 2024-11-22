using System.Collections;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private AudioSource audioSource;

    public int speed = 2;
    public int jumpPower = 5;
    public int sizeOfcoin = 10;
    public float maxTime = 10f;

    private int currentScore;
    private float currentTime;
    private float walkThreshold = 0.3f;
    private bool isWalking = false;
    public bool isTouching = true;
    private bool controlEnabled = true;
    public string idleAnimation = "Idle";
    public string runAnimation = "Run";
    public string jumpAnimation = "Jump";
    public string fallAnimation = "Fall";
    private SoundEffect se;
    private PlayerInfo pInfo;
    private Vector3 move = Vector3.zero;

    private float fallStartY;
    public AudioClip collectSound;
    private Animator anim;
    public TextMeshProUGUI textUI;
    public TextMeshProUGUI textUITimer;
    public JumpState jumpState = JumpState.Grounded;

    void Start()
    {
        anim = GetComponent<Animator>();
        se = SoundEffect.ShareInstance;
        pInfo = GetComponent<PlayerInfo>();
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();

        currentTime = maxTime;
        UpdateScoreUI();
        UpdateTimerUI();
    }

    void Update()
    {
        if (controlEnabled)
        {
            HandleMovement();
            UpdateTimer();
            HandleJump();
        }

        CheckFallOffPlatform();
    }

    private void HandleMovement()
    {
        float inputX = Input.GetAxis("Horizontal");
        move.x = inputX * speed;
        HandleWalkSound(inputX);
        move.z = 0;
        if (jumpState == JumpState.Grounded && Mathf.Abs(inputX) > 0.1f)
        {
            anim.Play(runAnimation);

            if (inputX > 0)
            {
                transform.eulerAngles = new Vector3(0, 90, 0);
            }
            else if (inputX < 0)
            {
                transform.eulerAngles = new Vector3(0, -90, 0);
            }
        }
        else if (jumpState == JumpState.Jumping)
        {
            anim.Play(jumpAnimation);
        }
        else if (jumpState == JumpState.Falling)
        {
            anim.Play(fallAnimation);
        }
        else
        {
            anim.Play(idleAnimation);
        }
        move.y = rb.velocity.y;
        rb.velocity = move;
        HandleJumpState(rb);
    }

    private void HandleWalkSound(float inputX)
    {
        if (Mathf.Abs(inputX) > walkThreshold && Mathf.Abs(rb.velocity.y) < 0.1f && jumpState == JumpState.Grounded)
        {
            if (!isWalking)
            {
                se.PlaySoundEffect("walk");
                isWalking = true;
            }
        }
        else if (isWalking)
        {
            se.StopSoundEffect("walk");
            isWalking = false;
        }
    }

    private void HandleJump()
    {
        if (jumpState == JumpState.Grounded && Input.GetKeyDown(KeyCode.Space))
        {
            isTouching = false;
            se.PlaySoundEffect("jump");
            rb.velocity = new Vector3(rb.velocity.x, jumpPower, 0);
            jumpState = JumpState.Jumping;
        }
        else if (pInfo.doubleJump && jumpState == JumpState.Jumping && Input.GetKeyDown(KeyCode.Space))
        {
            isTouching = false;
            se.PlaySoundEffect("jump");
            rb.velocity = new Vector3(rb.velocity.x, jumpPower, 0);
            pInfo.DoubleJumpState(false);
        }
    }

    private void HandleJumpState(Rigidbody rb)
    {
        if (rb.velocity.y > 0.2f && !isTouching)
        {
            jumpState = JumpState.Jumping;
        }
        else if (rb.velocity.y < -0.2f && !isTouching)
        {
            if (jumpState != JumpState.Falling && !isTouching)
            {
                fallStartY = transform.position.y;
            }
            jumpState = JumpState.Falling;
        }
        else
        {
            jumpState = JumpState.Grounded;
        }

        if (rb.velocity.y == 0)
        {
            jumpState = JumpState.Grounded;
        }
    }

    private void UpdateTimer()
    {
        currentTime -= Time.deltaTime;
        UpdateTimerUI();
        if (currentTime <= 0)
        {
            currentTime = 0;
            PauseGame();
        }
    }

    private void UpdateScoreUI()
    {
        textUI.text = "Score: " + currentScore;
    }

    private void UpdateTimerUI()
    {
        textUITimer.text = "Time: " + currentTime.ToString("F2");
    }

    private void CheckFallOffPlatform()
    {
        if (transform.position.y < -5)
        {
            PauseGame();
            Debug.Log("Ball fell off the platform! Game Paused!");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        isTouching = true;
        if (collision.gameObject.CompareTag("floor") || collision.gameObject.CompareTag("movingPlatform"))
        {
            if (jumpState == JumpState.Falling && Mathf.Abs(transform.position.y - fallStartY) >= 0.01f)
            {
                se.PlaySoundEffect("jumpOnGround");
            }
            jumpState = JumpState.Grounded;

            if (collision.gameObject.CompareTag("movingPlatform"))
            {
                transform.parent = collision.transform;
            }
            jumpState = JumpState.Grounded;
        }
        else if (collision.gameObject.CompareTag("finish"))
        {
            WinGame();
        }
    }

    private void OnCollisionExit(Collision collision)
    {

        if (collision.gameObject.CompareTag("floor") || collision.gameObject.CompareTag("movingPlatform"))
        {
            if (collision.gameObject.CompareTag("movingPlatform"))
            {
                transform.parent = null;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("collect"))
        {
            CollectCoin(other.gameObject);
        }
    }

    private void CollectCoin(GameObject coin)
    {
        currentScore++;
        coin.SetActive(false);
        UpdateScoreUI();

        if (collectSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(collectSound);
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
        Debug.Log("Game Paused!");
    }

    private void WinGame()
    {
        Time.timeScale = 0;
        Debug.Log("Game Win!");
    }
}
