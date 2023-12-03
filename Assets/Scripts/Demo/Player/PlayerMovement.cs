using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Processors;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float dashingPower = 24f;
    [SerializeField] private float dashingTime = 0.2f;
    [SerializeField] private float dashingCooldown = 1f;
    [SerializeField] private float decayRate = 0.1f;
    [SerializeField] private float gameOverScreenDelay = 0.5f;
    [SerializeField] private Behaviour playerInput;

    private Animator anim;
    private Rigidbody2D body;
    private BoxCollider2D boxCollider;
    public GameObject interactIcon;
    public HealthBar healthBar;
    public GameOverScreen gameOverScreen;
    public PlayerRespawn playerRespawn;
    public ParticleSystem dust;
    public ParticleSystem jumpDust;
    public ParticleSystem dashDust;

    private float horizontalInput;
    private bool canDash = true;
    private bool isDashing;
    private bool isInteracting = false;
    private bool dead = false;
    private bool pause = false;
    public int maxHealth = 100;
    public int currentHealth;

    private Vector2 boxSize = new Vector2(0.1f, 1f);

    private static PlayerMovement instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        playerRespawn.SetStartingPoint(transform.position);
        interactIcon.SetActive(false);
        EnablePlayerMovement();
    }

    private void Start()
    {
        currentHealth = maxHealth; 
        healthBar.SetMaxHealth(maxHealth);
    }

    void Update()
    {
        if (dead || pause)
        {
            return;
        }

        anim.SetBool("dash", isDashing);
        if (isDashing)
        {
            anim.SetBool("grounded", true);
            return;
        }

        body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
        
        FlipPlayer();

        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded());
    }

    void FlipPlayer() {
        Vector3 localScale = transform.localScale;
        if (horizontalInput > 0.01f)
        {
            transform.localScale = new Vector3(Mathf.Abs(localScale.x), localScale.y, localScale.z);
        }
        else if (horizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(-Mathf.Abs(localScale.x), localScale.y, localScale.z);
        }
    }

    public void Jump(InputAction.CallbackContext context) {
        if (context.performed && isGrounded() && !dead) {
            body.velocity = new Vector2(body.velocity.x, jumpPower);
            Vector2 dustPosition = new Vector2(transform.position.x, transform.position.y - 1f);
            ParticleSystem _jumpDust = Instantiate(jumpDust, dustPosition, transform.rotation);
            _jumpDust.Play();
            StartCoroutine(DestroyAfterDelay(_jumpDust, 1f));
        } else if (context.canceled && body.velocity.y > 0f) {
            body.velocity = new Vector2(body.velocity.x, body.velocity.y / 2f);
        }
    }

    private IEnumerator DestroyAfterDelay(ParticleSystem effect, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(effect.gameObject);
    }

    public void Move(InputAction.CallbackContext context) {
        if (context.performed)
        {
            dust.Play();
        }
        else {
            dust.Stop();
        }
        horizontalInput = context.action.ReadValue<Vector2>().x;
    }

    public void Dash(InputAction.CallbackContext context) {
        if (context.performed && canDash && !dead) { 
            StartCoroutine(Dash());
        }
    }

    public void TakeDamage(int damage) {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);


        healthBar.SetHealth(currentHealth);
        if (currentHealth > 0) {
            anim.SetTrigger("hurt");
        } else {
            if (!dead) {
                anim.SetTrigger("die");
                Invoke("DisablePlayerMovement", gameOverScreenDelay);
                Invoke("GameOver", gameOverScreenDelay);
                dead = true;
            }
        }
    }

    public void GameOver() {
        gameOverScreen.Show();
    }

    public void Respawn() {
        EnablePlayerMovement();
        AddHealth(maxHealth);
        playerRespawn.Respawn();
        anim.ResetTrigger("die");
        anim.Play("Idle");
        dead = false;
    }

    public void AddHealth(int health)
    {
        currentHealth = Mathf.Clamp(currentHealth + health, 0, maxHealth);
        healthBar.SetHealth(currentHealth);
    }

    IEnumerator Dash() {
        canDash = false;
        isDashing = true;

        dashDust.Play();

        float originalGravity = body.gravityScale;
        body.gravityScale = 0f;

        float currentSpeed = dashingPower;

        float elapsedTime = 0f;
        while (elapsedTime < dashingTime) {
            body.velocity = new Vector2(transform.localScale.x * currentSpeed, 0f);
            currentSpeed *= Mathf.Exp(-decayRate * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        dashDust.Stop();

        body.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    public void Interaction(InputAction.CallbackContext context) {
        if (context.performed && !isInteracting) {
            CheckInteraction();
        }
    }

    bool isGrounded() {
        // Get the current size and center from the boxCollider
        Vector2 currentSize = boxCollider.bounds.size;
        Vector2 currentCenter = boxCollider.bounds.center;
        
        // Adjust the size and center for the foot area
        float footHeight = currentSize.y / 100; // making the box height a quarter of the full height
        float footWidth = currentSize.x;
        Vector2 footSize = new Vector2(footWidth, footHeight);
        
        // Adjust the center to move it down towards the feet
        Vector2 footCenter = new Vector2(currentCenter.x, currentCenter.y - currentSize.y / 2 + footHeight / 2);
        
        // Perform the BoxCast
        RaycastHit2D raycastHit = Physics2D.BoxCast(footCenter, footSize, 0, Vector2.down, 0.1f, groundLayer);

        return raycastHit.collider != null;
    }

    public bool canAttack() {
        return !isDashing;
    }

    public void OpenInteractableIcon() { 
        interactIcon.SetActive(true);
    }

    public void CloseInteractableIcon() { 
        interactIcon.SetActive(false);
    }

    private void CheckInteraction() {
        isInteracting = true;

        RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, boxSize, 0, Vector2.zero);

        if (hits.Length > 0)
        {
            foreach (RaycastHit2D rc in hits)
            {
                if (rc.transform.GetComponent<Interactable>())
                {
                    rc.transform.GetComponent<Interactable>().Interact();
                    isInteracting = false;
                    return;
                }
            }
        }

        isInteracting = false;
    }

    public void DisablePlayerMovement()
    {
        pause = true;
        playerInput.enabled = false;
        anim.enabled = false;
        body.bodyType = RigidbodyType2D.Static;
    }

    public void EnablePlayerMovement()
    {
        pause = false;
        playerInput.enabled = true;
        anim.enabled = true;
        body.bodyType = RigidbodyType2D.Dynamic;
    }
}
