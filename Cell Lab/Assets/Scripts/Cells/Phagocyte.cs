public class Phagocyte : Cell
{
    void Awake()
    {
        type = typeof(Phagocyte);
        sprite = LoadSvgResource("Cells/phagocyte");
        reactsToFood = true;
    }
}
