using System;
using TMPro;
using UnityEngine;

public class SlideMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI currencyUI;
    [SerializeField] private TextMeshProUGUI EnemyUI;
    //[SerializeField] private TextMeshProUGUI LvlLabel;

    [SerializeField] private Animator anim;

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
        selectedTurret.turrentType = Turret.turretType.Dmg;
    }

    public void UpgradeCtrl()
    {
        selectedTurret.UpgradeCtrl();
        selectedTurret.turrentType = Turret.turretType.Ctrl;
    }

    public void UpgradeSpd()
    {
        selectedTurret.UpgradeSpd();
        selectedTurret.turrentType = Turret.turretType.Spd;
    }

    public void UpgradeSprt()
    {
        selectedTurret.UpgradeSprt();
        selectedTurret.turrentType = Turret.turretType.Sprt;
    }

    public void UpgradeTurretSpeciality()
    {
        // switch (selectedTurret.turretSpeciality)
        // {
        //     case TurretSpecialy.DMG:
        //         break;
        //     case TurretSpecialy.SPD:
        //         break;
        //     case TurretSpecialy.CTRL:
        //         break;
        //     case TurretSpecialy.SPRT:
        //         break;
        //     case TurretSpecialy.None:
        //         break;
        //     default:
        //         break;
        // }
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
        if (selectedTurret != null)
        {
            _ = selectedTurret.dmgLevel;
            //LvlLabel.text = "" + selectedTurret.dmgLevel;
        }
    }
}
