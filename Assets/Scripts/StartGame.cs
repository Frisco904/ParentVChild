using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    [SerializeField] public AK.Wwise.Switch Menu;
    [SerializeField] public AK.Wwise.Event MusicStart;

    void Start() {
        // Wwise call to start music.
        Menu.SetValue(gameObject);
        MusicStart.Post(gameObject);
    }
    public void StartButtonClick()
    {
        SceneManager.LoadScene(1);
    }
}
