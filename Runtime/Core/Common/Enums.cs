namespace PBBox
{
    [System.Flags]
    public enum Direction4
    {
        None = 0,
        Up = 1 << 0,
        Left = 1 << 1,
        Down = 1 << 2,
        Right = 1 << 3
    }
}