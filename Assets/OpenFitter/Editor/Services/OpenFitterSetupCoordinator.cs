using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OpenFitter.Editor.Services;

namespace OpenFitter.Editor
{
    public sealed class OpenFitterSetupCoordinator
    {
        public sealed class SetupEntry
        {
            public string Name { get; }
            public ISetupTask Task { get; }
            public StateChangedHandler? OnComplete { get; }
            public Func<bool>? ShouldSkip { get; }
            public Func<bool> IsReady { get; }
            public string WebsiteUrl { get; }

            public SetupEntry(string name, ISetupTask task, StateChangedHandler? onComplete = null, Func<bool>? shouldSkip = null, Func<bool>? isReady = null, string websiteUrl = "")
            {
                Name = name;
                Task = task;
                OnComplete = onComplete;
                ShouldSkip = shouldSkip;
                IsReady = isReady ?? (() => Task.IsReady);
                WebsiteUrl = websiteUrl;
            }
        }

        private readonly List<SetupEntry> entries;
        private readonly PrefsChangedHandler? onAvailableChanged;
        private CancellationTokenSource? processAllCts;

        public event StateChangedHandler? OnStateChanged;
        public IReadOnlyList<SetupEntry> Entries => entries;
        public bool IsProcessingAll => processAllCts != null;

        public OpenFitterSetupCoordinator(List<SetupEntry> entries)
        {
            this.entries = entries ?? new();
        }

        public async Task ProcessAllAsync()
        {
            if (IsProcessingAll) return;

            processAllCts = new();
            try
            {
                foreach (var entry in entries)
                {
                    if (processAllCts.IsCancellationRequested) break;
                    if (entry.ShouldSkip?.Invoke() == true || entry.IsReady()) continue;

                    await RunTaskAsync(entry, processAllCts.Token);
                    if (!entry.IsReady()) break;
                }
            }
            finally
            {
                processAllCts?.Dispose();
                processAllCts = null;
                OnStateChanged?.Invoke();
            }
        }

        public void ProcessAll() => _ = ProcessAllAsync();

        public void CancelProcessAll() => processAllCts?.Cancel();

        public void AbortCurrentTask()
        {
            foreach (var entry in entries) if (entry.Task.IsRunning) entry.Task.Abort();
        }

        public async Task StartTaskAsync(ISetupTask task)
        {
            var entry = entries.Find(e => e.Task == task);
            if (entry != null) await RunTaskAsync(entry);
        }

        public void StartTask(ISetupTask task) => _ = StartTaskAsync(task);

        private async Task RunTaskAsync(SetupEntry entry, CancellationToken ct = default)
        {
            if (entry.Task.IsRunning) return;

            var result = entry.Task.Start();
            if (result.IsFailed)
            {
                SetupResultHandler.HandleError(result, entry.Name);
                return;
            }

            try
            {
                while (entry.Task.IsRunning && !ct.IsCancellationRequested)
                {
                    result = entry.Task.Update();
                    SetupResultHandler.UpdateProgressBar(result, entry.Name, entry.Task.Progress);
                    OnStateChanged?.Invoke();
                    await Task.Yield();
                }

                if (result.IsSuccess)
                {
                    entry.OnComplete?.Invoke();
                    onAvailableChanged?.Invoke();
                }
                else if (result.IsFailed)
                {
                    SetupResultHandler.HandleError(result, entry.Name);
                }
            }
            finally
            {
                SetupResultHandler.UpdateProgressBar(SetupResult.Success(), entry.Name, 1f);
                OnStateChanged?.Invoke();
            }
        }
    }
}

