# アーキテクチャ

OpenFitterUnityUIのシステムアーキテクチャとデザインパターンについて説明します。

## 概要

OpenFitterは**Model-View-Presenter (MVP)**アーキテクチャパターンを採用しています。このパターンにより、UIロジックとビジネスロジックが明確に分離され、テストしやすく保守しやすいコードベースを実現しています。

## MVPアーキテクチャ

### Model（モデル）

データとビジネスロジックを表現します。

**主要なモデルクラス**:

- `OpenFitterState` - ウィザード全体の状態を管理
- `ConfigInfo` - フィッティング設定情報
- `BlendShapeEntry` - ブレンドシェイプのエントリ情報
- `SetupResult` - セットアップタスクの結果

モデルは以下の責務を持ちます：
- アプリケーションの状態を保持
- データの検証
- ビジネスルールの実装

### View（ビュー）

ユーザーインターフェースを表現し、ユーザー入力を受け取ります。

**主要なビューインターフェース**:

- `IRootView` - ルートウィンドウのインターフェース
- `IWizardView` - ウィザードのメインインターフェース
- `ISourceSelectionStepView` - ソース選択ステップのビュー
- `ITargetSelectionStepView` - ターゲット選択ステップのビュー
- `IEnvironmentSetupStepView` - 環境セットアップステップのビュー
- `IBlendShapeStepView` - ブレンドシェイプ設定ステップのビュー
- `IExecutionStepView` - 実行ステップのビュー
- `ICompletionStepView` - 完了ステップのビュー

**ビューの実装**:
- UIToolkitを使用してビューを実装
- `.uxml`ファイルでUIレイアウトを定義
- `.uss`ファイル（必要に応じて）でスタイルを定義

ビューの責務：
- UIの表示と更新
- ユーザー入力の受付
- プレゼンターへのイベント通知

### Presenter（プレゼンター）

ビューとモデルの間を仲介し、UIロジックを管理します。

**主要なプレゼンタークラス**:

- `OpenFitterRootPresenter` - ルートウィンドウのプレゼンター
- `OpenFitterWizardPresenter` - ウィザード全体のコーディネーター
- `SourceSelectionStepPresenter` - ソース選択ステップのプレゼンター
- `TargetSelectionStepPresenter` - ターゲット選択ステップのプレゼンター
- `EnvironmentSetupStepPresenter` - 環境セットアップステップのプレゼンター
- `BlendShapeStepPresenter` - ブレンドシェイプ設定ステップのプレゼンター
- `ExecutionStepPresenter` - 実行ステップのプレゼンター
- `CompletionStepPresenter` - 完了ステップのプレゼンター

プレゼンターの責務：
- ユーザー入力の処理
- モデルの更新
- ビューの更新指示
- ビジネスロジックの調整

## レイヤー構造

```
┌─────────────────────────────────────┐
│          UI Layer (Views)            │
│  - UIToolkit components              │
│  - .uxml layout files                │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│     Presentation Layer (Presenters) │
│  - UI logic                          │
│  - User interaction handling         │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│      Business Layer (Services)       │
│  - FittingService                    │
│  - EnvironmentService                │
│  - SetupCoordinator                  │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│      Data Layer (Models)             │
│  - State management                  │
│  - Configuration                     │
└─────────────────────────────────────┘
```

## サービス層

ビジネスロジックを実装する主要なサービスクラス：

### FittingService

フィッティングプロセス全体を調整します。

**主要な機能**:
- フィッティングの実行管理
- 進捗状況の監視
- エラーハンドリング

**使用する戦略パターン**:
- `IFittingStrategy` - フィッティング戦略のインターフェース
  - `ContinuousFittingStrategy` - 連続フィッティング戦略
  - `SingleStepFittingStrategy` - シングルステップフィッティング戦略

### OpenFitterEnvironmentService

Blenderと依存関係の環境セットアップを管理します。

**主要な機能**:
- 環境の検証
- 必要なコンポーネントのインストール状態確認
- セットアップタスクの調整

### OpenFitterSetupCoordinator

インストールタスクを調整し、セットアップワークフローを管理します。

