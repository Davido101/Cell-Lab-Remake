using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public Vector2 position = new Vector2(0, 0);
    public Vector2 velocity = new Vector2(5000000, 0);
    public GameObject cellObject;
    public Rigidbody2D rb;
    public Substrate substrate;
    // Start is called before the first frame update
    void Start()
    {
        cellObject = this.gameObject;
        rb = cellObject.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(0, 0);
        velocity.x = 0;
        velocity.y = 0;
    }

    public void update()
    {
        if (rb.velocity.x <= velocity.x || rb.velocity.y <= velocity.y) //fix later
        {
        }
    }

    public void fixedupdate()
    {
        
    }

    public void Kill()
    {
        Destroy(cellObject);
    }
}
