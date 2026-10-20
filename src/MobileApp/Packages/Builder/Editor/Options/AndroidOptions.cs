namespace Sotbifay.Builder.Editor
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class AndroidOptions
    {
        [Option("splitApplicationBinary")]
        public bool SplitApplicationBinary { get; set; } = false;
    }
}