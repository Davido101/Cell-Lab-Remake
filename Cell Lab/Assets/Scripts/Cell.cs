using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

public class Cell : MonoBehaviour
{
    const float springConst = 100;
    public GameObject cellObject;
    public Rigidbody2D rb;
    public Substrate substrate;
    public Vector2 position = Vector2.zero;
    public Vector2 velocity = new Vector2(0.05f,0.05f);
    public Vector2 force = Vector2.zero;
    public float radius = 30;
    public bool dead = false;
    
    void Start()
    {
        cellObject = this.gameObject;
        cellObject.transform.localScale = new Vector3(radius, radius, 1);
        rb = cellObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        cellObject.GetComponent<SpriteRenderer>().color = (Color)(new Color32(179, 255, 57, 255));
    }

    public void update()
    {
        cellObject.transform.localScale = new Vector3(radius, radius, 1);
    }

    public void fixedupdate(float deltaT)
    {
        float length = position.magnitude;
        if (length >= 1)
        {
            Kill();
            return;
        }

        float r = radius / 1000;
        float substrateCollision = length + r - 1;
        if (substrateCollision > 0)
        {
            force += -position / length * substrateCollision * springConst;
        }

        velocity += force * deltaT;
        position += velocity * deltaT;
        SetPos(position);

        force = Vector2.zero;
    }

    public void react(Cell cell)
    {
        Vector2 relativePos = cell.position - this.position;
        float proximity = relativePos.magnitude;
        float collision = (this.radius + cell.radius) / 1000 - proximity;
        if (collision > 0)
        {
            force += -relativePos / proximity * collision * springConst;
        }
    }

    public void SetPos(Vector2 pos)
    {
        this.gameObject.transform.position = new Vector3(pos.x, pos.y, 0);
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

