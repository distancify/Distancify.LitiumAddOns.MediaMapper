using Distancify.LitiumAddOns.MediaMapper.Services;
using Litium.FieldFramework;
using Litium.Owin.InversionOfControl;
using Litium.Products;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Distancify.LitiumAddOns.MediaMapper
{
    public static class IIoCContainerExtensions
    {
        public static void RegisterMediaMapper(this IIoCContainer container, IMediaProfiler mediaProfiler, string uploadFolder)
        {
            container.For<IMediaMapper>()
                    .UsingFactoryMethod(() => new Services.MediaMapper(
                        container.Resolve<BaseProductService>(),
                        container.Resolve<VariantService>(),
                        container.Resolve<FieldDefinitionService>(),
                        container.Resolve<MediaArchive>(),
                        container.ResolveAll<IFieldSetter>(),
                        mediaProfiler,
                        uploadFolder))
                    .RegisterAsSingleton();
        }

    }
}
