namespace WageringGG
{
    public static class GetGroupName
    {
        public static string Wager(int id)
        {
            return $"wager_{id}";
        }

        public static string Tournament(int id)
        {
            return $"tournament_{id}";
        }
    }
}
