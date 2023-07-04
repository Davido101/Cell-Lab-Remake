using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Cell : MonoBehaviour
{
    public bool optimizedInteractions = true;

    public int gridID;

    public float springConst = 200;
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
        setup();
    }

    public void setup()
    {
        if (KillIfOutsideSubstrate()) return;
        gameObject.transform.localScale = new Vector3(radius, radius, 1);
        color.a = opacity;
        gameObject.GetComponent<SpriteRenderer>().color = color;
    }

    public void update()
    {
        handleGraphics();
    }

    public void handleGraphics()
    {
        if (dead)
        {
            GetComponent<Renderer>().enabled = false;
            return;
        }

        moveDuration += Time.deltaTime;
        float lerping = Mathf.Min(moveDuration / Time.fixedDeltaTime, 1);
        float x = Mathf.Lerp(lastPosition.x, position.x, lerping);
        float y = Mathf.Lerp(lastPosition.y, position.y, lerping);
        gameObject.transform.position = new Vector3(x, y, 0);
        gameObject.transform.localScale = new Vector3(radius, radius, 1);
    }

    public void fixedupdate(float deltaT)
    {
        handlePhysics(deltaT);
    }

    public void handlePhysics(float deltaT)
    {
        force += SubstrateReactionForce();
        UpdatePos(deltaT);
        if (KillIfOutsideSubstrate()) return;
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
        return ReactionForce(position, position.magnitude + radius - substrate.radius);
    }

    public Vector2 ReactionForce(Cell cell)
    {
        // manual implementation is somehow noticeably faster
        Vector2 relativePos = new Vector2(cell.position.x - position.x, cell.position.y - position.y);
        if (relativePos.sqrMagnitude >= Mathf.Pow(radius + cell.radius, 2)) return Vector2.zero;
        float collision = radius + cell.radius - relativePos.magnitude;
        return ReactionForce(relativePos, collision);
    }

    public Vector2 ReactionForce(Vector2 pos, float collision)
    {
        if (collision <= 0) return Vector2.zero;
        return -pos.normalized * collision * springConst;
    }

    public void Kill()
    {
        dead = true;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
