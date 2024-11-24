using System.Collections;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private ObjectPooling op;
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
    public string hitAnimation = "Hit";
    public string DeadAnimation = "Dead";
    private SoundEffect se;
    private PlayerInfo pInfo;
    private Vector3 move = Vector3.zero;

    private float fallStartY;
    public AudioClip collectSound;
    private Animator anim;
    public TextMeshProUGUI textUI;
    public TextMeshProUGUI textUITimer;
    public JumpState jumpState = JumpState.Grounded;
    private bool damaged = false;
    private bool isDeadAnimationPlaying = false;
    void Start()
    {
        op = ObjectPooling.SharedInstance;
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
            HandleJump();
        }
        //ตอนตาย
        if (isDeadAnimationPlaying)
        {
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName(DeadAnimation) && stateInfo.normalizedTime >= 1.0f)
            {
                isDeadAnimationPlaying = false;
                LoseGame();
            }
        }
        UpdateTimer();
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
        else if (pInfo.doubleJump && Input.GetKeyDown(KeyCode.Space))
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
    private void HandleDamaged()
    {

        if (damaged) return;
        pInfo.TakeDamage(1);
        damaged = true;
        controlEnabled = false;
        anim.Play(hitAnimation);
        Vector3 knockbackDirection = transform.forward * -1; // ถอยหลังจากตำแหน่งปัจจุบัน
        knockbackDirection.y = 0.5f; // เพิ่มแรงกระเด้งขึ้นเล็กน้อย
        rb.AddForce(knockbackDirection * 5, ForceMode.Impulse); // เพิ่มแรงกระเด็น
        StartCoroutine(DamageEffectCoroutine());
    }
    private IEnumerator DamageEffectCoroutine()
    {
        float blinkDuration = 2f;
        float blinkInterval = 0.1f;
        float elapsedTime = 0f;

        // ดึง GameObject ลูกทั้งหมดที่มี Renderer
        Transform[] childTransforms = GetComponentsInChildren<Transform>();

        // กระพริบตัวละครโดยปิด/เปิด GameObject ที่มี Renderer
        while (elapsedTime < blinkDuration)
        {
            SetChildrenActive(childTransforms, false); // ซ่อน
            yield return new WaitForSeconds(blinkInterval);
            SetChildrenActive(childTransforms, true); // แสดง
            yield return new WaitForSeconds(blinkInterval);
            elapsedTime += blinkInterval * 2;
        }

        // จบการกระพริบ: แสดงตัวละครทั้งหมด
        SetChildrenActive(childTransforms, true);

        // เปิดการควบคุมใหม่
        if (pInfo.health > 0)
        {
            damaged = false;
            controlEnabled = true;
        }
        else
        {
            OnDead();
        }


    }

    private void SetChildrenActive(Transform[] childTransforms, bool isActive)
    {
        foreach (Transform child in childTransforms)
        {
            if (child != transform)
            {
                child.gameObject.SetActive(isActive);
            }
        }
    }
    private void OnDead()
    {
        anim.Play(DeadAnimation);
        isDeadAnimationPlaying = true;
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
        //โดนกับดัก
        if (collision.gameObject.CompareTag("trap"))
        {
            HandleDamaged();
        }
        //water 
        else if (collision.gameObject.CompareTag("water"))
        {
            LoseGame();

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
            else
            {
                isTouching = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("collect"))
        {
            CollectCoin(other.gameObject);
        }
        if (other.gameObject.CompareTag("wing"))
        {
            CollectWing(other.gameObject);

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
    private void CollectWing(GameObject wing)
    {
        ObjectController wingComponent = wing.GetComponent<ObjectController>();
        pInfo.DoubleJumpState(true);
        wing.SetActive(false);
        op.ResetObject(wing, "powerUp", wingComponent.index);


    }

    private void PauseGame()
    {
        Time.timeScale = 0;
        Debug.Log("Game Paused!");
    }

    private void LoseGame()
    {
        Time.timeScale = 0;
        Debug.Log("You Lose!");
    }

    private void WinGame()
    {
        Time.timeScale = 0;
        Debug.Log("Game Win!");
    }
}
