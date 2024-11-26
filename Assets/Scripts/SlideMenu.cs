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
        selectedTurret.turrentType = Turret.TurretType.Dmg;
    }

    public void UpgradeCtrl()
    {
        selectedTurret.UpgradeCtrl();
        selectedTurret.turrentType = Turret.TurretType.Ctrl;
    }

    public void UpgradeSpd()
    {
        selectedTurret.UpgradeSpd();
        selectedTurret.turrentType = Turret.TurretType.Spd;
    }

    public void UpgradeSprt()
    {
        selectedTurret.UpgradeSprt();
        selectedTurret.turrentType = Turret.TurretType.Sprt;
    }

    public void UpgradeTurretType()
    {
        switch (selectedTurret.turrentType)
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

    void OnGUI()
    {
        currencyUI.text = "$" + LevelManager.main.GetCurrency().ToString();
        if (Time.time >= startTime + delayTime)
        {
            EnemyUI.text = "Enemies Left: " + LevelManager.main.GetEnemiesLeft().ToString();
        }
        if (selectedTurret == null) return;
        string lvlTxt = "";
        switch (selectedTurret.turrentType)
        {
            case Turret.TurretType.Dmg:
                lvlTxt = selectedTurret.dmgLevel.ToString();
                if (selectedTurret.dmgLevel == selectedTurret.maxLvl) lvlTxt = "Max";
                LvlLabel.text = "Turret Lvl: " + lvlTxt;
                turretTypeImage.sprite = dmgSprite;
                turretTypeImage.gameObject.SetActive(true);
                UpgradeButton.SetActive(true);
                SpecializeButtonGroup.SetActive(false);
                break;
            case Turret.TurretType.Spd:
                lvlTxt = selectedTurret.spdLevel.ToString();
                if (selectedTurret.spdLevel == selectedTurret.maxLvl) lvlTxt = "Max";
                LvlLabel.text = "Turret Lvl: " + lvlTxt;
                turretTypeImage.sprite = spdSprite;
                turretTypeImage.gameObject.SetActive(true);
                UpgradeButton.SetActive(true);
                SpecializeButtonGroup.SetActive(false);
                break;
            case Turret.TurretType.Ctrl:
                lvlTxt = selectedTurret.ctrlLevel.ToString();
                if (selectedTurret.ctrlLevel == selectedTurret.maxLvl) lvlTxt = "Max";
                LvlLabel.text = "Turret Lvl: " + lvlTxt;
                turretTypeImage.sprite = ctrlSprite;
                turretTypeImage.gameObject.SetActive(false);
                UpgradeButton.SetActive(true);
                SpecializeButtonGroup.SetActive(false);
                break;
            case Turret.TurretType.Sprt:
                lvlTxt = selectedTurret.spdLevel.ToString();
                if (selectedTurret.ctrlLevel == selectedTurret.sprtLevel) lvlTxt = "Max";
                LvlLabel.text = "Turret Lvl: " + lvlTxt;
                turretTypeImage.sprite = sprtSprite;
                turretTypeImage.gameObject.SetActive(false);
                UpgradeButton.SetActive(true);
                SpecializeButtonGroup.SetActive(false);
                break;
            case Turret.TurretType.None:
                LvlLabel.text = "Choose Path: $100";
                turretTypeImage.sprite = null;
                turretTypeImage.gameObject.SetActive(false);
                UpgradeButton.SetActive(false);
                SpecializeButtonGroup.SetActive(true);
                break;
            default:
                break;
        }
    }
}
