namespace Coffee.UIEffects
{
    /// <summary>
    /// Color effect mode.
    /// </summary>
    public enum ColorMode
    {
        Fill = 1,
        Add = 2,
    }

    public static class ColorModeExtensions
    {
        public static string GetKeyword(this ColorMode mode)
        {
            return mode switch
            {
                ColorMode.Fill => "FILL",
                ColorMode.Add => "ADD",
                _ => null
            };
        }
    }
}