using Unity.Burst;
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

    public override bool handleGraphics()
    {
        if (!base.handleGraphics())
            return false;

        // set shader here
        return true;
    }

    public override float Eat(float amount)
    {
        float eaten = Mathf.Min(amount, lipids + mass);
        if (eaten > lipids)
        {
            mass -= eaten - lipids;
            lipids = 0;
        }
        else
        {
            lipids -= eaten;
        }
        return eaten;
    }
}
