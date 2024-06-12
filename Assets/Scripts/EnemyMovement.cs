using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] float enemySpeed = 1.0f;

    Rigidbody2D myRigidbody;
    CircleCollider2D circleCollider;
    BoxCollider2D boxCollider;
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        myRigidbody.velocity = new Vector2(enemySpeed, 0f);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        enemySpeed = -enemySpeed;
        FlipEnemy(); 
    }


    void FlipEnemy()
    {
        transform.localScale = new Vector2(-(Mathf.Sign(myRigidbody.velocity.x)), 1f);    
    }
}
