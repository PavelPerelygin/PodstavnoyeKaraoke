namespace Boards.Base
{
    public class BoardLoadInfo
    {
        public TypeBoard Type { get; set; }
        public float ShowTime { get; set; } = 0f;
        public float ShowDelay { get; set; } = 0f;
        public float HideTime { get; set; } = 0f;
        public float HideDelay { get; set; } = 0f;
    }
}