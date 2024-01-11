using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public static Manager Instance;

    private Player player;
    private Weapon weapon;
    private Spawner spawner;

    [SerializeField] private GameObject deathScreen;
    [SerializeField] private GameObject loadingScreen;

    [Header("ActionIndicator")]
    [SerializeField] private Slider actionIndicator;
    [SerializeField] private float startTimeAction = 1f;
    [SerializeField] private float timeAction = 0;
    [SerializeField] private Vector3 offset;
    
    [SerializeField] private GameObject indicator;
    [SerializeField] private GameObject endGameTrigger;

    [SerializeField] private Image healthBar;
    [SerializeField] private Image staminaBar;
    [SerializeField] private Image weaponImage;
    [SerializeField] private Text ammoCountUI;

    [SerializeField] private Text medKitCountUI;
    [SerializeField] private Text itemCountUI;

    private GameObject[] enemies;
    private GameObject[] items;
    private GameObject[] enemySpawner;

    public int countMedKit = 0;
    public int countItem = 0;

    private void Start()
    {
        Instance = this;

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        weapon = player.GetComponent<Weapon>();
        spawner = GameObject.FindGameObjectWithTag("PlayerSpawnPoint").GetComponent<Spawner>();

        timeAction = startTimeAction;
    }

    private void Update()
    {
        deathScreen.SetActive(!player.gameObject.activeSelf && !loadingScreen.activeSelf);

        if (player.gameObject.activeSelf)
        {
            healthBar.fillAmount = player.health / 100f;
            staminaBar.fillAmount = player.stamina / 100f;

            ammoCountUI.text = $"{weapon.slot[weapon.activeSlot].currentAmmo}/{weapon.slot[weapon.activeSlot].allAmmo}";

            weaponImage.sprite = weapon.slot[weapon.activeSlot].weaponData.weaponSprite;

            if (countItem >= 3)
            {
                indicator.SetActive(true);

                Vector2 difference = endGameTrigger.transform.position - indicator.transform.position;
                float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
                indicator.transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);
            }
            else indicator.SetActive(false);

            UpdateCountItem();

            switch (player.actionType)
            {
                case Player.ActionType.None:
                    HideActionIndicator();
                    actionIndicator.maxValue = startTimeAction;
                    break;
                case Player.ActionType.Healing:
                    ShowActionIndicator();
                    ProgressAction();
                    if (TimerActionIndicator() <= 0)
                    {
                        player.actionType = Player.ActionType.None;
                        player.UseMedKit();
                        timeAction = startTimeAction;
                    }
                    break;
                case Player.ActionType.Reloading:
                    ShowActionIndicator();
                    ProgressAction();
                    if (TimerActionIndicator() <= 0)
                    {
                        player.actionType = Player.ActionType.None;
                        weapon.Reload();
                        timeAction = startTimeAction;
                    }
                    break;
            }
        }
    }

    private void FixedUpdate()
    {
        FollowIndicator();
    }

    private void FollowIndicator()
    {
        actionIndicator.transform.position = player.transform.position + offset;
    }

    private void ShowActionIndicator()
    {
        actionIndicator.gameObject.SetActive(true);
    }

    private void HideActionIndicator()
    {
        actionIndicator.gameObject.SetActive(false);
    }

    private float TimerActionIndicator()
    {
        return timeAction -= Time.deltaTime;
    }

    private void ProgressAction()
    {
        actionIndicator.value = timeAction;
    }

    private void RespawnPlayer()
    {
        spawner.SpawnPlayer();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        CameraFollow.Instance.player = player.transform;

        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        items = GameObject.FindGameObjectsWithTag("Item");
        enemySpawner = GameObject.FindGameObjectsWithTag("EnemySpawnPoint");

        foreach (var item in enemies)
            Destroy(item);

        foreach (var item in items)
            item.GetComponent<ItemPickup>().target = player.transform;

        foreach (var item in enemySpawner)
            item.GetComponent<Spawner>().playerTransform = player.transform;
    }

    private void UpdateCountItem()
    {
        countMedKit = 0;
        countItem = 0;

        var inventoryItems = Inventory.Instance.Items;

        foreach (var item in inventoryItems)
        {
            switch (item.item.itemName)
            {
                case "MedKit":
                    countMedKit = item.itemCount;
                    break;
                case "Item":
                    countItem = item.itemCount;
                    break;
            }
        }

        medKitCountUI.text = countMedKit.ToString();
        itemCountUI.text = $"{countItem}/3";
    }
}
