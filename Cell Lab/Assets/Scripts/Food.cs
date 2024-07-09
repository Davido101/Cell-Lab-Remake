using UnityEngine;

public class Food : MonoBehaviour
{
    public int gridID;

    public Vector2 position = Vector2.zero;
    public float size = 0.1875f;
    public float coating = 0;
    public float radius = Mathf.Sqrt(1.2f / 3) / 1000;
    public bool eaten = false;
    public Substrate substrate;

    private void Start()
    {
        gameObject.transform.localScale = new Vector3(radius * 12800, radius * 12800, 1);
        gameObject.transform.localPosition = position;
    }

    public void fixedupdate(float dt)
    {
        radius = Mathf.Sqrt(size / 3) / 1000;
    }

    public float Eat(float amount)
    {
        if (eaten) return 0;
        float totalEaten = Mathf.Min(size, amount);
        size -= totalEaten;
        if (size <= 0)
        {
            eaten = true;
        }
        return totalEaten;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
