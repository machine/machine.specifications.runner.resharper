using System;
using JetBrains.Application.Icons.CompiledIconsCs;
using JetBrains.UI.Icons;

namespace Machine.Specifications.Runner.ReSharper.Resources
{
    public static class MspecThemedIcons
    {
        [CompiledIconCs]
        public sealed class MSpec : CompiledIconCsClass
        {
            public static IconId Id = new CompiledIconCsId(typeof(MSpec));

            public override CompiledIconCsIdOwner.ThemedIconThemeImage[] GetThemeImages()
            {
                return Array.Empty<CompiledIconCsIdOwner.ThemedIconThemeImage>();
            }
        }
    }
}
