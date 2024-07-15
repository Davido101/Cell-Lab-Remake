using UnityEngine;

public class Flagellocyte : Cell
{
    public float swimForce = 2608.2f/10f;// 20865.6f;

    void Awake()
    {
        type = typeof(Flagellocyte);
        shader = LoadShader("Cells/Materials/FlagellocyteMaterial");
    }

    public override void fixedupdate(float dt)
    {
        shader.SetFloat("speed", swimForce / 50);
        shader.SetFloat("age", age % Mathf.PI);
        force += new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * swimForce;
        HandlePhysics(dt);
    }
}