**主要な機能**:
- タスクの並列実行
- 依存関係の解決
- 進捗レポート

## データフロー

### 1. ユーザー入力フロー

```
User Input → View → Presenter → Model → Service
                ↓                          ↓
            UI Update ←──────────────── Result
```

### 2. フィッティング実行フロー

```
ExecutionStepPresenter
    ↓
FittingService
    ↓
OpenFitterFittingRunner
    ↓
OpenFitterCommandBuilder
    ↓
OpenFitterCommandRunner
    ↓
Blender (External Process)
    ↓
FittingProgressParser
    ↓
Progress Updates → ExecutionStepView
```

### 3. 環境セットアップフロー

```
EnvironmentSetupStepPresenter
    ↓
OpenFitterSetupCoordinator
    ↓
OpenFitterEnvironmentService
    ↓
┌────────────────┬─────────────────┬──────────────────┐
│                │                 │                  │
BlenderDownloader│BlenderAddonDownloader│OpenFitterCoreDownloader
│                │                 │                  │
BlenderInstaller │BlenderAddonInstaller │OpenFitterCoreInstaller
└────────────────┴─────────────────┴──────────────────┘
    ↓
SetupResult → EnvironmentSetupStepView
```

## デザインパターン

### 1. Strategy Pattern（戦略パターン）

フィッティング戦略の切り替えに使用：

```csharp
public interface IFittingStrategy
{
    Task<bool> ExecuteFittingAsync(/* parameters */);
}

public class ContinuousFittingStrategy : IFittingStrategy { }
public class SingleStepFittingStrategy : IFittingStrategy { }
```

### 2. Repository Pattern（リポジトリパターン）

設定の永続化に使用：

```csharp
public interface IOpenFitterConfigRepository
{
    ConfigInfo LoadConfig();
    void SaveConfig(ConfigInfo config);
}
```

### 3. Observer Pattern（オブザーバーパターン）

ビューとプレゼンター間のイベント通知に使用：

```csharp
// View
public event Action OnNextButtonClicked;

// Presenter
view.OnNextButtonClicked += HandleNextButtonClicked;
```

### 4. Dependency Injection（依存性注入）

サービスとプレゼンター間の疎結合を実現：

```csharp
public class ExecutionStepPresenter
{
    private readonly IFittingService fittingService;

    public ExecutionStepPresenter(IFittingService fittingService)
    {
        this.fittingService = fittingService;
    }
}
```

## ステート管理

### OpenFitterState

アプリケーション全体の状態を一元管理：

```csharp
public class OpenFitterState
{
    public OpenFitterWizardStepEnum CurrentStep { get; set; }
    public ConfigInfo Config { get; set; }
    public List<BlendShapeEntry> BlendShapeEntries { get; set; }
    // ... その他の状態
}
```

状態の変更は常にプレゼンターを通じて行われ、ビューは状態を直接変更しません。

## エラーハンドリング

### エラーの伝播

```
Service Layer (Exception thrown)
    ↓
Presenter Layer (Catch and handle)
    ↓
View Layer (Display error to user)
```

### エラーハンドリングのベストプラクティス

1. **サービス層**: 詳細なエラー情報を含む例外をスロー
2. **プレゼンター層**: 例外をキャッチし、ユーザーフレンドリーなメッセージに変換
3. **ビュー層**: エラーメッセージを適切にユーザーに表示

## 拡張性

### 新しいウィザードステップの追加

1. `IStepView`インターフェースを実装したビューを作成
2. `.uxml`ファイルでUIレイアウトを定義
3. `WizardStepPresenterBase`を継承したプレゼンターを作成
4. `OpenFitterWizardStepEnum`に新しいステップを追加
5. `OpenFitterWizardPresenter`にステップのナビゲーションロジックを追加

### 新しいフィッティング戦略の追加

1. `IFittingStrategy`インターフェースを実装
2. 戦略固有のロジックを実装
3. `FittingService`で新しい戦略を選択可能にする

## テスタビリティ

MVPアーキテクチャにより、以下のテストが容易になります：

