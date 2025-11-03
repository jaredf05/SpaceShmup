using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(BlinkColorOnHit))]
public class EnemyShield : MonoBehaviour
{
    [Header("Inscribed")]
    public float health = 10;

    private List<EnemyShield> protectors = new List<EnemyShield>();
    private BlinkColorOnHit blinker;

    void Start()
    {
        blinker = GetComponent<BlinkColorOnHit>();
        blinker.ignoreOnCollisionEnter = true;

        if (transform.parent == null) return;
        EnemyShield shieldParent = transform.parent.GetComponent<EnemyShield>();
        if (shieldParent != null)
        {
            shieldParent.AddProtector(this);
        }
    }

    public void AddProtector(EnemyShield shieldChild)
    {
        protectors.Add(shieldChild);
    }


    public bool isActive
    {
        get { return gameObject.activeInHierarchy; }
        private set { gameObject.SetActive(value); }
    }

    public float TakeDamage (float dmg)
    {
        // Can we pass damage to a protector EnemyShield?
        foreach (EnemyShield es in protectors)
        {
            if (es.isActive)
            {
                Debug.Log($"{name} passing {dmg} dmg to protector {es.name}"); //tmp
                dmg = es.TakeDamage(dmg);
                if (dmg == 0)
                {
                    Debug.Log($"{name} protector {es.name} absorbed all damage"); //tmp
                    return 0;
                }
            }
        }
        Debug.Log($"{name} taking {dmg} damage. Current health: {health}"); //tmp
        // If code gets here, then tis EnemyShield will blink and take damage
        // Make blinker blink
        blinker.SetColors();

        health -= dmg;
        if (health <= 0)
        {
            Debug.Log($"{name} broke! Remaining dmg: {-health}"); //tmp
            // Deactivate this EnemyShield GameObject
            isActive = false;
            // Return any damage that was not absorbed by this EnemyShield
            return -health;
        }
        Debug.Log($"{name} remaining health: {health}"); //tmp
        return 0;
    }

}
