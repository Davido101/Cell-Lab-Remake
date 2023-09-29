using UnityEngine;

public class Game : MonoBehaviour
{
    public Substrate substrate;
    public UI ui;
    public new Camera camera;

    void Start()
    {
        camera = (Camera)FindObjectOfType(typeof(Camera));
    }

    void Update()
    {
        substrate.update();
    }

    void FixedUpdate()
    {
        substrate.fixedupdate();
    }
}
