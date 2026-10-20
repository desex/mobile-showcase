namespace Sotbifay.Builder.Editor
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class AndroidSignOptions
    {
        [Option("signApk")]
        public bool SignApk { get; set; } = true;
    }
}