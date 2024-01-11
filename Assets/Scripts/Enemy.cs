using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private Player player;

    [SerializeField] private enum TypeEnemy { Melle, Range };
    [SerializeField] private TypeEnemy typeEnemy;

    [SerializeField] private enum TypeGun { None, M4A1, Vector, Pistol };
    [SerializeField] private TypeGun typeGun;

    [Header("Objects")]
    [SerializeField] private GameObject shield;
    [SerializeField] private GameObject hands;
    [SerializeField] private SpriteRenderer gun;
    [SerializeField] private GameObject melleAttackPos;
    [SerializeField] private GameObject firePointPos;
    [SerializeField] private GameObject bulletPrefab;

    [Header("Parametrs")]
    [SerializeField] private int health;
    [SerializeField] private int shieldProtect;
    private int startHealth;
    private int startShieldProtect;

    public enum ActionType
    {
        None,
        Running,
        Dashing,
    }
    [Header("Action")]
    [SerializeField] private ActionType actionType;

    [SerializeField] private bool playerDetected;

    [Header("Movement")]
    [SerializeField] private Vector2 startPos;
    [SerializeField] private Vector3 randomPoint;
    [SerializeField] private float startTimeBtwGenerateRandomPoint;
    private float timeBtwGenerateRandomPoint;
    [SerializeField] private float randomPointRadius;
    [SerializeField] private float stoppingDistance;
    private float rotationZ;
    private Vector2 moveDirection;
    private Vector2 lookDirection;
    private Vector2 movePosition;

    [Header("Fight")]
    [SerializeField] private float startTimeBtwAttack;
    private float timeBtwAttack;
    [SerializeField] private float startShootingRange;
    [SerializeField] private float attackRange;

    [Header("")]
    [SerializeField] private float startDetectionRange;
    private float detectionRange;
    private float maxDetectionRange;
    private float playerDistance;
    private Vector2 playerDirection;
    [SerializeField] private LayerMask playerLayer;

    [Header("MatBlink")]
    private Material matBlink;
    private Material matDefault;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private LayerMask layer;
    private bool HandRight = true;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.avoidancePriority = Random.Range(0, 100);

        spriteRenderer = GetComponent<SpriteRenderer>();
        matBlink = Resources.Load("Blink", typeof(Material)) as Material;
        matDefault = spriteRenderer.material;

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        animator = GetComponent<Animator>();

        detectionRange = startDetectionRange;
        maxDetectionRange = startDetectionRange * 1.5f;

        startPos = transform.position;

        randomPoint = startPos;
        timeBtwGenerateRandomPoint = startTimeBtwGenerateRandomPoint;

        timeBtwAttack = startTimeBtwAttack;

        startHealth = health;
        startShieldProtect = shieldProtect;
    }

    private void Update()
    {
        if (player.gameObject.activeSelf)
        {
            playerDistance = Vector2.Distance(player.transform.position, transform.position);

            if (playerDistance <= detectionRange)
                playerDetected = true;
            else
                playerDetected = false;

            if (timeBtwAttack <= 0)
            {
                if (playerDistance <= attackRange)
                    switch (typeEnemy)
                    {
                        case TypeEnemy.Melle:
                            MelleAttack();
                            break;
                        case TypeEnemy.Range:
                            RangeAttack();
                            break;
                    }

                timeBtwAttack = startTimeBtwAttack;
            }
            else timeBtwAttack -= Time.deltaTime;

            Movement();
            HandRotation(lookDirection);
            Animation();
        }
    }

    private Vector2 GetRandomPoint(float radius, Vector2 position)
    {
        NavMeshHit navMeshHit;
        NavMesh.SamplePosition((Vector2)Random.insideUnitSphere * radius + position, out navMeshHit, radius, NavMesh.AllAreas);

        Vector2 randomPoint = navMeshHit.position;

        return randomPoint;
    } 

    private void Movement()
    {
        if (playerDetected)
        {
            startDetectionRange = maxDetectionRange;
            agent.stoppingDistance = stoppingDistance;

            FollowThePlayer();
            actionType = ActionType.Running;

            if (timeBtwGenerateRandomPoint <= 0)
            {
                actionType = ActionType.Dashing;
                timeBtwGenerateRandomPoint = startTimeBtwGenerateRandomPoint;
            }
            else timeBtwGenerateRandomPoint -= Time.deltaTime;

            //switch (typeEnemy)
            //{
            //    case TypeEnemy.Melle:
            //        break;
            //    case TypeEnemy.Range:
            //        break;
            //}
        }
        else
        {
            startDetectionRange = detectionRange;
            agent.stoppingDistance = 0;

            Patrol();
            actionType = ActionType.Running;

            
        }

        agent.SetDestination(movePosition);

        moveDirection = movePosition - (Vector2)transform.position;
        lookDirection = moveDirection;

        //isRunning = agent.velocity.magnitude > 0.1f;

        if (actionType == ActionType.Dashing)
        {
            StartCoroutine(Dash());
        }
        else StopAllCoroutines();
    }

    private void Patrol()
    {
        if (agent.velocity.magnitude == 0)
        {
            if (timeBtwGenerateRandomPoint <= 0)
            {
                movePosition = GetRandomPoint(randomPointRadius, startPos);
                timeBtwGenerateRandomPoint = startTimeBtwGenerateRandomPoint;
            }
            else timeBtwGenerateRandomPoint -= Time.deltaTime;

            moveDirection = (Vector3)movePosition - transform.position;
            lookDirection = moveDirection;    
        }
    }

    private Vector2 FollowThePlayer()
    {
        movePosition = player.transform.position;

        return movePosition;
    }

    private Vector2 RandomMove()
    {
        movePosition = GetRandomPoint(randomPointRadius, transform.position);

        return movePosition;
    }

    private IEnumerator Dash()
    {
        while (true)
        {
            agent.velocity = GetRandomPoint(1f, transform.position).normalized * 5;
            yield return new WaitForFixedUpdate();

            yield return new WaitForSecondsRealtime(.01f);
            actionType = ActionType.Dashing;
        }
    }

    private void HandRotation(Vector2 direction)
    {
        rotationZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        hands.transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);

        if (!HandRight && direction.x > 0)
        {
            FlipGun();
        }
        else if (HandRight && direction.x < 0)
        {
            FlipGun();
        }
    }

    private void Animation()
    {
        animator.SetFloat("Horizontal", moveDirection.x);
        animator.SetFloat("Vertical", moveDirection.y);

        animator.SetBool("IsRunning", actionType == ActionType.Running);

        if (moveDirection.x != 0 || moveDirection.y != 0)
        {
            animator.SetFloat("Last_Horizontal", moveDirection.x);
            animator.SetFloat("Last_Vertical", moveDirection.y);
        }
    }

    private void MelleAttack()
    {
        playerDirection = player.transform.position - transform.position;

        animator.SetTrigger("Attack");
        animator.SetFloat("AttackHorizontal", playerDirection.x);
        animator.SetFloat("AttackVertical", playerDirection.y);

        Collider2D[] hitEnemy = Physics2D.OverlapCircleAll(melleAttackPos.transform.position, attackRange, playerLayer, ~this.gameObject.layer);

        foreach (Collider2D enemy in hitEnemy)
        {
            enemy.GetComponent<Player>().TakeDamage(10);
        }
    }

    private void RangeAttack()
    {
        playerDirection = player.transform.position - transform.position;

        RaycastHit2D lookingAt = Physics2D.Raycast(transform.position, playerDirection, attackRange, ~layer);

        if (lookingAt.collider.name == "Player")
        {
            hands.transform.right = playerDirection;
            if (timeBtwAttack <= 0)
            {
                switch (typeGun)
                {
                    case TypeGun.Pistol:
                        Shoot();
                        break;
                    case TypeGun.Vector:
                        Shoot();
                        break;
                    case TypeGun.M4A1:
                        StartCoroutine(ShootM4A1());
                        break;
                }
            }
            else
                timeBtwAttack -= Time.deltaTime;

            if (playerDirection.y > 0)
            {
                gun.sortingOrder = 1;
            }
            else gun.sortingOrder = 3;
        }
    }

    private IEnumerator ShootM4A1()
    {
        for (int i = 0; i < 3; i++)
        {
            Shoot();
            yield return new WaitForSeconds(0.15f);
        }
    }

    private void Shoot()
    {
        GameObject bullet = ObjectPool.Instance.GetPooledObject(bulletPrefab);

        if (bullet != null)
        {
            bullet.transform.position = firePointPos.transform.position;
            bullet.transform.rotation = firePointPos.transform.rotation;
            bullet.SetActive(true);

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.AddForce(firePointPos.transform.right * 30, ForceMode2D.Impulse);
        }

        timeBtwAttack = startTimeBtwAttack;
    }

    private void FlipGun()
    {
        HandRight = !HandRight;
        Vector3 Scaler = gun.transform.localScale;
        Scaler.y *= -1;
        gun.transform.localScale = Scaler;
    }

    private void ResetMaterial()
    {
        spriteRenderer.material = matDefault;
    }

    public void TakeDamage(int damage)
    {
        if (shieldProtect <= 0)
        {
            health -= damage;
        }
        else
        {
            shieldProtect -= damage;
            if (shieldProtect <= 0)
            {
                shield.SetActive(false);
            }
        }


        spriteRenderer.material = matBlink;

        if (health <= 0)
            Death();
        else Invoke("ResetMaterial", .1f);
    }

    private void Death()
    {
        gameObject.SetActive(false);
        health = startHealth;
        shieldProtect = startShieldProtect;
        if (shield != null)
        {
            shield.SetActive(true);
        }
        ResetMaterial();
        GetComponent<ItemBag>().InstantiateItem(transform.position);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, startDetectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);


        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(startPos, randomPointRadius);
        Gizmos.DrawSphere(randomPoint, .1f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.DrawWireSphere(melleAttackPos.transform.position, attackRange);
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
