using UnityEngine;
using UnityEngine.UI;

public class EnemyFloatingFeedMeter : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private int sliderValue;

    public void UpdateFeedMeter(float currentValue, float maxValue)
    {
        slider.value = currentValue / maxValue;

    }
    public void Awake()
    {
        slider.value = sliderValue;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation= Camera.main.transform.rotation;
        transform.position = target.position + offset;
    }
}
