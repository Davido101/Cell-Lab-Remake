using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VectorGraphics;
using Unity.VectorGraphics.Editor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Cell : MonoBehaviour
{
    public bool optimizedInteractions = true;
    public bool reactsToFood = false;

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
    public float radiusChangeSpeed = 1;
    public float consumptionRate = 1;
    public bool dead = false;
    const float opacity = 0.8f;
    public Color color = Color.white;
    public Sprite sprite;

    void Awake()
    {
        sprite = LoadSvgResource("Cells/cell");
    }

    void Start()
    {
        setup();
    }

    internal Sprite LoadSvgResource(string resourcePath)
    {
        return Resources.Load<GameObject>(resourcePath).GetComponent<SpriteRenderer>().sprite;
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

        radius += (mass / radius / 16000 - radius) * radiusChangeSpeed * Time.deltaTime;
        gameObject.transform.position = new Vector3(x, y, 0);
        gameObject.transform.localScale = new Vector3(radius, radius, 1);
    }

    public void fixedupdate(float dt)
    {
        handlePhysics(dt);
    }

    public void handlePhysics(float dt)
    {
        force += SubstrateReactionForce();
        UpdatePos(dt);
        if (KillIfOutsideSubstrate()) return;
    }

    public void React(Cell cell, float dt)
    {
        if (dead || cell.dead) return;
        force += ReactionForce(cell);
    }

    public void React(Food food, float dt)
    {
        if (dead || food.eaten) return;
        (Vector2 relativePos, float collision) = CalculateCollision(position, food.position, radius, food.radius);
        if (collision > 0)
        {
            float eaten = food.Eat(consumptionRate * dt);
            Grow(eaten);
        }
    }

    public void UpdatePos(float dt)
    {
        lastPosition = position;
        velocity += force / mass * dt;
        position += velocity * dt;
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
        (Vector2 relativePos, float collision) = CalculateCollision(position, cell.position, radius, cell.radius);
        return ReactionForce(relativePos, collision);
    }

    public Vector2 ReactionForce(Vector2 pos, float collision)
    {
        if (collision <= 0) return Vector2.zero;
        return -pos.normalized * collision * springConst;
    }

    public (Vector2, float) CalculateCollision(Vector2 pos1, Vector2 pos2, float radius1, float radius2)
    {
        // manual implementation is somehow noticeably faster
        Vector2 relativePos = new Vector2(pos2.x - pos1.x, pos2.y - pos1.y);
        if (relativePos.sqrMagnitude >= Mathf.Pow(radius1 + radius2, 2)) return (Vector2.zero, 0);
        float collision = radius1 + radius2 - relativePos.magnitude;
        return (relativePos, collision);
    }

    public bool Kill()
    {
        if (dead) return false;
        return dead = true;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void Grow(float amount)
    {
        mass += amount;
    }
}
