using HealthModule;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private Slider _slider;

    private IHealth _healthModel;
    
    public void Initialize(IHealth healthModel, int maxHealthValue)
    {
        _healthModel = healthModel;
        _slider.maxValue = maxHealthValue;
    }

    void Update()
    {
        if (_healthModel == null)
            return;

        _slider.value = _healthModel.Health;
    }
}