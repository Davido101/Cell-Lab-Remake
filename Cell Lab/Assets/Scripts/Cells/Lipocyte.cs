using UnityEngine;

public class Lipocyte : Cell
{
    public float lipids = 13.68f;

    void Awake()
    {
        type = typeof(Lipocyte);
        shader = LoadShader("Cells/Materials/LipocyteMaterial");
        reactsToFood = false;
        mass = 3.6f;
        radius = Mathf.Sqrt(3.6f)*8;
    }

    public override void fixedupdate(float dt)
    {
        //shader.SetFloat("lipids", lipids);
        HandlePhysics(dt);
    }

    public override float Eat(float amount)
    {
        float eaten = Mathf.Min(amount, lipids);
        lipids -= eaten;
        return eaten;
    }
}
