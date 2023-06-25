using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

public class Cell : MonoBehaviour
{
    const float springConst = 2000;
    public GameObject cellObject;
    public Rigidbody2D rb;
    public Substrate substrate;
    public float radius = 30;
    public bool dead = false;
    // Start is called before the first frame update
    void Start()
    {
        cellObject = this.gameObject;
        cellObject.transform.localScale = new Vector3(radius, radius, 1);
        rb = cellObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.velocity = new Vector2(0.1f, 0.1f);
        cellObject.GetComponent<SpriteRenderer>().color = (Color)(new Color32(179, 255, 57, 255));
    }

    public void update()
    {
        cellObject.transform.localScale = new Vector3(radius, radius, 1);
        Vector2 position = GetPos();
        float length = position.magnitude;
        if (length >= 1)
        {
            Kill();
            return;
        }
    }

    public void fixedupdate()
    {
        Vector2 position = GetPos();
        float r = radius / 1000;
        float length = position.magnitude;
        float substrateCollision = length + r - 1;
        if (substrateCollision > 0)
        {
            Vector2 force = -position / length * substrateCollision * springConst;
            //force.x = Mathf.Round(force.x);
            //force.y = Mathf.Round(force.y);
            //Debug.Log(force);
            rb.AddRelativeForce(force);
        }
    }

    public Vector2 GetPos()
    {
        return this.gameObject.transform.position;
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

