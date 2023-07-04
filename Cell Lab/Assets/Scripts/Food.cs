using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public GameObject foodObject;
    public Vector2 position = Vector2.zero;
    public float size = 1.2f;
    public float coating = 0;
    public float radius = Mathf.Sqrt(1.2f / 3) / 1000;
    public Substrate substrate;

    private void Start()
    {
        foodObject = gameObject;
        foodObject.transform.localScale = new Vector3(radius, radius, 1);
    }

    public void fixedupdate()
    {
        radius = Mathf.Sqrt(size / 3) / 1000;
    }
}
