using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentAi : MonoBehaviour
{
    [Header("Opponent Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float rotateSpeed = 10f;

    [SerializeField] private CharacterController charController;
    [SerializeField] private Animator animator;

    [Header("Opponent Fighting")]
    [SerializeField] private float attackCooldown = 0.8f;
    [SerializeField] private int attackDamage = 5;
    [SerializeField] private int attackCount = 0;
    [SerializeField] private int randomNumber;
    [SerializeField] private float attackRadius = 2f;
    [SerializeField] private FightingController fightingControllers;
    [SerializeField] private Transform players;
    [SerializeField] private bool isTakingDamage;
    private string[] attackAnimations = { "Attack1Animation", "Attack2Animation", "Attack3Animation", "Attack4Animation" };
    private float lastAttackTime;
    private bool isDead;


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
        currentHealth = maxHealth;
        createRandomNumber();

        // Assign healthBar by tag (assuming the object with CanvasLayer has this tag)
        GameObject healthBarObj = GameObject.FindGameObjectWithTag("UI");
        healthBar = healthBarObj.GetComponent<GameUI>();

    }



    private void Update()
    {
        if (isDead) return;
        if (attackCount == randomNumber)
        {
            attackCount = 0;
            createRandomNumber();
        }
      
        if (players.gameObject.activeSelf && Vector3.Distance(transform.position, players.position) <= attackRadius)
        {
            animator.SetBool("Walking", false);

            if (Time.time - lastAttackTime > attackCooldown)
            {
                int randomAttackIndex = Random.Range(0, attackAnimations.Length);

                if (!isTakingDamage)
                {
                    PerformAttack(randomAttackIndex);
                }

                    //Play hit/damage animation on the player
                fightingControllers.StartCoroutine(fightingControllers.PlayHitDamageAniamtion(attackDamage));
                }
            }
            else
            {
                if (players.gameObject.activeSelf)
                {
                    Vector3 direction = (players.position - transform.position).normalized;
                    charController.Move(direction * moveSpeed * Time.deltaTime);

                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

                    animator.SetBool("Walking", true);
                }

            }

        
    }



    private void createRandomNumber()
    {
        randomNumber = Random.Range(1, 5);
    }


    private void PerformAttack(int attackIndex)
    {
        animator.Play(attackAnimations[attackIndex]);

        lastAttackTime = Time.time;
        // Check for enemy to damage enemy (implement your logic here)

    }




    public IEnumerator PlayHitDamageAniamtion(int takeDamage)
    {
        yield return new WaitForSeconds(0.2f);

        animator.Play("HitDamageAnimation");

     
        int randomIndex = Random.Range(0, hitsound.Length);
        AudioSource.PlayClipAtPoint(hitsound[randomIndex], transform.position);


        currentHealth -= takeDamage;
        healthBar.UpdateOpponentHealthSmooth(currentHealth);

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
        charController.enabled = false;

        // Optional: trigger round end
        GameManager.Instance?.OnOpponentDefeated();
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

    public void SearchPlayer()
    {
        GameObject playerObjects = GameObject.FindWithTag("Player");
     
        players = playerObjects.transform;
        fightingControllers= playerObjects.GetComponent<FightingController>();
        
    }
}