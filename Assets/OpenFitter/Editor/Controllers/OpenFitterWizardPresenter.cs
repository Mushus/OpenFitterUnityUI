using System;
using OpenFitter.Editor.Services;

namespace OpenFitter.Editor
{
    public sealed class OpenFitterWizardPresenter : IDisposable
    {
        private readonly OpenFitterState stateService;
        private readonly IOpenFitterEnvironmentService environmentService;
        private readonly ConfigurationService configurationService;
        private readonly OpenFitterWizardView view;
        private WizardStepPresenterBase? currentStepPresenter;

        public OpenFitterWizardPresenter(
            OpenFitterState stateService,
            IOpenFitterEnvironmentService environmentService,
            ConfigurationService configurationService,
            OpenFitterWizardView view)
        {
            this.stateService = stateService;
            this.environmentService = environmentService;
            this.configurationService = configurationService;
            this.view = view;

            view.OnNextClicked += OnNextClicked;
            view.OnBackClicked += OnBackClicked;

            var initialStep = stateService.WizardStep;

            // Should auto-skip logic (Only on fresh start)
            if (initialStep == WizardStep.None || initialStep < WizardStep.EnvironmentSetup)
            {
                initialStep = environmentService.IsEnvironmentReady()
                    ? WizardStep.SourceSelection
                    : WizardStep.EnvironmentSetup;
            }

            // Ensure we start with a valid step presenter
            NavigateToStep(initialStep);
        }

        public void Dispose()
        {
            view.OnNextClicked -= OnNextClicked;
            view.OnBackClicked -= OnBackClicked;

            currentStepPresenter?.Dispose();
            currentStepPresenter = null;
        }

        private void OnNextClicked()
        {
            if (currentStepPresenter == null || !currentStepPresenter.CanProceed())
            {
                return;
            }

            var current = stateService.WizardStep;
            if (current == WizardStep.Completion)
            {
                NavigateToStep(WizardStep.SourceSelection);
                return;
            }

            var nextStep = (WizardStep)((int)current + 1);
            if (nextStep <= WizardStep.Completion)
            {
                NavigateToStep(nextStep);
            }
        }

        private void OnBackClicked()
        {
            if (currentStepPresenter == null || !currentStepPresenter.CanGoBack())
            {
                return;
            }

            var current = stateService.WizardStep;
            if (current == WizardStep.Completion)
            {
                NavigateToStep(WizardStep.AdvancedOptions);
                return;
            }

            var prevStep = (WizardStep)((int)current - 1);
            if (prevStep >= WizardStep.EnvironmentSetup)
            {
                NavigateToStep(prevStep);
            }
        }

        private void NavigateToStep(WizardStep step)
        {
            // Avoid redundant navigation
            if (currentStepPresenter != null && stateService.WizardStep == step)
            {
                return;
            }

            // Dispose previous step
            if (currentStepPresenter != null)
            {
                currentStepPresenter.OnExit();
                currentStepPresenter.Dispose();
                currentStepPresenter = null;
            }

            view.ClearStepContent();
            stateService.WizardStep = step;

            view.SetCurrentStep(step);
            view.UpdateStepIndicators(step);

            // Create new step presenter
            currentStepPresenter = CreateStepPresenter(step);

            if (currentStepPresenter != null)
            {
                // UI is essentially already created in constructor, but logical entry point
                currentStepPresenter.OnEnter();
                UpdateNavigationButtons();
            }
        }

        private WizardStepPresenterBase? CreateStepPresenter(WizardStep step)
        {
            var container = view.GetStepContentContainer();
            if (container == null)
            {
                UnityEngine.Debug.LogError("Failed to get step content container.");
                return null;
            }

            // Using fully qualified factory/constructors based on new signature:
            // (State, [Services], View, Presenter, Parent)

            return step switch
            {
                WizardStep.EnvironmentSetup => new EnvironmentSetupStepPresenter(stateService, environmentService, view, this, container),
                WizardStep.SourceSelection => new SourceSelectionStepPresenter(stateService, view, this, container),
                WizardStep.TargetSelection => new TargetSelectionStepPresenter(stateService, configurationService, view, this, container),
                WizardStep.BlendShapeCustomization => new BlendShapeStepPresenter(stateService, configurationService, view, this, container),
                WizardStep.AdvancedOptions => new AdvancedOptionsStepPresenter(stateService, view, this, container),
                WizardStep.Execution => new ExecutionStepPresenter(stateService, environmentService, configurationService, view, this, container),
                WizardStep.Completion => new CompletionStepPresenter(stateService, view, this, container),
                _ => null
            };
        }

        private void UpdateNavigationButtons()
        {
            if (currentStepPresenter == null)
            {
                return;
            }

            var current = stateService.WizardStep;
            bool canGoBack = current > WizardStep.EnvironmentSetup && currentStepPresenter.CanGoBack();
            view.SetBackButtonEnabled(canGoBack);

            view.SetNextButtonEnabled(currentStepPresenter.CanProceed());

            string nextText = current switch
            {
                WizardStep.AdvancedOptions => I18n.Tr("Execute"),
                WizardStep.Execution => I18n.Tr("To Completion"),
                WizardStep.Completion => I18n.Tr("Continue Work"),
                _ => I18n.Tr("Next")
            };
            view.SetNextButtonText(nextText);
        }


        public void GoNext()
        {
            OnNextClicked();
        }

        public void GoBack()
        {
            OnBackClicked();
        }

        public void NavigateTo(WizardStep step)
        {
            NavigateToStep(step);
        }

        public void NotifyStatusChanged()
        {
            UpdateNavigationButtons();
        }
    }
}
