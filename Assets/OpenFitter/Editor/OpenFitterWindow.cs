using UnityEditor;
using UnityEngine;
using OpenFitter.Editor.Controllers;
using OpenFitter.Editor.Views;

namespace OpenFitter.Editor
{
    public sealed class OpenFitterWindow : EditorWindow
    {
        private OpenFitterRootPresenter? rootPresenter;

        [MenuItem("Tools/OpenFitter")]
        public static void ShowWindow()
        {
            var window = GetWindow<OpenFitterWindow>(I18n.Tr("OpenFitter"));
            window.minSize = new Vector2(500f, 600f);
        }

        private void OnEnable() => rootPresenter?.OnEnable();

        private void OnDisable() => rootPresenter?.OnDisable();
        private void OnDestroy() => rootPresenter?.Dispose();

        public void CreateGUI()
        {
            var rootView = new OpenFitterRootView(rootVisualElement);
            rootPresenter = new OpenFitterRootPresenter(rootView);
            rootPresenter.OnEnable();
        }

        public void RefreshProjectJson() => rootPresenter?.RefreshProjectJson();
    }
}
