using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Slot
{
    public WeaponData weaponData;

    [Header("Bullet Count")]
    public int currentAmmo;
    public int allAmmo;
}
public class Weapon : MonoBehaviour
{
    private Player player;
    private Camera cachedCamera;
    
    public List<Slot> slot = new List<Slot>();
    public int activeSlot = 0;

    [Header("Objects")]
    [SerializeField] private GameObject hands;
    public SpriteRenderer weaponSprite;
    [SerializeField] private GameObject melleAttackPos;
    [SerializeField] private GameObject firePointPos;
    [SerializeField] private GameObject bulletPrefab;

    [Header("Rotation")]
    public Vector2 difference;
    private float rotationZ;

    [Header("Fight")]
    [SerializeField] private float melleAttackRange;
    [SerializeField] private LayerMask enemyLayer;
    private float timeBtwShoots;
    
    [Header("Keybinds")]
    [SerializeField] private KeyCode attackKey = KeyCode.Mouse0;
    [SerializeField] private KeyCode weaponSwichKey = KeyCode.Q;
    [SerializeField] private KeyCode realoadKey = KeyCode.R;

    private bool HandRight = true;

    private void Start()
    {
        player = GetComponent<Player>();
        cachedCamera = Camera.main;

        weaponSprite.sprite = slot[activeSlot].weaponData.weaponSprite;
        timeBtwShoots = slot[activeSlot].weaponData.delay;

        foreach (var item in slot)
        {
            item.currentAmmo = item.weaponData.bulletCount;
        }
    }

    private void Update()
    {
        if (!PauseMenu.Instance.pauseGame)
        {
            if (timeBtwShoots <= 0)
            {
                switch (slot[activeSlot].weaponData.shootingType)
                {
                    case WeaponData.ShootingType.Melle:
                        if (Input.GetKeyDown(attackKey) && player.actionType == Player.ActionType.None)
                        {
                            MelleAttack();
                            timeBtwShoots = slot[activeSlot].weaponData.delay;
                        }
                        break;
                    case WeaponData.ShootingType.Single:
                        if (Input.GetKeyDown(attackKey) && slot[activeSlot].currentAmmo > 0 && player.actionType == Player.ActionType.None)
                        {
                            Shoot();
                            timeBtwShoots = slot[activeSlot].weaponData.delay;
                        }
                        break;
                    case WeaponData.ShootingType.AutomaticFire:
                        if (Input.GetKey(attackKey) && slot[activeSlot].currentAmmo > 0 && player.actionType == Player.ActionType.None)
                        {
                            Shoot();
                            timeBtwShoots = slot[activeSlot].weaponData.delay;
                        }
                        break;
                    case WeaponData.ShootingType.Burst:
                        if (Input.GetKey(attackKey) && slot[activeSlot].currentAmmo > 0 && player.actionType == Player.ActionType.None)
                        {
                            StartCoroutine(ShootBurst());
                            timeBtwShoots = slot[activeSlot].weaponData.delay;
                        }
                        break;
                }
            }
            else timeBtwShoots -= Time.deltaTime;

            HandleInput();
        }
    }

    private void FixedUpdate()
    {
        if (!PauseMenu.Instance.pauseGame)
        {
            HandsMovement();
            UpdateSprite();
            UpdateAllAmmo();
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(weaponSwichKey) && player.actionType == Player.ActionType.None)
        {
            WeaponSwich();
        }

        if (Input.GetKeyDown(realoadKey) && slot[activeSlot].currentAmmo < slot[activeSlot].weaponData.bulletCount && player.actionType == Player.ActionType.None)
        {
            player.actionType = Player.ActionType.Reloading;
        }
    }

