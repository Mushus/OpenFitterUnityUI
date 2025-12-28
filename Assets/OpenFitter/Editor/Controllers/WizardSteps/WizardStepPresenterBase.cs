#nullable enable
using UnityEditor;
using UnityEngine.UIElements;
using OpenFitter.Editor.Services;

namespace OpenFitter.Editor
{
    /// <summary>
    /// Base class for wizard step presenters.
    /// </summary>
    public abstract class WizardStepPresenterBase
    {
        protected readonly OpenFitterState stateService;
        protected readonly OpenFitterWizardView view;
        protected OpenFitterWizardPresenter presenter;
        protected VisualElement stepContainer;

        protected WizardStepPresenterBase(
            OpenFitterState stateService,
            OpenFitterWizardView view,
            OpenFitterWizardPresenter presenter,
            VisualElement parent)
        {
            this.stateService = stateService;
            this.view = view;
            this.presenter = presenter;
            stepContainer = parent;

            // Load UXML only if UxmlPath is provided (for backward compatibility)
            // New view-based presenters should set UxmlPath to empty string
            if (!string.IsNullOrEmpty(UxmlPath))
            {
                var stepAsset = LoadStepAsset();
                stepAsset.CloneTree(stepContainer);
            }
        }

        public void RequestNextStep()
        {
            presenter.GoNext();
        }

        protected void InvokeStatusChanged()
        {
            presenter.NotifyStatusChanged();
        }

        public virtual void OnEnter()
        {
            Refresh();
        }

        public virtual void OnExit()
        {
            UnbindElements();
        }

        public abstract bool CanProceed();

        public virtual bool CanGoBack() => true;

        public abstract void Refresh();

        protected abstract string UxmlPath { get; }

        protected virtual VisualTreeAsset LoadStepAsset() => AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlPath);



        protected abstract void BindElements();

        protected abstract void UnbindElements();

        public virtual void Dispose() { }
    }
}

