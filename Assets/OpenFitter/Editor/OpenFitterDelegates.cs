namespace OpenFitter.Editor
{
    /// <summary>
    /// セットアップの状態が変更された際のハンドラー
    /// </summary>
    public delegate void SetupStateChangedHandler();

    /// <summary>
    /// ログを受信した際のハンドラー
    /// </summary>
    public delegate void LogReceivedHandler(string message);

    /// <summary>
    /// ステータスが変更された際のハンドラー
    /// </summary>
    public delegate void StatusChangedHandler(string status);

    /// <summary>
    /// ステップが変更された際のハンドラー
    /// </summary>
    public delegate void StepChangedHandler(int currentStep, int totalSteps);

    /// <summary>
    /// ナビゲーションボタンがクリックされた際のハンドラー
    /// </summary>
    public delegate void NavigationClickHandler();

    /// <summary>
    /// 設定が変更された際のハンドラー
    /// </summary>
    public delegate void PrefsChangedHandler();

    /// <summary>
    /// 進捗が変更された際のハンドラー
    /// </summary>
    public delegate void ProgressChangedHandler(float progress, string detail);

    /// <summary>
    /// 状態が変更された際のハンドラー
    /// </summary>
    public delegate void StateChangedHandler();
}
