using System.Collections;
using System.Linq.Expressions;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FightingController : NetworkBehaviour
{
    [Header("Player Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float rotateSpeed = 10f;

    private CharacterController charController;
    private Animator animator;

    [Header("Player Fighting")]
    [SerializeField] private float attackCooldown = 0.8f;
    public int attackDamage = 5;
    private string[] attackAnimations = { "Attack1Animation", "Attack2Animation", "Attack3Animation", "Attack4Animation" };
    [SerializeField] private float attackRadius = 2f;
    private float lastAttackTime;
    [SerializeField] private Transform opponent;
    private bool isDead;

    [Header("Player Dodge")]
    [SerializeField] private float dodgeDistance = 3.5f;
    [SerializeField] private float dodgeCooldown = .7f; // Cooldown for dodging
    [SerializeField] private float dodgeDuration = 0.5f; // Duration of dodge movement
    private float lastDodgeTime;
    private bool isDodging = false;

    [Header("Attack Effect")]
    [SerializeField] private ParticleSystem attack1Effect;
    [SerializeField] private ParticleSystem attack2Effect;
    [SerializeField] private ParticleSystem attack3Effect;
    [SerializeField] private ParticleSystem attack4Effect;

    [Header("Audio Effect")]
    [SerializeField] private AudioClip[] hitsound;

    [Header("Health")]

    public int maxHealth = 100;
    public int currentHealth;
    [SerializeField] private GameUI healthBar;

    private void Awake()
    {
        // Lock mouse 
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        charController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        // Assign healthBar by tag (assuming the object with CanvasLayer has this tag)
        GameObject healthBarObj = GameObject.FindGameObjectWithTag("UI");
        if (healthBarObj != null)
        {
            healthBar = healthBarObj.GetComponent<GameUI>();
        }

    }

    private void Update()
    {
        if (isDead) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if (!isDodging)
        {
            HandleMovement();
            HandleAttackInput();
        }
        PerformDodgeFront();
    }

    private void HandleMovement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(-verticalInput, 0f, horizontalInput);

        // Rotate and move if there is movement vector
        if (movement != Vector3.zero)
        {
            RotateCharacter(movement);
        }

        // Walking animation is true as long as there's some directional input (moving or turning the character)
        bool isWalking = horizontalInput != 0 || verticalInput != 0;
        animator.SetBool("Walking", isWalking);

        // Apply movement regardless
        charController.Move(movement * moveSpeed * Time.deltaTime);
    }

    private void RotateCharacter(Vector3 movement)
    {
        Quaternion targetRotation = Quaternion.LookRotation(movement);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }

    private void HandleAttackInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) PerformAttack(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) PerformAttack(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) PerformAttack(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4)) PerformAttack(3);
    }

    private void PerformAttack(int attackIndex)
    {
        if (Time.time - lastAttackTime > attackCooldown)
        {
            animator.Play(attackAnimations[attackIndex]);
            lastAttackTime = Time.time;

            if (Vector3.Distance(transform.position, opponent.position) <= attackRadius)
            {
                opponent.GetComponent<OpponentAi>().StartCoroutine(opponent.GetComponent<OpponentAi>().PlayHitDamageAniamtion(attackDamage));
            }
            
        }
    }

    private void PerformDodgeFront()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isDodging && Time.time - lastDodgeTime > dodgeCooldown)
        {
            animator.Play("DodgeFrontAnimation");
            StartCoroutine(DodgeForwardCoroutine());
            lastDodgeTime = Time.time;
        }
    }

    private IEnumerator DodgeForwardCoroutine()
    {
        isDodging = true;
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + transform.forward * dodgeDistance;

        while (elapsedTime < dodgeDuration)
        {
            Vector3 newPosition = Vector3.Lerp(startPosition, endPosition, elapsedTime / dodgeDuration);
            Vector3 moveDelta = newPosition - transform.position;
            charController.Move(moveDelta);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // To ensure it arrives exactly at end position
        Vector3 finalDelta = endPosition - transform.position;
        charController.Move(finalDelta);

        isDodging = false;
    }

    public IEnumerator PlayHitDamageAniamtion(int takeDamage)
    {
        yield return new WaitForSeconds(0.2f);

        animator.Play("HitDamageAnimation");
        if (hitsound != null && hitsound.Length > 0)
        {
            int randomIndex = Random.Range(0, hitsound.Length);
            AudioSource.PlayClipAtPoint(hitsound[randomIndex], transform.position);

        }

        currentHealth -= takeDamage;
        healthBar.UpdatePlayerHealthSmooth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }


    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        animator.SetBool("Walking", false);
        //

        charController.enabled = false;
        Debug.Log("Player is dead");

        // Optional: trigger round end
        GameManager.Instance?.OnPlayerDefeated();
    }

    public void Attack1Effect()
    {
        attack1Effect.Play();
    }

    public void Attack2Effect()
    {
        attack2Effect.Play();
    }
    public void Attack3Effect()
    {
        attack3Effect.Play();
    }

    public void Attack4Effect()
    {
        attack4Effect.Play();
    }


    public void SearchOpponent()
    {
        GameObject playerObjects = GameObject.FindWithTag("Enemy");
        
        opponent = playerObjects.transform;
        
    }
}

