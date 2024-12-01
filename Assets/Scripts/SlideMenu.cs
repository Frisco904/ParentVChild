using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlideMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI currencyUI;
    [SerializeField] private TextMeshProUGUI EnemyUI;
    [SerializeField] private GameObject UpgradeButton;
    [SerializeField] private TextMeshProUGUI upgradeCostLabel;
    [SerializeField] private GameObject SpecializeButtonGroup;
    [SerializeField] private TextMeshProUGUI LvlLabel;
    [SerializeField] private Animator anim;

    [Header("Sprite References")]
    [SerializeField] private Image turretTypeImage;
    [SerializeField] private Sprite dmgSprite;
    [SerializeField] private Sprite spdSprite;
    [SerializeField] private Sprite ctrlSprite;
    [SerializeField] private Sprite sprtSprite;


    private float delayTime = 5f;
    private float startTime;
    private Turret selectedTurret;
    private bool isMenuOpen = true;

    private void Start()
    {
        startTime = Time.time;
        SetMenu(false);
    }

    //This is for the menu animation
    public void SetMenu(bool value)
    {

        isMenuOpen = value;
        anim.SetBool("MenuAnimation", isMenuOpen);

        // Check the Level manager for the selected turret every time the menu is toggled
        selectedTurret = LevelManager.main.selectedTurret;
    }

    public void UpgradeDmg()
    {
        selectedTurret.UpgradeDmg();
        selectedTurret.turretType = Turret.TurretType.Dmg;
    }

    public void UpgradeCtrl()
    {
        selectedTurret.UpgradeCtrl();
        selectedTurret.turretType = Turret.TurretType.Ctrl;
    }

    public void UpgradeSpd()
    {
        selectedTurret.UpgradeSpd();
        selectedTurret.turretType = Turret.TurretType.Spd;
    }

    public void UpgradeSprt()
    {
        selectedTurret.UpgradeSprt();
        selectedTurret.turretType = Turret.TurretType.Sprt;
    }

    public void UpgradeTurretType()
    {
        switch (selectedTurret.turretType)
        {
            case Turret.TurretType.Dmg:
                selectedTurret.UpgradeDmg();
                break;
            case Turret.TurretType.Spd:
                selectedTurret.UpgradeSpd();
                break;
            case Turret.TurretType.Ctrl:
                selectedTurret.UpgradeCtrl();
                break;
            case Turret.TurretType.Sprt:
                selectedTurret.UpgradeSprt();
                break;
            case Turret.TurretType.None:
                break;
            default:
                break;
        }
    }

    public void SellTurret()
    {
        selectedTurret.SellTurret();
        TurretBuilder.main.DeselectTurretCheck();
        SetMenu(false);
    }

    public void RepairTurret()
    {
        selectedTurret.RepairTurret();
        SetMenu(true);
    }

    void OnGUI()
    {
        currencyUI.text = "$" + LevelManager.main.GetCurrency().ToString();
        if (Time.time >= startTime + delayTime)
        {
            EnemyUI.text = "Enemies Left: " + LevelManager.main.GetEnemiesLeft().ToString();
        }
        if (selectedTurret == null) return;
        string lvlTxt = "";
        switch (selectedTurret.turretType)
        {
            case Turret.TurretType.Dmg:
                lvlTxt = (selectedTurret.dmgLevel - 1).ToString();
                if (selectedTurret.dmgLevel == selectedTurret.maxLvl) lvlTxt = "Max";
                LvlLabel.text = "Turret Lvl: " + lvlTxt;
                turretTypeImage.sprite = dmgSprite;
                upgradeCostLabel.text = "($" + selectedTurret.calculateCostDamage() + ")";
                turretTypeImage.gameObject.SetActive(true);
                UpgradeButton.SetActive(true);
                SpecializeButtonGroup.SetActive(false);
                break;
            case Turret.TurretType.Spd:
                lvlTxt = (selectedTurret.spdLevel - 1).ToString();
                if (selectedTurret.spdLevel == selectedTurret.maxLvl) lvlTxt = "Max";
                LvlLabel.text = "Turret Lvl: " + lvlTxt;
                turretTypeImage.sprite = spdSprite;
                upgradeCostLabel.text = "($" + selectedTurret.calculateCostFireRate() + ")";
                turretTypeImage.gameObject.SetActive(true);
                UpgradeButton.SetActive(true);
                // if (calculateCostFireRate() > LevelManager.main.GetCurrency()) UpgradeButton.interactable = false;
                SpecializeButtonGroup.SetActive(false);
                break;
            case Turret.TurretType.Ctrl:
                lvlTxt = (selectedTurret.ctrlLevel - 1).ToString();
                if (selectedTurret.ctrlLevel == selectedTurret.maxLvl) lvlTxt = "Max";
                LvlLabel.text = "Turret Lvl: " + lvlTxt;
                turretTypeImage.sprite = ctrlSprite;
                upgradeCostLabel.text = "($" + selectedTurret.calculateCostCtrl() + ")";
                turretTypeImage.gameObject.SetActive(true);
                UpgradeButton.SetActive(true);
                SpecializeButtonGroup.SetActive(false);
                break;
            case Turret.TurretType.Sprt:
                lvlTxt = (selectedTurret.sprtLevel - 1).ToString();
                if (selectedTurret.sprtLevel == selectedTurret.maxLvl) lvlTxt = "Max";
                LvlLabel.text = "Turret Lvl: " + lvlTxt;
                turretTypeImage.sprite = sprtSprite;
                upgradeCostLabel.text = "($" + selectedTurret.calculateCostSprt() + ")";
                turretTypeImage.gameObject.SetActive(true);
                UpgradeButton.SetActive(true);
                SpecializeButtonGroup.SetActive(false);
                break;
            case Turret.TurretType.None:
                LvlLabel.text = "Specialize: $150";
                turretTypeImage.sprite = null;
                upgradeCostLabel.text = "";
                turretTypeImage.gameObject.SetActive(false);
                UpgradeButton.SetActive(false);
                SpecializeButtonGroup.SetActive(true);
                break;
            default:
                break;
        }
    }
}
