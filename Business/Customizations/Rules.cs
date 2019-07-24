namespace Business.Customizations
{
    public static class Rules
    {
        public static bool IsJcpSpecific(int userId)
        {
            return userId == 1;
        }
    }
}
