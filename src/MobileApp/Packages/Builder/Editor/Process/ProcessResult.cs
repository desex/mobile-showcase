namespace Sotbifay.Builder.Editor
{
    public readonly struct ProcessResult
    {
        public ProcessResult(string output, string error)
        {
            Output = output;
            Error = error;
        }
        
        public string Output { get; }
        public string Error { get; }
        
        public bool HasError => string.IsNullOrWhiteSpace(Error) == false;

        public override string ToString() => 
            HasError ? $"[ERROR] {Error}" : Output;
    }
}