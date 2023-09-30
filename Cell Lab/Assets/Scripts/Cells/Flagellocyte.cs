using UnityEngine;
using UnityEngine.U2D;

public class Flagellocyte : Cell
{
    public float swimForce = 0.1f;

    void Awake()
    {
        type = typeof(Flagellocyte);
        sprite = LoadSvgResource("Cells/flagellocyte");
    }

    public override void fixedupdate(float dt)
    {
        force += new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * swimForce;
        handlePhysics(dt);
    }
}
