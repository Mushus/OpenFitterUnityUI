#nullable enable
using System;

namespace OpenFitter.Editor.Controllers
{
    /// <summary>
    /// Interface that the Root Presenter exposes to the Window.
    /// </summary>
    public interface IRootPresenter : IDisposable
    {
        void OnEnable();
        void OnDisable();
        void RefreshProjectJson();
    }
}
