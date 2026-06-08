using HealthModule;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    private ISpendHealth healthModel;

    public void Initialize(ISpendHealth health, int maxHealth)
    {
        healthModel = health;
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
    }

    void Update()
    {
        if (healthModel == null) return;
        slider.value = healthModel.Health;
    }
}