using TMPro;
using UnityEngine;

public class SlideMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI currencyUI;
    [SerializeField] private TextMeshProUGUI EnemyUI;
    [SerializeField] private TextMeshProUGUI DmgLabel;
    [SerializeField] private TextMeshProUGUI SpdLabel;
    [SerializeField] private TextMeshProUGUI SprtLabel;
    [SerializeField] private TextMeshProUGUI CtrlLabel;

    [SerializeField] private Animator anim;

    private float delayTime = 5f;
    private float startTime;
    private Turret selectedTurret;

    private bool isMenuOpen = true;

    private void Start()
    {
        startTime = Time.time;
        ToggleMenu();

    }

    //This is for the menu animation
    public void ToggleMenu()
    {
        if (isMenuOpen) anim.Play("MenuClose"); else anim.Play("MenuOpen");
        isMenuOpen = !isMenuOpen;

        // Check the Level manager for the selected turret every time the menu is toggled
        selectedTurret = LevelManager.main.selectedTurret;
    }

    public void UpgradeDmg()
    {
        selectedTurret.UpgradeDmg();
    }

    public void UpgradeCtrl()
    {
        selectedTurret.UpgradeCtrl();
    }

    public void UpgradeSpd()
    {
        selectedTurret.UpgradeSpd();
    }

    public void UpgradeSprt()
    {
        selectedTurret.UpgradeSprt();
    }
    
    public void SellTurret() {
       selectedTurret.SellTurret();
    }
    private void OnGUI()
    {
        currencyUI.text = "$" + LevelManager.main.GetCurrency().ToString();
        if (Time.time >= startTime + delayTime)
        {
            EnemyUI.text = "Enemies Left: " + LevelManager.main.GetEnemiesLeft().ToString();
        }
        if (selectedTurret != null)
        {
            DmgLabel.text = "" + selectedTurret.dmgLevel;
            SpdLabel.text = "" + selectedTurret.spdLevel;
            CtrlLabel.text = "" + selectedTurret.ctrlLevel;
            SprtLabel.text = "" + selectedTurret.sprtLevel;
        }
    }
}
