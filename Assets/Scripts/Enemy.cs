using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Enemy : NetworkBehaviour
{
    [SerializeField] private GameObject prefabBullet;
    [SerializeField] private int maxLife;
    [SerializeField] private float speed;
    [SerializeField] private float sideSpeed;
    [SerializeField] private float weaponReloadTime;
    [SerializeField] private int magazineSize;
    [SerializeField] private float shootDelay;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private int killScore;

    public GameObject lifeBar;

    private int life;
    private bool killed;

    private SceneScript sceneScript;

    private void Awake()
    {
        sceneScript = FindObjectOfType<SceneScript>();
    }

    // Start is called before the first frame update
    void Start()
    {
        killed = false;
        life = maxLife;

        GetComponent<Rigidbody2D>().velocity = Vector2.down * speed;

        StartCoroutine(startShoot());
    }

    // Update is called once per frame
    void Update()
    {
        //lifeBar.transform.localScale = new Vector3(0.028f * life / maxLife, 1, 0.003f);

        if (life <= 0 && !killed)
        {
            killed = true;
            enemyDeath();
        }
    }

    IEnumerator startShoot()
    {
        while (true)
        {
            StartCoroutine(shoot());

            yield return new WaitForSeconds(weaponReloadTime);
        }
    }

    IEnumerator shoot()
    {
        for (int i = 0; i < magazineSize; i++)
        {
            instantiateBullet();

            yield return new WaitForSeconds(shootDelay);
        }
    }

    void instantiateBullet()
    {
        GameObject bullet = Instantiate(prefabBullet, transform.position, Quaternion.identity);

        bullet.GetComponent<Rigidbody2D>().velocity = Vector2.down * bulletSpeed;

        NetworkServer.Spawn(bullet);

        Destroy(bullet, 3f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (transform.position.y <= collision.transform.position.y)
            {
                GetComponent<Rigidbody2D>().velocity = Vector2.left * sideSpeed;
            }
        }
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Ship>().decreaseLife(10);
            enemyDeath();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.down * speed;
        }
    }

    public void decreaseLife(int damage)
    {
        life -= damage;
    }

    public void enemyDeath()
    {
        life = 0;
        increaseScore(killScore);
        Destroy(gameObject);
    }

    void increaseScore(int value)
    {
        sceneScript.GetComponent<SceneScript>().increaseScore(value);
    }
}
