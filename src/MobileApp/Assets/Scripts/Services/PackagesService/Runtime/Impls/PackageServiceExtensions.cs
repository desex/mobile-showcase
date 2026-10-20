using System;
using UnityEngine;

namespace Services.PackagesService
{
    public static class PackageServiceExtensions
    {
        public static long GetPackageVersionCode(this IPackagesService packagesService) => 
            packagesService?.GetPackageVersionCode(Application.identifier) ?? throw new ArgumentNullException(nameof(packagesService));
    }
}