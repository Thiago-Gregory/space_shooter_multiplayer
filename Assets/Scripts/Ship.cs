using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Ship : NetworkBehaviour
{
    [SerializeField] private GameObject textAmmo;
    [SerializeField] private GameObject prefabBullet;
    [SerializeField] private int speed;
    [SerializeField] private int maxAmmo;
    [SerializeField] private float maxLife;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float shootCooldown;

    private Rigidbody2D rb;
    private GameObject sliderReloading;
    private SceneScript sceneScript;
    private float moveX;
    private float moveY;
    private float life;
    private float shootCooldownTime;
    private int ammoCount;
    private bool reloadingAmmo;

    private void Awake()
    {
        sceneScript = FindObjectOfType<SceneScript>();
        sceneScript.showPlayerUI();
        sliderReloading = GameObject.FindGameObjectWithTag("SliderReloadingAmmo");
        reloadingAmmo = false;
        life = maxLife;
        ammoCount = maxAmmo;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sliderReloading.GetComponent<Slider>().value = 0;
    }

    void Update()
    {
        moveX = Input.GetAxis("Horizontal") * speed;
        moveY = Input.GetAxis("Vertical") * speed;

        rb.velocity = new Vector2(moveX, moveY);

        checkPlayerInPlayArea();

        if (isLocalPlayer)
        {
            if (!reloadingAmmo && ammoCount > 0 && (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)))
            {
                if (Time.time > shootCooldownTime)
                {
                    shootCooldownTime = Time.time + shootCooldown;
                    ammoCount--;
                    cmdShoot();
                }
            }
            else
            {
                CancelInvoke("cmdShoot");
            }

            if (!reloadingAmmo && ammoCount < maxAmmo && Input.GetKeyDown(KeyCode.R))
            {
                reloadAmmo();
            }

            if (reloadingAmmo && sliderReloading.GetComponent<Slider>().value < 1)
            {
                sliderReloading.GetComponent<Slider>().value += 0.01f;
            }
            else if (sliderReloading.GetComponent<Slider>().value >= 1)
            {
                ammoCount = maxAmmo;
                reloadingAmmo = false;
                sliderReloading.GetComponent<Slider>().value = 0;
                sliderReloading.GetComponent<Slider>().fillRect.gameObject.SetActive(false);
            }

            if (life > maxLife)
            {
                life = maxLife;
            }

            
            if (life <= 0)
            {
                if (isLocalPlayer)
                {
                    sceneScript.showGameOver();
                }
                playerDeath();
            }

            //healingZone.transform.position = transform.position;

            sceneScript.UILifeBar(maxLife, life);
            sceneScript.UIAmmoText("Ammo: " + ammoCount);
        }
    }

    void reloadAmmo()
    {
        CancelInvoke("cmdShoot");

        sliderReloading.GetComponent<Slider>().value = 0;
        sliderReloading.GetComponent<Slider>().fillRect.gameObject.SetActive(true);
        reloadingAmmo = true;
    }

    void checkPlayerInPlayArea()
    {
        if (transform.position.x <= -7.375f)
        {
            transform.position = new Vector2(-7.375f, transform.position.y);
        }
        else if (transform.position.x >= 7.375f)
        {
            transform.position = new Vector2(7.375f, transform.position.y);
        }

        if (transform.position.y <= -5.5f)
        {
            transform.position = new Vector2(transform.position.x, -5.5f);
        }
        else if (transform.position.y >= 5.5f)
        {
            transform.position = new Vector2(transform.position.x, 5.5f);
        }
    }

    [Command]
    void cmdShoot()
    {
        GameObject bullet = Instantiate(prefabBullet, transform.position, Quaternion.identity);

        bullet.GetComponent<Rigidbody2D>().velocity = Vector2.up * bulletSpeed;

        NetworkServer.Spawn(bullet);

        Destroy(bullet, 1.5f);
    }

    public void decreaseLife(int damage)
    {
        life -= damage;
    }

    [Command]
    private void playerDeath()
    {
        Destroy(gameObject);
    }
}
