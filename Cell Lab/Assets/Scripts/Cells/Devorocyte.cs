using System.Collections.Concurrent;
using UnityEngine;

public class Devorocyte : Cell
{
    float devoringRate = 1;

    private void Awake()
    {
        type = typeof(Devorocyte);
        shader = LoadShader("Cells/Materials/DevorocyteMaterial");
    }

    public override void React(Cell cell, float dt)
    {
        if (cell is Lipocyte lipocyte)
        {
            if (lipocyte.lipids == 0)
            {
                Consume(cell, dt);
            } else 
            {
                float eaten = lipocyte.Eat(devoringRate * dt);
                Grow(eaten);
            }
        }
        else if (cell is not Keratinocyte) Consume(cell, dt);
        force += ReactionForce(cell);
    }

    public void Consume(Cell cell, float dt)
    {
        if (dead || cell.dead || !CollidingWith(cell)) return;
        float eaten = cell.Eat(devoringRate * dt);
        Grow(eaten);
    }
}
