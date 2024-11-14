using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject[] torrentPrefab;
    [SerializeField] private TextMeshProUGUI currencyUI;
    [SerializeField] private TextMeshProUGUI EnemyUI;
    [SerializeField] private GameObject upgradeUI;
    [SerializeField] private Animator anim;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button upgradeRange;
    [SerializeField] private Button upgradeDamage;
    [SerializeField] private Button upgradeFireRate;
    [SerializeField] public Button sellBtn;

    private float delayTime = 5f;
    private float startTime;
    private GameObject selectedTower;


    private bool isMenuOpen = true;

    public static Menu menu;

    private void Awake()
    {
        menu = this;
    }

    private void Start()
    {
        menuButton.onClick.AddListener(ToggleMenu); 
        upgradeRange.onClick.AddListener(UpgradeTRangeValue);
        upgradeDamage.onClick.AddListener(UpgradeTDmg);
        upgradeFireRate.onClick.AddListener(UpgradeTFireRate);
        sellBtn.onClick.AddListener(SellTorrent);
        startTime = Time.time;

    }
    //This is for the menu animation
    public void ToggleMenu()
    {
        if (isMenuOpen) anim.Play("MenuClose"); else anim.Play("MenuOpen");
        isMenuOpen = !isMenuOpen;
    }

    private void OnGUI()
    {
        currencyUI.text = LevelManager.main.GetCurrency().ToString();
        if (Time.time >= startTime + delayTime)
        {
            EnemyUI.text = "Enemies Left: " + LevelManager.main.GetEnemiesLeft().ToString();
        }
    }
    #region Upgrade UI
    private void UpgradeTRangeValue()
    {
        Tower.main.UpgradeTRange();
    }

    private void UpgradeTDmg()
    {
        Tower.main.UpgradeTDamage();
    }

    private void UpgradeTFireRate()
    {
        Tower.main.UpgradeTFireSpeed();
    }

    private void SellTorrent()
    {
        Tower.main.SellTorrent();
        
    }
    #endregion
    //This for getting the value in the TurrentSpawner.
    public void SetSelectedTower(GameObject tower)
    {
        selectedTower = tower;
        Debug.Log("Tower Selected: " + tower.name);
    }
    public GameObject GetSelectedTower()
    {
        return selectedTower;
    }
}
