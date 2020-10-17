namespace n0tFlix.Subtitles.TheSubDB
{
    public interface ILanguage
    {
        int Count { get; }
        string Name { get; }

        string ToString();
    }
}
