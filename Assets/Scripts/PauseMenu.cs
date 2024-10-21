using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public bool gameIsPaused = false;

    [Header("Mixer Snapshots")]
    [SerializeField] public AudioMixerSnapshot pausedMixer;
    [SerializeField] public AudioMixerSnapshot unpausedMixer;

    [Header("UI Components")]
    [SerializeField] public GameObject pauseMenuUI;
    [SerializeField] public GameObject settingsMenuUI;
    [SerializeField] private GameObject victoryScreenUI;
    [SerializeField] private GameObject defeatScreenUI;

    
    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;

    private AudioMixer mixer;
    
    private void Awake()
    {
         mixer = Resources.Load("MainMix") as AudioMixer;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (gameIsPaused)
            {
                Resume();
            } else {
                Pause();
            }
        }  

        if (gameIsPaused)
        {
            // mixer.SetFloat("volume", volume);
        }
    }

    public void Resume()
    {
        unpausedMixer.TransitionTo(.01f);
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    void Pause()
    {
        pausedMixer.TransitionTo(.01f);
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
    }

    public void Settings()
    {
       settingsMenuUI.SetActive(!settingsMenuUI.activeSelf);
       pauseMenuUI.SetActive(!pauseMenuUI.activeSelf);
    }
    public void Victory()
    {
        victoryScreenUI.SetActive(true);
        //pauseMenuUI.SetActive((!pauseMenuUI.activeSelf));
    }
    public void Defeat()
    {
        defeatScreenUI.SetActive(true);

    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void QuitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }
}
