namespace OpenFitter.Editor
{
    /// <summary>
    /// Wizard step definitions.
    /// </summary>
    public enum WizardStep
    {
        None = -1,
        EnvironmentSetup = 0,
        SourceSelection = 1,
        TargetSelection = 2,
        BlendShapeCustomization = 3,
        AdvancedOptions = 4,
        Execution = 5,
        Completion = 6
    }

    /// <summary>
    /// Wizard step metadata helpers.
    /// </summary>
    public static class WizardStepMetadata
    {
        public static string GetStepTitle(WizardStep step) => step switch
        {
            WizardStep.EnvironmentSetup => I18n.Tr("Environment Setup"),
            WizardStep.SourceSelection => I18n.Tr("Outfit Selection"),
            WizardStep.TargetSelection => I18n.Tr("Target Selection"),
            WizardStep.BlendShapeCustomization => I18n.Tr("Body/Shape Adjustment"),
            WizardStep.AdvancedOptions => I18n.Tr("Advanced Settings"),
            WizardStep.Execution => I18n.Tr("Execution"),
            WizardStep.Completion => I18n.Tr("Completion"),
            _ => ""
        };

        public static int GetTotalSteps() => (int)WizardStep.Completion + 1;
    }
}
