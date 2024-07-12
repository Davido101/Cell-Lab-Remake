using UnityEngine;
using UnityEngine.UI;

public class Darken : MonoBehaviour
{
    public static Image darken;
    static float alpha;
    static public Darken instance;
    public float speed;

    void Awake()
    {
        instance = this;
        darken = this.GetComponent<Image>();
    }

    public static void Enable()
    {
        darken.raycastTarget = true;
        alpha = 0.686f;
    }

    public static void Disable()
    {
        darken.raycastTarget = false;
        alpha = 0;
    }

    public void Update()
    {
        darken.color = new Color(0, 0, 0, Mathf.Lerp(darken.color.a, alpha, 0.5f * Time.deltaTime * speed));
    }
}
