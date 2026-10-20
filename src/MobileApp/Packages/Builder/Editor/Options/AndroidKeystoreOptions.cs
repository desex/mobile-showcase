namespace Sotbifay.Builder.Editor
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class AndroidKeystoreOptions
    {
        [Option("androidKeystoreName")]
        public string KeyStoreName { get; set; } = string.Empty;
        
        [Option("androidKeystorePassword")]
        public string KeyStorePassword { get; set; } = string.Empty;
        
        [Option("androidAliasName")]
        public string AliasName { get; set; } = string.Empty;
        
        [Option("androidAliasPassword")]
        public string AliasPassword { get; set; } = string.Empty;
    }
}