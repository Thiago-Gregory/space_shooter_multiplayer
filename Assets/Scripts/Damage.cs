using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Damage : NetworkBehaviour
{
    [SerializeField] private int bulletDamage;
    [SerializeField] private List<string> enemyTag;
    
    void OnTriggerEnter2D(Collider2D collider)
    {
        /*
         * if (enemyTag.Contains(collider.tag))
        {
            //Destroy(collider.gameObject);
            Destroy(gameObject);
        }
        */

        if (gameObject.CompareTag("EnemyBullet") && collider.CompareTag("Player"))
        {
            collider.GetComponent<Ship>().decreaseLife(bulletDamage);
            Destroy(gameObject);
        }
        else if (gameObject.CompareTag("PlayerBullet") && collider.CompareTag("Enemy"))
        {
            //collider.GetComponent<Enemy>().lifeBar.GetComponent<Renderer>().enabled = true;
            collider.GetComponent<Enemy>().decreaseLife(bulletDamage);
            Destroy(gameObject);
        }
    }
}
