using UnityEngine;
using UnityEngine.UI;
using HealthModule;

public class HealthBarSetup : MonoBehaviour
{
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private int maxHealth = 100;

    void Start()
    {
        if (healthBar != null)
        {
            healthBar.Initialize(GetComponent<ISpendHealth>(), maxHealth);
        }

    }
}