using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private int damage;

    private enum TypeBullet { Player, Enemy };
    [SerializeField] private TypeBullet typeBullet;

    private void Start()
    {
        Invoke("DestroyBullet", 0.5f);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        switch (collider.tag)
        {
            case "Enemy":
                if (typeBullet == TypeBullet.Player)
                {
                    collider.GetComponent<Enemy>().TakeDamage(damage);
                }

                DestroyBullet();
                break;
            case "Player":
                if (typeBullet == TypeBullet.Enemy)
                {
                    collider.GetComponent<Player>().TakeDamage(damage);
                }

                DestroyBullet();
                break;
            case "Wall":
                DestroyBullet();
                break;
        }
    }

    private void DestroyBullet()
    {
        gameObject.SetActive(false);
    }
}
