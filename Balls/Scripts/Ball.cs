using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public float minPower;
    public float maxPower;
    public float wallHitPower;
    public float minWallHitPowerMagnitude;
    public float maxLineStretchX;
    public float maxLineStretchY;
    public float maxStretchDistance;

    LineRenderer line;
    Rigidbody2D rb;
    Vector2 startPoint;
    Vector2 endPoint;
    Vector2 dir;
    Vector2 lineEndVector;
    float lastDistance;
    float powerCalculator;
    float squaredDistanceFromstartPos;
    bool canApplyForce;
    int layerMask;

    public void Initialize()
    {
        layerMask = LayerMask.GetMask("Enemy");
        line = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody2D>();
        AddForceTowardsRandomTarget();
    }

    public void Refresh()
    {
        if (Input.GetMouseButtonDown(0))
        {
            line.enabled = true;
            startPoint = GetScreenToWorldPoint();
            lastDistance = squaredDistanceFromstartPos;

            MainFlow.Instance.SlowMotion(true);
        }

        if (Input.GetMouseButton(0))
        {
            SetLineRendererAndForceValue();
            endPoint = GetScreenToWorldPoint();
            dir = (startPoint - endPoint);
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (canApplyForce)
            {
                rb.velocity = Vector2.zero;
                rb.AddForce(dir.normalized * powerCalculator, ForceMode2D.Impulse);
            }

            lastDistance = 0;
            squaredDistanceFromstartPos = 0;
            line.enabled = false;
            canApplyForce = false;

            MainFlow.Instance.SlowMotion(false);
        }
    }

    Vector2 GetScreenToWorldPoint()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void SetLineRendererAndForceValue()
    {
        lineEndVector = (Vector2)transform.position + dir;
        //lineEndVector.x = Mathf.Clamp(lineEndVector.x, -maxLineStretchX, maxLineStretchX);
        //lineEndVector.y = Mathf.Clamp(lineEndVector.y, -maxLineStretchY, maxLineStretchY);

        line.SetPosition(0, transform.position);
        line.SetPosition(1, lineEndVector);

        squaredDistanceFromstartPos = Vector3.SqrMagnitude(dir);

        if (lastDistance != squaredDistanceFromstartPos)
        {
            lastDistance = squaredDistanceFromstartPos;
            powerCalculator = (squaredDistanceFromstartPos * maxPower) / maxStretchDistance;
            powerCalculator = Mathf.Clamp(powerCalculator, minPower, maxPower);
            canApplyForce = true;
        }
    }

    public void AddForceTowardsRandomTarget()
    {
        Vector2 targetVector = new Vector2(Random.Range(-MainFlow.Instance.worldWidth, MainFlow.Instance.worldWidth),
                        Random.Range(-MainFlow.Instance.worldHeight, MainFlow.Instance.worldHeight));
        rb.AddForce((targetVector - (Vector2)transform.position).normalized * Random.Range(2f, 4f), ForceMode2D.Impulse);
    }

    public void BlastEffectToObject()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 50f, layerMask);

        foreach (Collider2D enemy in enemies)
        {
            float distanceFromEnemy = Vector2.Distance(transform.position, enemy.transform.position);
            Vector2 direction = (enemy.transform.position - transform.position).normalized;
            float force = Mathf.Lerp(15f, 0f, distanceFromEnemy/10f);
            enemy.GetComponent<Rigidbody2D>().AddForce(direction * force, ForceMode2D.Impulse);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (rb.velocity.sqrMagnitude < minWallHitPowerMagnitude)
        {
            Vector2 normal = collision.contacts[0].normal;
            rb.AddForce(normal * wallHitPower, ForceMode2D.Impulse);
        }

        Collider2D otherCollider = collision.collider;

        if (otherCollider.CompareTag("Safe"))
        {
            Vector2 normal = collision.contacts[0].normal;
            rb.AddForce(normal * wallHitPower, ForceMode2D.Impulse);
            CameraShaker.Instance.ShakeCamera();
        }
        else if (otherCollider.CompareTag("Wall"))
        {
            MainFlow.Instance.soundManager.PlayWallHitSound();
        }
    }
}
