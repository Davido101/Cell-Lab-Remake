using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Cell : MonoBehaviour
{
    public float springConst = 200;
    public GameObject cellObject;
    public Substrate substrate;
    public Type cellType = typeof(Cell);
    public Vector2 position = Vector2.zero;
    public Vector2 lastPosition = Vector2.zero;
    public Vector2 velocity = Vector2.zero;
    public Vector2 force = Vector2.zero;
    public float moveDuration = 0;
    public float mass = 2.16f;
    public float radius = Mathf.Sqrt(2.16f/16000);
    public bool dead = false;
    const float opacity = 0.8f;
    public Color color = Color.white;
    
    void Start()
    {
        cellObject = this.gameObject;
        cellObject.transform.localScale = new Vector3(radius, radius, 1);
        color.a = opacity;
        cellObject.GetComponent<SpriteRenderer>().color = color;
    }

    public void update()
    {
        moveDuration += Time.deltaTime;
        float lerping = Mathf.Min(moveDuration / Time.fixedDeltaTime, 1);
        float x = Mathf.Lerp(lastPosition.x, position.x, lerping);
        float y = Mathf.Lerp(lastPosition.y, position.y, lerping);
        cellObject.transform.position = new Vector3(x, y, 0);
        cellObject.transform.localScale = new Vector3(radius, radius, 1);
    }

    public void fixedupdate(float deltaT)
    {
        if (KillIfOutsideSubstrate()) return;
        force += SubstrateReactionForce();
        UpdatePos(deltaT);
    }

    public void React(Cell cell)
    {
        force += ReactionForce(cell);
    }

    public void UpdatePos(float deltaT)
    {
        lastPosition = position;
        velocity += force / mass * deltaT;
        position += velocity * deltaT;
        force = Vector2.zero;
        moveDuration = 0;
    }

    public Boolean KillIfOutsideSubstrate()
    {
        if (position.magnitude >= substrate.radius)
        {
            Kill();
            return true;
        }
        return false;
    }

    public Vector2 SubstrateReactionForce()
    {
        return this.ReactionForce(position, position.magnitude + radius - substrate.radius);
    }

    public Vector2 ReactionForce(Cell cell)
    {
        Vector2 relativePos = cell.position - this.position;
        float collision = this.radius + cell.radius - relativePos.magnitude;
        return this.ReactionForce(relativePos, collision);
    }

    public Vector2 ReactionForce(Vector2 pos, float collision)
    {
        return -pos / pos.magnitude * Mathf.Max(collision, 0) * springConst;
    }

    public void Kill()
    {
        dead = true;
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }
}

