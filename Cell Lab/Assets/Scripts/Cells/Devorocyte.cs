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
        Consume(cell, dt);
        force += ReactionForce(cell);
    }

    public void Consume(Cell cell, float dt)
    {
        if (dead || cell.dead || !CollidingWith(cell)) return;
        float eaten = cell.Eat(devoringRate * dt);
        Grow(eaten);
    }
}
