public class Photocyte : Cell
{
    public float photosynthesisRate = 1;

    void Awake()
    {
        type = typeof(Photocyte);
        sprite = LoadSvgResource("Cells/photocyte");
    }

    public override void fixedupdate(float dt)
    {
        ConsumeLight(dt);
        handlePhysics(dt);
    }

    public void ConsumeLight(float dt)
    {
        Grow(substrate.LightAmountAt(position.x, position.y) * photosynthesisRate * dt);
    }
}
