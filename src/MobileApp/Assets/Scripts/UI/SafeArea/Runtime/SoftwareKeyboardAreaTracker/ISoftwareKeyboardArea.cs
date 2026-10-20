using System;

namespace Sotbify.MobileApp.UI.SafeArea
{
    public interface ISoftwareKeyboardArea : IDisposable
    {
        int GetHeight(bool includeInput = false);
    }
}