using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] private GameObject healthBarParent = null;
    [SerializeField] private Image healthBar = null;
    [SerializeField] private Health health = null;
    void Awake()
    {
        health.ClientHealthUpdatedEvent += onHealthUpdated;
    }

    // Update is called once per frame
    void OnDestroy()
    {
        health.ClientHealthUpdatedEvent -= onHealthUpdated;
    }

    private void onHealthUpdated(int currentHealth, int maxHealth) {
        healthBar.fillAmount = (float)currentHealth / maxHealth;
    }

    private void OnMouseEnter() {
        healthBarParent.SetActive(true);
    }

    private void OnMouseExit() {
        healthBarParent.SetActive(false);
    }
}
