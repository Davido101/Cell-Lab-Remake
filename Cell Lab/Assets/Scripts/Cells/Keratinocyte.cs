public class Keratinocyte : Cell
{
    void Awake()
    {
        type = typeof(Keratinocyte);
        shader = LoadShader("Cells/Materials/KeratinocyteMaterial");
        reactsToFood = false;
    }
}