    private void HandsMovement()
    {
        difference = cachedCamera.ScreenToWorldPoint(Input.mousePosition) - hands.transform.position;
        rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        hands.transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);
    }

    private void WeaponSwich()
    {
        if (activeSlot == 0)
        {
            activeSlot = 1;
        }
        else activeSlot = 0;

        weaponSprite.sprite = slot[activeSlot].weaponData.weaponSprite;
    }

    private void UpdateSprite()
    {
        if (!HandRight && difference.x > 0)
        {
            FlipGun();
        }
        else if (HandRight && difference.x < 0)
        {
            FlipGun();
        }

        if (difference.y > 0)
        {
            weaponSprite.sortingOrder = 1;
        }
        else weaponSprite.sortingOrder = 3;
    }

    private void UpdateAllAmmo()
    {
        foreach (var slotItem in slot)
        {
            string ammoItemName = GetAmmoType(slotItem.weaponData.weaponType);
            InventorySlot ammoInventorySlot = Inventory.Instance.Items.Find(item => item.item.name == ammoItemName);

            if (ammoInventorySlot != null)
            {
                slotItem.allAmmo = ammoInventorySlot.itemCount;
            }
            else if (ammoItemName == "Melle")
            {
                slotItem.allAmmo = 0;
            }
        }
    }

    private void MelleAttack()
    {
        Collider2D[] hitEnemy = Physics2D.OverlapCircleAll(melleAttackPos.transform.position, melleAttackRange, enemyLayer, ~this.gameObject.layer);

        foreach (Collider2D enemy in hitEnemy)
        {
            enemy.GetComponent<Enemy>().TakeDamage(10);
        }
    }

    private void Shoot()
    {
        slot[activeSlot].currentAmmo -= 1;

        GameObject bullet = ObjectPool.Instance.GetPooledObject(bulletPrefab);

        if (bullet != null)
        {
            bullet.transform.position = firePointPos.transform.position;
            bullet.transform.rotation = firePointPos.transform.rotation;
            bullet.SetActive(true);

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.AddForce(firePointPos.transform.right * 30, ForceMode2D.Impulse);
        }
    }

    private IEnumerator ShootBurst()
    {
        if (slot[activeSlot].currentAmmo < 3)
        {
            int j = slot[activeSlot].currentAmmo;
            for (int i = 0; i < j; i++)
            {
                Shoot();
                yield return new WaitForSeconds(0.15f);
            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                Shoot();
                yield return new WaitForSeconds(0.15f);
            }
        }
    }

    public void Reload()
    {
        //yield return new WaitForSeconds(.1f);
        int reason = slot[activeSlot].weaponData.bulletCount - slot[activeSlot].currentAmmo;
        string ammoItemName = GetAmmoType(slot[activeSlot].weaponData.weaponType);

        // Find the inventory slot corresponding to the ammo item
        InventorySlot ammoInventorySlot = Inventory.Instance.Items.Find(item => item.item.name == ammoItemName);

        if (ammoInventorySlot != null)
        {
            // Add ammunition to currentAmmo from the inventory
            int ammoToAdd = Mathf.Min(reason, ammoInventorySlot.itemCount);
            slot[activeSlot].currentAmmo += ammoToAdd;
            ammoInventorySlot.itemCount -= ammoToAdd;
        }
    }

    private string GetAmmoType(WeaponData.WeaponType weaponType)
    {
        switch (weaponType)
        {
            case WeaponData.WeaponType.Pistol:
                return "AmmoPistol";
            case WeaponData.WeaponType.SMG:
                return "AmmoSMG";
            case WeaponData.WeaponType.AR:
                return "AmmoAR";
            case WeaponData.WeaponType.Melle:
                return "Melle";
            default:
                return "";
        }
    }

    private void FlipGun()
    {
        HandRight = !HandRight;
        Vector3 Scaler = hands.transform.localScale;
        Scaler.y *= -1;
        hands.transform.localScale = Scaler;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(melleAttackPos.transform.position, melleAttackRange);

        Gizmos.DrawWireSphere(melleAttackPos.transform.position, .025f);
        Gizmos.DrawWireSphere(firePointPos.transform.position, .025f);
    }
}
