public class Photocyte : Cell
{
    public float photosynthesisRate = 1;

    void Awake()
    {
        type = typeof(Photocyte);
        shader = LoadShader("Cells/Materials/PhotocyteMaterial");
    }

    public override void fixedupdate(float dt)
    {
        ConsumeLight(dt);
        HandlePhysics(dt);
    }

    public void ConsumeLight(float dt)
    {
        Grow(substrate.LightAmountAt(position.x, position.y) * photosynthesisRate * dt);
    }
}
