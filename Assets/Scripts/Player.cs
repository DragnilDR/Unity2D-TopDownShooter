using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb;
    private Animator animator;
    private Weapon weapon;

    [Header("Parametrs")]
    public int health;
    public int stamina;
    [SerializeField] private float speed;
    [SerializeField] private float dashSpeed;

    public enum ActionType
    {
        None,
        Healing,
        Reloading
    }

    public ActionType actionType;

    [Header("State")]
    private bool isDashing;

    [Header("Movement")]
    private Vector2 moveDirection;
    private Vector2 dashDirection;

    [Header("Material")]
    private Material matBlink;
    private Material matDefault;
    private SpriteRenderer spriteRenderer;

    [Header("Keybinds")]
    public KeyCode dashKey = KeyCode.LeftShift;
    public KeyCode useMedKitKey = KeyCode.E;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        weapon = GetComponent<Weapon>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        matBlink = Resources.Load("Blink", typeof(Material)) as Material;
        matDefault = spriteRenderer.material;
    }

    private void Update()
    {
        if (!PauseMenu.Instance.pauseGame)
        {
            HandleInput();
            Animation();

            if (isDashing)
            {
                StartCoroutine(Dash());
            }
            else StopAllCoroutines();
        }   
    }

    private void FixedUpdate()
    {
        if (!PauseMenu.Instance.pauseGame)
        {
            if (!isDashing)
                Movement();

            stamina = Mathf.Clamp(stamina + 1, 0, 100);
        }
    }

    private void HandleInput()
    {
        moveDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        if (Input.GetKeyDown(useMedKitKey) && health < 100 && actionType == ActionType.None)
        {
            actionType = ActionType.Healing;

            if (actionType == ActionType.None)
                UseMedKit();
        }
        
        if (Input.GetKeyDown(dashKey) && moveDirection != Vector2.zero && stamina == 100)
        {
            stamina = 0;
            dashDirection = moveDirection;
            isDashing = true;
            animator.SetBool("isDashing", isDashing);
        }
        else animator.SetBool("isDashing", isDashing);
    }

    private void Movement()
    {
        rb.velocity = moveDirection * speed;
    }

    private void Animation()
    {
        animator.SetFloat("Speed", moveDirection.sqrMagnitude);
        animator.SetFloat("Movement_X", moveDirection.x);
        animator.SetFloat("Movement_Y", moveDirection.y);

        animator.SetFloat("Horizontal", weapon.difference.x);
        animator.SetFloat("Vertical", weapon.difference.y);

        if (weapon.difference.x != 0 || weapon.difference.y != 0)
        {
            animator.SetFloat("Last_Horizontal", weapon.difference.x);
            animator.SetFloat("Last_Vertical", weapon.difference.y);
        }
    }

    public void UseMedKit()
    {
        InventorySlot medKit = null;

        foreach (var item in Inventory.Instance.Items)
        {
            if (item.item.name == "MedKit")
            {
                medKit = item;
                break;
            }
        }

        if (medKit != null)
        {
            if (medKit.itemCount > 0)
            {
                health = 100;
                medKit.itemCount -= 1;
            }
            else
            {
                Inventory.Instance.Items.Remove(medKit);
            }
        }
    }

    private IEnumerator Dash()
    {
        while (true)
        {
            rb.velocity = dashDirection * dashSpeed;
            yield return new WaitForFixedUpdate();

            yield return new WaitForSecondsRealtime(.2f);
            isDashing = false;
        }
    }

    private void ResetMaterial()
    {
        spriteRenderer.material = matDefault;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        spriteRenderer.material = matBlink;

        if (health <= 0)
            Death();
        else Invoke("ResetMaterial", .1f);
    }

    private void Death()
    {
        gameObject.SetActive(false);
        health = 100;
        ResetMaterial();
    }
}