
namespace Extensions
{
    public static class FloatExtensions
    {
        public static float Percent(this float target, float percent)
        {
            float perc = percent;
            if (perc > 1f)
                perc = 1f;
            else if (perc < 0)
                perc = 0f;

            return (target / 100f) * (perc * 100f);
        }
        
        public static float AddIfMore(this float target, float compared)
        {
            float v = 0;
        
            if (target > compared)
                v = compared + (target - compared);
            else
                v = compared;

            return v;
        }
        
    }
}