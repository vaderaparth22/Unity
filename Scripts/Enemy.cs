using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float forceToPlayer;
    public float forceToRandom;

    Rigidbody2D rb;
    Collider2D myCollider;
    Vector2 targetVector;
    SpriteRenderer spriteRenderer;
    bool isInsideWorld;
    bool isDanger;

    public void Initialize(bool isDanger)
    {
        rb = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        myCollider.enabled = false;
        this.isDanger = isDanger;
    }

    public void AddForceTowardsTarget()
    {
        targetVector = new Vector2(Random.Range(-MainFlow.Instance.worldWidth, MainFlow.Instance.worldWidth),
                        Random.Range(-MainFlow.Instance.worldHeight, MainFlow.Instance.worldHeight));
        rb.AddForce((targetVector - (Vector2)transform.position).normalized * Random.Range(3f,5f), ForceMode2D.Impulse);
    }

    public void RefreshUpdate()
    {
        if(!isInsideWorld)
        {
            if (transform.position.y < (MainFlow.Instance.worldHeight - 1f))
            {
                myCollider.enabled = true;
                isInsideWorld = true;
            }
        }
    }

    void GoTowardsPlayer()
    {
        Vector2 playerPosition = new Vector2(MainFlow.Instance.playerBall.transform.position.x, MainFlow.Instance.playerBall.transform.position.y);
        Vector2 direction = (playerPosition - (Vector2)transform.position).normalized;
        Vector2 vectorNearPlayer = new Vector2(Random.Range(direction.x - 0.3f, direction.x + 0.3f), 
                                    Random.Range(direction.y - 0.3f, direction.y + 0.3f));
        rb.AddForce(vectorNearPlayer * forceToPlayer, ForceMode2D.Impulse);
    }

    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D otherCollider = collision.collider;

        if(otherCollider.CompareTag("Player"))
        {
            MainFlow.Instance.PlayExplosionAt(collision.transform.position);
            MainFlow.Instance.soundManager.PlaySafeHitSound();

            SpawnManager.Instance.RemoveEnemy(this);

            if(!isDanger)
            {
                gameObject.SetActive(false);
            }
        }
        else if (otherCollider.CompareTag("Danger") || otherCollider.CompareTag("Safe"))
        {
            MainFlow.Instance.soundManager.PlayEnemiesHitSound();
        }
        else
        {
            if(otherCollider.CompareTag("Wall") && isDanger)
            {
                GoTowardsPlayer();
            }
            else
            {
                Vector2 normal = collision.contacts[0].normal;
                rb.AddForce(normal * forceToRandom, ForceMode2D.Impulse);
            }
        }
    }
}
