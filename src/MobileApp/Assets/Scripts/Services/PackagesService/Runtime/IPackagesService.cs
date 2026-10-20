namespace Services.PackagesService
{
    public interface IPackagesService
    {
        public long GetPackageVersionCode(string packageName);
    }
}