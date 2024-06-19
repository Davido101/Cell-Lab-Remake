using UnityEngine;

public class Flagellocyte : Cell
{
    public float swimForce = 20865.6f;

    void Awake()
    {
        type = typeof(Flagellocyte);
        shader = LoadShader("Cells/Materials/FlagellocyteMaterial");
    }

    public override void fixedupdate(float dt)
    {
        shader.SetFloat("speed", swimForce * 100);
        shader.SetFloat("age", age);
        force += new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * swimForce;
        HandlePhysics(dt);
    }
}
