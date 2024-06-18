public class Phagocyte : Cell
{
    void Awake()
    {
        type = typeof(Phagocyte);
        shader = LoadShader("Cells/Materials/PhagocyteMaterial");
        reactsToFood = true;
    }
}
