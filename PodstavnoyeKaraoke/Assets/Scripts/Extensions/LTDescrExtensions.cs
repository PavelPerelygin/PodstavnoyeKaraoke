namespace Extensions
{
    public static class LTDescrExtensions
    {
        public static float GetMaximumExecutionTime(params LTDescr[] LTDescr)
        {
            float maxTime = 0f;

            for (int i = 0; i < LTDescr.Length; i++)
            {
                float time = LTDescr[i].time + LTDescr[i].delay;
                if (time > maxTime)
                    maxTime = time;
            }

            return maxTime;
        }
    }
}