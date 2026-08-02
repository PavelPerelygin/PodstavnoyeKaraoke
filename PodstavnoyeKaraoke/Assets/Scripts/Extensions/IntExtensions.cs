namespace Extensions
{
    public static class IntExtensions
    {
        public static int Limiter(this int target, int limit)
        {
            int result = target;
            
            if (limit < 0)
            {
                if (result < limit)
                    result = limit;
            }else if (limit > 0)
            {
                if (result > limit)
                    result = limit;
            }
            else
            {
                result = 0;
            }

            return result;
        }
    }
}