### ユニットテスト

- **プレゼンター**: モックビューとモックサービスを使用してテスト
- **サービス**: ビジネスロジックを独立してテスト
- **モデル**: データ検証ロジックをテスト

### インテグレーションテスト

- プレゼンターとサービスの統合をテスト
- エンドツーエンドのワークフローをテスト

## まとめ

OpenFitterUnityUIのアーキテクチャは以下の原則に基づいています：

- **関心の分離**: UI、ビジネスロジック、データが明確に分離
- **疎結合**: インターフェースを使用して依存関係を最小化
- **テスタビリティ**: 各コンポーネントが独立してテスト可能
- **拡張性**: 新しい機能を容易に追加可能
- **保守性**: コードが読みやすく、理解しやすい

---

## English

# Architecture

This document explains the system architecture and design patterns of OpenFitter.

## Overview

OpenFitter adopts the **Model-View-Presenter (MVP)** architectural pattern. This pattern provides clear separation between UI logic and business logic, resulting in a testable and maintainable codebase.

## MVP Architecture

### Model

Represents data and business logic.

**Key Model Classes**:

- `OpenFitterState` - Manages overall wizard state
- `ConfigInfo` - Fitting configuration information
- `BlendShapeEntry` - Blendshape entry information
- `SetupResult` - Setup task results

Model responsibilities:
- Hold application state
- Data validation
- Implement business rules

### View

Represents the user interface and receives user input.

**Key View Interfaces**:

- `IRootView` - Root window interface
- `IWizardView` - Main wizard interface
- `ISourceSelectionStepView` - Source selection step view
- `ITargetSelectionStepView` - Target selection step view
- `IEnvironmentSetupStepView` - Environment setup step view
- `IBlendShapeStepView` - Blendshape configuration step view
- `IExecutionStepView` - Execution step view
- `ICompletionStepView` - Completion step view

**View Implementation**:
- Implement views using UIToolkit
- Define UI layout in `.uxml` files
- Define styles in `.uss` files (as needed)

View responsibilities:
- Display and update UI
- Accept user input
- Notify presenter of events

### Presenter

Mediates between view and model, managing UI logic.

**Key Presenter Classes**:

- `OpenFitterRootPresenter` - Root window presenter
- `OpenFitterWizardPresenter` - Overall wizard coordinator
- `SourceSelectionStepPresenter` - Source selection step presenter
- `TargetSelectionStepPresenter` - Target selection step presenter
- `EnvironmentSetupStepPresenter` - Environment setup step presenter
- `BlendShapeStepPresenter` - Blendshape configuration step presenter
- `ExecutionStepPresenter` - Execution step presenter
- `CompletionStepPresenter` - Completion step presenter

Presenter responsibilities:
- Process user input
- Update model
- Instruct view updates
- Coordinate business logic

## Layer Structure

```
┌─────────────────────────────────────┐
│          UI Layer (Views)            │
│  - UIToolkit components              │
│  - .uxml layout files                │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│     Presentation Layer (Presenters) │
│  - UI logic                          │
│  - User interaction handling         │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│      Business Layer (Services)       │
│  - FittingService                    │
│  - EnvironmentService                │
│  - SetupCoordinator                  │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│      Data Layer (Models)             │
│  - State management                  │
│  - Configuration                     │
└─────────────────────────────────────┘
```

## Service Layer

Key service classes implementing business logic:

### FittingService

Orchestrates the entire fitting process.

**Key Features**:
- Manage fitting execution
- Monitor progress
- Error handling

**Strategy Pattern Usage**:
- `IFittingStrategy` - Fitting strategy interface
  - `ContinuousFittingStrategy` - Continuous fitting strategy
  - `SingleStepFittingStrategy` - Single-step fitting strategy

### OpenFitterEnvironmentService

Manages environment setup for Blender and dependencies.

**Key Features**:
- Environment validation
- Check installation status of required components
- Coordinate setup tasks

### OpenFitterSetupCoordinator

Coordinates installation tasks and manages setup workflow.

**Key Features**:
- Parallel task execution
- Dependency resolution
- Progress reporting

