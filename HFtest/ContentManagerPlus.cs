using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Text;

namespace ComputingProjectHF
{
    public class ContentManagerPlus : ContentManager
    {
        public ContentManagerPlus(IServiceProvider serviceProvider,
        string RootDirectory) : base(serviceProvider, RootDirectory)
        {
        }

        public T LoadContentExclusive<T>(string AssetName)
        {
            return ReadAsset<T>(AssetName, null);
        }

        public void Unload(IDisposable ContentItem)
        {
            ContentItem.Dispose();
        }
    }
}
