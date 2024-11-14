using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI currencyUI;
    [SerializeField] private TextMeshProUGUI EnemyUI;
    [SerializeField] private Animator leftAnim;
    [SerializeField] private Animator rightAnim;
    [SerializeField] private Button menuButton;

    private float delayTime = 5f;
    private float startTime;


    private bool isMenuOpen = true;

    private void Start()
    {
        menuButton.onClick.AddListener(ToggleMenu);
        startTime = Time.time;
    }
    //This is for the menu animation
    public void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;
        rightAnim.SetBool("MenuOpen", isMenuOpen);
        leftAnim.SetBool("MenuOpen", isMenuOpen);
    }

    private void OnGUI()
    {
        currencyUI.text = LevelManager.main.GetCurrency().ToString();
        if (Time.time >= startTime + delayTime)
        {
            EnemyUI.text = "Enemies Left: " + LevelManager.main.GetEnemiesLeft().ToString();
        }
    }

}
