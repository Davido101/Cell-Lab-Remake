using UnityEngine;

public class Food : MonoBehaviour
{
    public int gridID;

    public Vector2 position = Vector2.zero;
    public float size = 1.2f;
    public float coating = 0;
    public float radius = Mathf.Sqrt(1.2f / 3) / 1000;
    public bool eaten = false;
    public Substrate substrate;

    private void Start()
    {
        gameObject.transform.localScale = new Vector3(radius * 2, radius * 2, 1);
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