## Data Flow

### 1. User Input Flow

```
User Input → View → Presenter → Model → Service
                ↓                          ↓
            UI Update ←──────────────── Result
```

### 2. Fitting Execution Flow

```
ExecutionStepPresenter
    ↓
FittingService
    ↓
OpenFitterFittingRunner
    ↓
OpenFitterCommandBuilder
    ↓
OpenFitterCommandRunner
    ↓
Blender (External Process)
    ↓
FittingProgressParser
    ↓
Progress Updates → ExecutionStepView
```

### 3. Environment Setup Flow

```
EnvironmentSetupStepPresenter
    ↓
OpenFitterSetupCoordinator
    ↓
OpenFitterEnvironmentService
    ↓
┌────────────────┬─────────────────┬──────────────────┐
│                │                 │                  │
BlenderDownloader│BlenderAddonDownloader│OpenFitterCoreDownloader
│                │                 │                  │
BlenderInstaller │BlenderAddonInstaller │OpenFitterCoreInstaller
└────────────────┴─────────────────┴──────────────────┘
    ↓
SetupResult → EnvironmentSetupStepView
```

## Design Patterns

### 1. Strategy Pattern

Used for switching fitting strategies:

```csharp
public interface IFittingStrategy
{
    Task<bool> ExecuteFittingAsync(/* parameters */);
}

public class ContinuousFittingStrategy : IFittingStrategy { }
public class SingleStepFittingStrategy : IFittingStrategy { }
```

### 2. Repository Pattern

Used for configuration persistence:

```csharp
public interface IOpenFitterConfigRepository
{
    ConfigInfo LoadConfig();
    void SaveConfig(ConfigInfo config);
}
```

### 3. Observer Pattern

Used for event notification between view and presenter:

```csharp
// View
public event Action OnNextButtonClicked;

// Presenter
view.OnNextButtonClicked += HandleNextButtonClicked;
```

### 4. Dependency Injection

Achieves loose coupling between services and presenters:

```csharp
public class ExecutionStepPresenter
{
    private readonly IFittingService fittingService;

    public ExecutionStepPresenter(IFittingService fittingService)
    {
        this.fittingService = fittingService;
    }
}
```

## State Management

### OpenFitterState

Centrally manages application-wide state:

```csharp
public class OpenFitterState
{
    public OpenFitterWizardStepEnum CurrentStep { get; set; }
    public ConfigInfo Config { get; set; }
    public List<BlendShapeEntry> BlendShapeEntries { get; set; }
    // ... other state
}
```

State changes are always made through presenters; views never modify state directly.

## Error Handling

### Error Propagation

```
Service Layer (Exception thrown)
    ↓
Presenter Layer (Catch and handle)
    ↓
View Layer (Display error to user)
```

### Error Handling Best Practices

1. **Service Layer**: Throw exceptions with detailed error information
2. **Presenter Layer**: Catch exceptions and convert to user-friendly messages
3. **View Layer**: Appropriately display error messages to user

## Extensibility

### Adding a New Wizard Step

1. Create a view implementing the `IStepView` interface
2. Define UI layout in `.uxml` file
3. Create a presenter inheriting from `WizardStepPresenterBase`
4. Add new step to `OpenFitterWizardStepEnum`
5. Add step navigation logic to `OpenFitterWizardPresenter`

### Adding a New Fitting Strategy

1. Implement the `IFittingStrategy` interface
2. Implement strategy-specific logic
3. Make new strategy selectable in `FittingService`

## Testability

The MVP architecture facilitates the following tests:

### Unit Tests

- **Presenters**: Test using mock views and mock services
- **Services**: Test business logic independently
- **Models**: Test data validation logic

### Integration Tests

- Test integration of presenters and services
- Test end-to-end workflows

## Summary

OpenFitter's architecture is based on the following principles:

- **Separation of Concerns**: Clear separation of UI, business logic, and data
- **Loose Coupling**: Minimize dependencies using interfaces
- **Testability**: Each component can be tested independently
- **Extensibility**: New features can be easily added
- **Maintainability**: Code is readable and understandable
