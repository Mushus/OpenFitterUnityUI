#nullable enable
using System;
using UnityEngine.UIElements;

namespace OpenFitter.Editor.Views
{
    /// <summary>
    /// Implementation of ISetupItemView using UI Toolkit.
    /// </summary>
    public sealed class SetupItemView : ISetupItemView
    {
        private readonly Label lblTitle;
        private readonly Label lblStatus;
        private readonly Button btnAction;
        private readonly Button btnWebsite;
        private readonly Button btnMore;

        public event Action? OnActionClicked;
        public event Action? OnMoreClicked;
        public event Action? OnWebsiteClicked;

        public SetupItemView(VisualElement root)
        {
            lblTitle = root.Q<Label>("lbl-title")!;
            lblStatus = root.Q<Label>("lbl-status")!;
            btnAction = root.Q<Button>("btn-action")!;
            btnWebsite = root.Q<Button>("btn-website")!;
            btnMore = root.Q<Button>("btn-more")!;

            btnAction.clicked += () => OnActionClicked?.Invoke();
            btnWebsite.clicked += () => OnWebsiteClicked?.Invoke();
            btnMore.clicked += () => OnMoreClicked?.Invoke();
        }

        public void SetTitle(string title) => lblTitle.text = title;

        public void SetStatus(string status, string className)
        {
            lblStatus.text = status;
            lblStatus.ClearClassList();
            lblStatus.AddToClassList(className);
        }

        public void SetActionButtonLabel(string label) => btnAction.text = label;

        public void SetActionButtonEnabled(bool enabled) => btnAction.SetEnabled(enabled);

        public void SetMoreButtonVisible(bool visible) => btnMore.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }
}
