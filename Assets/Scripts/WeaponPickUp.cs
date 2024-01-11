using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickUp : MonoBehaviour
{
    public WeaponData weaponData;
    public Transform target;

    public GameObject pickUpButton;

    public float pickupDist;

    public int bulletInWeapon = 0;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;

        bulletInWeapon = weaponData.bulletCount;

        gameObject.GetComponent<SpriteRenderer>().sprite = weaponData.weaponSprite;
        gameObject.name = weaponData.weaponName;
    }

    private void Update()
    {
        if (target.gameObject.activeSelf)
        {
            float playerDistance = Vector2.Distance(target.position, transform.position);

            if (playerDistance <= pickupDist)
            {
                pickUpButton.SetActive(true);
            }
            else pickUpButton.SetActive(false);

            if (playerDistance <= pickupDist && Input.GetKeyDown(KeyCode.F))
                Pickup();
        }
    }

    private void Pickup()
    {
        Weapon weapon = target.GetComponent<Weapon>();
        WeaponData oldWeapon = weaponData;
        int oldBulletInWeapon = bulletInWeapon;
        if (weapon.slot[weapon.activeSlot] != null)
        {
            weaponData = weapon.slot[weapon.activeSlot].weaponData;
            bulletInWeapon = weapon.slot[weapon.activeSlot].currentAmmo;

            weapon.slot[weapon.activeSlot].weaponData = oldWeapon;
            weapon.slot[weapon.activeSlot].currentAmmo = oldBulletInWeapon;
            weapon.weaponSprite.sprite = oldWeapon.weaponSprite;
            

            gameObject.GetComponent<SpriteRenderer>().sprite = weaponData.weaponSprite;
            gameObject.name = weaponData.weaponName;
            transform.position = target.position;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupDist);
    }
}
