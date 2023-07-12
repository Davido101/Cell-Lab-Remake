using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Devorocyte : Cell
{
    float devoringRate = 1;

    private void Awake()
    {
        type = typeof(Devorocyte);
        sprite = LoadSvgResource("Cells/devorocyte");
    }

    public override void React(Cell cell, float dt)
    {
        if (dead || cell.dead) return;
        float eaten = cell.Eat(devoringRate * dt);
        Grow(eaten);
        force += ReactionForce(cell);
    }
}
