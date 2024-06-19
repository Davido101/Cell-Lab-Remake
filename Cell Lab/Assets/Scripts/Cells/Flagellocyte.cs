using UnityEngine;
using static UnityEditor.ShaderData;

public class Flagellocyte : Cell
{
    public float swimForce = 0.1f;

    void Awake()
    {
        type = typeof(Flagellocyte);
        shader = LoadShader("Cells/Materials/FlagellocyteMaterial");
    }

    public override void fixedupdate(float dt)
    {
        shader.SetFloat("speed", swimForce * 100);
        force += new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * swimForce;
        handlePhysics(dt);
    }
}
