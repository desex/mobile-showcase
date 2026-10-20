#if VCONTAINER_SUPPORT

using System;
using System.IO;
using VContainer;
using VContainer.Unity;

namespace Sotbifay.Images
{
    public static class VContainerExtensions
    {
        public static void RegisterImages(this IContainerBuilder builder)
        {
            if(builder is null)
                throw new ArgumentNullException(nameof(builder));
            
            builder.RegisterEntryPoint<ImageCache>()
                .As<IImageCache>();
            
            builder.RegisterEntryPoint<ImagePool>()
                .As<IImagePool>();
            
            builder.RegisterFactory<Uri, IImageEntry>(resolver =>
            {
                var imagePool = resolver.Resolve<IImagePool>();
                return url => new UrlImageEntry(url, imagePool);
            }, Lifetime.Singleton);
            
            builder.RegisterFactory<FileInfo, IImageEntry>(resolver =>
            {
                var imagePool = resolver.Resolve<IImagePool>();
                return file => new FileInfoImageEntry(file, imagePool);
            }, Lifetime.Singleton);
        }
    }
}

#endif
