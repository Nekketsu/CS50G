namespace BreakoutGame
{
    public class ScoreData
    {
        public string Name { get; }
        public int Score { get; }

        public ScoreData(string name, int score)
        {
            Name = name;
            Score = score;
        }
    }
}
