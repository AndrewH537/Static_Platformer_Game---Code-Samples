using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{

    public float bulletSpeed;
    public Rigidbody2D theRB;

    public Vector2 moveDir;

    public GameObject impactEffect;

    public int damageAmount = 1;

    // Update is called once per frame
    void Update()
    {
        theRB.velocity = moveDir * bulletSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Enemy")
        {
            other.GetComponent<EnemyHealthController>().DamageEnemy(damageAmount);
        }

        if(other.tag == "Boss")
        {
            BossHealthController.instance.TakeDamage(damageAmount);
        }
        //add effect
        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }

        //Destory bullet if hits an object
        Destroy(gameObject);
    }

    private void OnBecameInvisible()
    {
        //Destroy bullet if goes past screen, avoids memory consumption
        Destroy(gameObject);
    }

}
