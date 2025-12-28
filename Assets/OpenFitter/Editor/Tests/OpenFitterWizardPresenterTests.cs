using NUnit.Framework;
using OpenFitter.Editor.Services;
using OpenFitter.Editor.Downloaders;
using UnityEngine.UIElements;
using System.Threading.Tasks;

namespace OpenFitter.Editor.Tests
{
    /// <summary>
    /// Wizardのナビゲーションロジック（特に自動スキップ機能）を検証するテストクラス。
    /// </summary>
    [TestFixture]
    public class OpenFitterWizardPresenterTests
    {
        // 環境サービス（Blender/Coreのインストール状態）のモック
        private class MockEnvironmentService : IOpenFitterEnvironmentService
        {
            public bool IsReady { get; set; }

            public string ScriptPath => string.Empty;
            public string BlenderPath => string.Empty;

            public bool IsEnvironmentReady() => IsReady;
            public bool EnsureOpenFitterCorePath() => true;
            public bool ValidateEnvironmentForRun() => true;

            public Task StartSetupAllAsync(OpenFitterSetupCoordinator coordinator) => Task.CompletedTask;
            public void StartSetupAll(OpenFitterSetupCoordinator coordinator) { }

            public EnvironmentValidationTask CreateEnvironmentValidationTask() => null!;
            public EnvironmentSetupContext CreateEnvironmentSetupContext() => null!;

            public void StartAddonDownload(OpenFitterSetupCoordinator c, BlenderAddonDownloader t) { }

            // Cleanup
            public void RemoveBlender(OpenFitterSetupCoordinator c) { }
            public void RemoveOpenFitterCore(OpenFitterSetupCoordinator c) { }
            public void RemoveAddon(OpenFitterSetupCoordinator c) { }
            public void ResetEnvironment(OpenFitterSetupCoordinator c) { }

            public void ReinstallBlender(OpenFitterSetupCoordinator c, BlenderDownloader t) { }
            public void ReinstallOpenFitterCore(OpenFitterSetupCoordinator c, OpenFitterCoreDownloader t) { }
            public void ReinstallAddon(OpenFitterSetupCoordinator c, BlenderAddonDownloader t) { }
        }

        private OpenFitterWizardPresenter? presenter;
        private MockEnvironmentService? mockEnv;
        private OpenFitterState? stateService;
        private OpenFitterWizardView? view;

        [SetUp]
        public void Setup()
        {
            mockEnv = new MockEnvironmentService();
            stateService = new OpenFitterState();
            stateService.ClearPrefs(); // テスト用に状態をクリア
        }

        [TearDown]
        public void Teardown()
        {
            if (stateService != null)
            {
                stateService.ClearPrefs();
                stateService.Dispose();
            }
            presenter?.Dispose();
        }

        // ... Tests are unchanged usually, but verify signatures ...

        [Test]
        public void Constructor_FreshStart_ResultIsSourceSelection_WhenEnvironmentReady()
        {
            stateService!.WizardStep = WizardStep.None;
            mockEnv!.IsReady = true;
            presenter = CreatePresenter(CreateMockRoot());
            Assert.AreEqual(WizardStep.SourceSelection, stateService.WizardStep);
        }

        [Test]
        public void Constructor_FreshStart_ResultIsEnvironmentSetup_WhenEnvironmentNotReady()
        {
            stateService!.WizardStep = WizardStep.None;
            mockEnv!.IsReady = false;
            presenter = CreatePresenter(CreateMockRoot());
            Assert.AreEqual(WizardStep.EnvironmentSetup, stateService.WizardStep);
        }

        [Test]
        public void Constructor_ExistingState_ResultIsPreserved_WhenEnvironmentReady()
        {
            stateService!.WizardStep = WizardStep.EnvironmentSetup;
            mockEnv!.IsReady = true;
            presenter = CreatePresenter(CreateMockRoot());
            Assert.AreEqual(WizardStep.EnvironmentSetup, stateService.WizardStep);
        }

        private VisualElement CreateMockRoot()
        {
            var root = new VisualElement();
            root.Add(new VisualElement { name = "step-content-container" });
            root.Add(new VisualElement { name = "step-indicators" });
            root.Add(new Label { name = "lbl-step-title" });
            root.Add(new Button { name = "btn-back" });
            root.Add(new Button { name = "btn-next" });
            return root;
        }

        private OpenFitterWizardPresenter CreatePresenter(VisualElement root)
        {
            view = new OpenFitterWizardView(root);
            var configService = new ConfigurationService();

            return new OpenFitterWizardPresenter(
                stateService!,
                mockEnv!,
                configService,
                view
            );
        }
    }
}

