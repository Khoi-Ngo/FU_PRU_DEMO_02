using System;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public UnityEvent<int, int> healthChanged;
    public UnityEvent<int, Vector2> damageableHit;
    public UnityEvent<GameObject, int> characterDamaged; // Event for character damaged
    public UnityEvent<GameObject, int> characterHealed; // Event for character healed
    [SerializeField] private bool isInvicible = false;
    private Animator animator;
    [SerializeField] public int _maxHealth;
    [SerializeField] private bool _isAlive = true;
    private float lowestToDie = -45f;
    public bool IsAlive
    {
        get { return _isAlive; }
        private set
        {
            _isAlive = value;
            animator.SetBool("isAlive", value);
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        Health = MaxHealth; // Initialize health at the start
    }

    public int MaxHealth
    {
        get { return _maxHealth; }
        set { _maxHealth = value; }
    }

    public int _health;
    private float timeSinceHit = 0;
    public float invicibilityTime = 0.25f;

    public int Health
    {
        get { return _health; }
        set
        {
            _health = value;
            healthChanged?.Invoke(_health, MaxHealth);
            // If health is less than or equal to 0, character is no longer alive
            if (_health <= 0)
            {
                IsAlive = false;
            }
        }
    }

    private void Update()
    {
        if (isInvicible)
        {
            if (timeSinceHit > invicibilityTime)
            {
                // Remove invincibility after the invincibility time
                isInvicible = false;
                timeSinceHit = 0;
            }
            timeSinceHit += Time.deltaTime;
        }
        if (transform.position.y <= lowestToDie && IsAlive)
        {
            IsAlive = false;
        }
    }

    public bool Hit(int damage, Vector2 knockback)
    {
        if (IsAlive && !isInvicible)
        {
            Health -= damage;
            isInvicible = true; // Start invincibility after being hit
            animator.SetTrigger("hit");
            damageableHit?.Invoke(damage, knockback);


            //Trigger to show the DAMAGE _TEXT NEAR THE CHAR
            CharEvents.characterDamaged?.Invoke(gameObject, damage);

            return true;
        }
        return false;
    }

    public void Heal(int healthRestore)
    {
        if (IsAlive)
        {
            int temp = Health;
            int maxHeal = Mathf.Abs(MaxHealth - Health);
            Health = maxHeal < healthRestore ? MaxHealth : Health += healthRestore;

            CharEvents.characterHealed?.Invoke(gameObject, Health - temp);

        }
    }
}
