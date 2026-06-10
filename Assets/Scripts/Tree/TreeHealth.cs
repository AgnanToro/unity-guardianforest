using UnityEngine;

public class TreeHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int CurrentHealth { get; private set; }
    public bool IsDead { get; private set; }

    void Start()
    {
        CurrentHealth = maxHealth;
        IsDead = false;
    }

    public bool TakeDamage(int damage)
    {
        if (IsDead) return true;

        CurrentHealth -= damage;
        CurrentHealth = Mathf.Max(CurrentHealth, 0);

        Debug.Log(gameObject.name + " HP: " + CurrentHealth);

        if (CurrentHealth <= 0)
        {
            IsDead = true;

            if (ForestManager.Instance != null)
                ForestManager.Instance.TreeDestroyed();

            Destroy(gameObject);
            return true;
        }

        return false;
    }
}