using System;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool optimizedInteractions = true;
    public bool reactsToFood = false;

    public int gridID;

    public float springConst = 200;

    public Substrate substrate;

    public Type type = typeof(Cell);

    public Vector2 position = Vector2.zero;
    public Vector2 lastPosition = Vector2.zero;
    public Vector2 velocity = Vector2.zero;
    public Vector2 force = Vector2.zero;
    public float angle = 0;
    public float moveDuration = 0;

    public float mass = 2.88f;
    public float minMass = 0.73f;
    public float maxMass = 3.6f;
    public float radius = Mathf.Sqrt(2.88f)*8;
    public float radiusChangeSpeed = 1;

    public float consumptionRate = 1;
    public float age = 0;
    public bool dead = false;

    const float opacity = 0.8f;
    public Color color = Color.white;
    public Material shader;

    void Awake()
    {
        shader = LoadShader("Cells/Materials/PhagocyteMaterial");
    }

    internal Material LoadShader(string shaderPath)
    {
        return Resources.Load<Material>(shaderPath);
    }

    void Start()
    {
        if (!setup()) return;
    }

    public virtual bool setup()
    {
        if (KillIfOutsideSubstrate()) return false;
        if (!handleGraphics()) return false;
        return true;
    }

    public virtual void update()
    {
        handleGraphics();
    }

    public virtual bool handleGraphics()
    {
        if (dead)
        {
            transform.GetChild(0).GetComponent<Renderer>().enabled = false;
            return false;
        }

        moveDuration += Time.deltaTime;
        float lerping = Mathf.Min(moveDuration / Time.fixedDeltaTime, 1);
        float x = Mathf.Lerp(lastPosition.x, position.x, lerping);
        float y = Mathf.Lerp(lastPosition.y, position.y, lerping);

        radius += (Mathf.Sqrt(mass) * 8 - radius) * radiusChangeSpeed * Time.deltaTime;
        gameObject.transform.position = new Vector3(x, y, 0);
        gameObject.transform.localScale = new Vector3(radius * 2, radius * 2, 1);
        gameObject.transform.eulerAngles = Vector3.forward * angle * Mathf.Rad2Deg; // unity takes in rotation as degrees
        color.a = opacity;

        shader.SetColor("col", color);
        return true;
    }

    public virtual void fixedupdate(float dt)
    {
        HandlePhysics(dt);
    }

    public virtual bool HandlePhysics(float dt)
    {
        age += dt;
        force += SubstrateReactionForce();
        force += DynamicFrictionForce();
        UpdatePos(dt);
        force = Vector2.zero;
        moveDuration = 0;
        if (KillIfOutsideSubstrate()) return false;
        return true;
    }

    public virtual void React(Cell cell, float dt)
    {
        if (dead || cell.dead) return;
        force += ReactionForce(cell);
    }

    public virtual void React(Food food, float dt)
    {
        if (dead || food.eaten) return;
        (Vector2 relativePos, float collision) = CalculateCollision(position, food.position, radius, food.radius);
        if (collision > 0)
        {
            float eaten = food.Eat(consumptionRate * dt);
            Grow(eaten);
        }
    }

    public virtual void UpdatePos(float dt)
    {
        lastPosition = position;
        velocity += force / mass * dt;
        position += velocity * dt;
    }

    public virtual bool KillIfOutsideSubstrate()
    {
        if (position.magnitude >= substrate.radius)
        {
            Kill();
            return true;
        }
        return false;
    }

    public virtual bool CollidingWith(Cell cell)
    {
        float dx = position.x - cell.position.x;
        float dy = position.y - cell.position.y;
        float total_radius = radius + cell.radius;
        return dx * dx + dy * dy <= total_radius * total_radius;
    }

    public virtual (Vector2, float) CalculateCollision(Vector2 pos1, Vector2 pos2, float radius1, float radius2)
    {
        // manual implementation is somehow noticeably faster
        Vector2 relativePos = new Vector2(pos2.x - pos1.x, pos2.y - pos1.y);
        if (relativePos.sqrMagnitude >= Mathf.Pow(radius1 + radius2, 2)) return (Vector2.zero, 0);
        float collision = radius1 + radius2 - relativePos.magnitude;
        return (relativePos, collision);
    }

    public virtual Vector2 SubstrateReactionForce()
    {
        return ReactionForce(position, position.magnitude + radius - substrate.radius);
    }

    public virtual Vector2 ReactionForce(Cell cell)
    {
        (Vector2 relativePos, float collision) = CalculateCollision(position, cell.position, radius, cell.radius);
        return ReactionForce(relativePos, collision);
    }

    public virtual Vector2 ReactionForce(Vector2 relativePos, float collision)
    {
        if (collision <= 0) return Vector2.zero;
        return collision * springConst * -relativePos.normalized;
    }

    public virtual Vector2 DynamicFrictionForce()
    {
        return -velocity * substrate.dynamicFriction;
    }

    public virtual bool Kill()
    {
        if (dead) return false;
        return dead = true;
    }

    public virtual void Destroy()
    {
        Destroy(gameObject);
    }

    public virtual void Grow(float amount)
    {
        mass += amount;
        if (mass > maxMass) mass = maxMass;
    }

    public virtual float Eat(float amount)
    {
        float eaten = Mathf.Min(amount, mass);
        mass -= eaten;
        if (mass < minMass) Kill();
        return eaten;
    }
}
