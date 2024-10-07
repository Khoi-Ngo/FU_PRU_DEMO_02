using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{

    public int attackDamage = 10;
    public Damageable playerDamageable;
    public Vector2 knockback = Vector2.zero;

    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        if (playerDamageable == null)
        {
            playerDamageable = GameObject.FindWithTag("Player").GetComponent<Damageable>();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the player is alive before attacking
        //TODO: Check any bad effect for attacking of player -> enemies
        if (playerDamageable != null && !playerDamageable.IsAlive)
        {
            Debug.Log("Player is dead. No more attacks.");

            return;
        }

        Damageable damageable = collision.GetComponent<Damageable>();
        if (damageable != null)
        {
            // HIT THE TARGET
            bool gotHit = damageable.Hit(attackDamage, knockback);

        }
    }
}
