# プロジェクト構造

OpenFitterUnityUIのディレクトリ構造とファイル構成について説明します。

## ディレクトリ概要

```
OpenFitter/
├── Assets/
│   └── OpenFitter/
│       ├── Editor/                    # Unity Editor拡張のメインコード
│       └── OpenFitter.asmdef          # アセンブリ定義ファイル
├── BlenderTools/                      # ランタイムにダウンロードされる（リポジトリには含まれない）
│   ├── blender/                       # Blenderポータブルインストール
│   ├── open-fitter-core/              # コアフィッティングアルゴリズム（GPL-3）
│   └── addons/                        # Blenderアドオン
├── docs/                              # VitePressドキュメント
│   ├── .vitepress/                    # VitePress設定
│   ├── development/                   # 開発者向けドキュメント
│   └── *.md                           # ドキュメントファイル
├── scripts/                           # ビルドと開発ツール
│   ├── create-unitypackage.ts         # Unityパッケージビルドスクリプト
│   └── l10n-validator.ts              # ローカリゼーション検証スクリプト
├── package.json                       # Node.js依存関係
├── package-lock.json
├── tsconfig.json                      # TypeScript設定
├── LICENSE                            # MITライセンス
└── README.md                          # プロジェクト概要
```

## Assets/OpenFitter/Editor/

Unity Editor拡張のメインコード。すべてMIT Licenseでライセンスされています。

### Controllers/

MVPパターンのPresenter（コントローラー）層。

```
Controllers/
├── IRootPresenter.cs                  # ルートプレゼンターインターフェース
├── IWizardPresenter.cs                # ウィザードプレゼンターインターフェース
├── OpenFitterRootPresenter.cs         # ルートウィンドウのプレゼンター
├── OpenFitterWizardPresenter.cs       # ウィザードメインプレゼンター
└── WizardSteps/                       # 各ステップのプレゼンター
    ├── WizardStepPresenterBase.cs     # ステッププレゼンターの基底クラス
    ├── SourceSelectionStepPresenter.cs
    ├── TargetSelectionStepPresenter.cs
    ├── EnvironmentSetupStepPresenter.cs
    ├── BlendShapeStepPresenter.cs
    ├── AdvancedOptionsStepPresenter.cs
    ├── ExecutionStepPresenter.cs
    ├── CompletionStepPresenter.cs
    └── SetupItemPresenter.cs          # セットアップアイテムのプレゼンター
```

**責務**:
- ユーザー入力の処理
- ビューの更新
- ビジネスロジックの調整

### Views/

UIToolkitベースのビュー層。

```
Views/
├── IRootView.cs                       # ルートビューインターフェース
├── IWizardView.cs                     # ウィザードビューインターフェース
├── OpenFitterRootView.cs              # ルートウィンドウビュー
├── OpenFitterWizardView.cs            # ウィザードメインビュー
├── WizardShell.uxml                   # ウィザードのメインレイアウト
├── WizardStepParts.uxml               # 共通ステップパーツ
├── OpenFitterSetupStep.uxml           # セットアップステップのレイアウト
└── WizardSteps/                       # 各ステップのビュー
    ├── ISourceSelectionStepView.cs
    ├── ITargetSelectionStepView.cs
    ├── IEnvironmentSetupStepView.cs
    ├── IBlendShapeStepView.cs
    ├── IAdvancedOptionsStepView.cs
    ├── IExecutionStepView.cs
    ├── ICompletionStepView.cs
    ├── ISetupItemView.cs
    ├── SourceSelectionStepView.cs
    ├── SourceSelectionStep.uxml
    ├── TargetSelectionStepView.cs
    ├── TargetSelectionStep.uxml
    ├── EnvironmentSetupStepView.cs
    ├── EnvironmentSetupStep.uxml
    ├── BlendShapeStepView.cs
    ├── BlendShapeStep.uxml
    ├── BlendShapeRow.uxml
    ├── AdvancedOptionsStepView.cs
    ├── ExecutionStepView.cs
    ├── ExecutionStep.uxml
    ├── CompletionStepView.cs
    ├── CompletionStep.uxml
    └── SetupItemView.cs
```

**責務**:
- UIの表示と更新
- ユーザー入力の受付
- プレゼンターへのイベント通知

### Services/

ビジネスロジックを実装するサービス層。

```
Services/
├── IOpenFitterEnvironmentService.cs   # 環境サービスインターフェース
├── OpenFitterEnvironmentService.cs    # 環境セットアップサービス
├── OpenFitterSetupCoordinator.cs      # セットアップタスクコーディネーター
├── OpenFitterFittingRunner.cs         # フィッティング実行管理
├── FittingService.cs                  # フィッティングサービス
├── FittingProgressParser.cs           # 進捗パーサー
├── OpenFitterState.cs                 # アプリケーション状態
├── SetupResult.cs                     # セットアップ結果
├── SetupResultHandler.cs              # セットアップ結果ハンドラー
├── ISetupTask.cs                      # セットアップタスクインターフェース
├── EnvironmentValidationTask.cs       # 環境検証タスク
└── Strategies/                        # フィッティング戦略
    ├── IFittingStrategy.cs            # 戦略インターフェース
    ├── ContinuousFittingStrategy.cs   # 連続フィッティング
    └── SingleStepFittingStrategy.cs   # シングルステップフィッティング
```

**責務**:
- ビジネスロジックの実装
- 外部プロセスの管理
- データの永続化と取得

### Downloaders/

Blenderと依存関係のダウンロード処理。

```
Downloaders/
├── BlenderDownloader.cs               # Blenderダウンローダー
├── BlenderAddonDownloader.cs          # アドオンダウンローダー
├── OpenFitterCoreDownloader.cs        # open-fitter-coreダウンローダー
└── ZipExtractionUtility.cs            # ZIP展開ユーティリティ
```

**責務**:
- 必要なコンポーネントのダウンロード
- ファイルの展開と検証
- ダウンロード進捗の報告

### Installers/

ダウンロードしたコンポーネントのインストール処理。

```
Installers/
├── BlenderInstaller.cs                # Blenderインストーラー
├── BlenderAddonInstaller.cs           # アドオンインストーラー
└── OpenFitterCoreInstaller.cs         # open-fitter-coreインストーラー
```

**責務**:
- コンポーネントのインストール
- 環境の設定
- インストール状態の検証

### Resources/

Pythonスクリプトとリソースファイル。

```
Resources/
├── openfitter_wrapper.py              # メインラッパースクリプト
├── check_addon_status.py              # アドオン状態チェック
└── install_addon_dependencies.py      # アドオン依存関係インストール
```

**責務**:
- Blenderとの統合
- フィッティングプロセスの実行
- 進捗報告

### ルートファイル

```
Editor/
├── OpenFitterWindow.cs                # メインウィンドウエントリーポイント
├── OpenFitterModels.cs                # データモデル定義
├── OpenFitterDelegates.cs             # デリゲート定義
├── OpenFitterConstants.cs             # 定数定義
├── OpenFitterWizardStepEnum.cs        # ウィザードステップ列挙型
├── I18n.cs                            # ローカリゼーション
├── ISetupStep.cs                      # セットアップステップインターフェース
├── OpenFitterCommandBuilder.cs        # コマンドライン構築
├── OpenFitterCommandRunner.cs         # コマンド実行
├── OpenFitterFileUtility.cs           # ファイルユーティリティ
├── OpenFitterPathUtility.cs           # パスユーティリティ
├── OpenFitterAssetPostprocessor.cs    # アセットポストプロセッサー
└── ProjectFilePostprocessor.cs        # プロジェクトファイルポストプロセッサー
```

### Tests/

ユニットテストとインテグレーションテスト。

```
Tests/
├── OpenFitter.Editor.Tests.asmdef     # テストアセンブリ定義
└── OpenFitterWizardPresenterTests.cs  # ウィザードプレゼンターテスト
```

**責務**:
- ユニットテストの実装
- モックオブジェクトの定義
- テストヘルパー関数

## BlenderTools/

**注意**: このディレクトリはリポジトリには含まれず、初回セットアップ時に自動的にダウンロード・構成されます。

### blender/

ポータブル版のBlenderインストール。

```
blender/
├── blender.exe                        # Blender実行ファイル（Windows）
├── [version]/                         # Blenderのバージョン番号
│   ├── python/                        # Blenderの内蔵Python
│   └── scripts/                       # Blenderスクリプト
└── ...
```

### open-fitter-core/

GPL-3ライセンスのコアフィッティングアルゴリズム。

```
open-fitter-core/
├── README.md                          # コアアルゴリズムのドキュメント
├── fitting.py                         # メインフィッティングロジック
├── rbf_deform.py                      # RBF変形実装
└── ...
```

### addons/

Blenderアドオン。

```
addons/
├── addon1/                            # アドオン1
├── addon2/                            # アドオン2
└── ...
```

## docs/

VitePressドキュメント。

```
docs/
├── .vitepress/                        # VitePress設定
│   ├── config.ts                      # サイト設定
│   └── theme/                         # カスタムテーマ（オプション）
├── development/                       # 開発者向けドキュメント
│   ├── index.md                       # 開発ガイド
│   ├── architecture.md                # アーキテクチャ
│   ├── technology.md                  # コア技術
│   ├── project-structure.md           # プロジェクト構造（このファイル）
│   └── building.md                    # ビルド方法
├── index.md                           # ホームページ
├── setup.md                           # セットアップガイド
├── usage.md                           # 使い方
├── troubleshooting.md                 # トラブルシューティング
└── license.md                         # ライセンス
```

## scripts/

Node.jsとTypeScriptで実装されたビルドと開発ツール。

```
scripts/
├── create-unitypackage.ts             # Unityパッケージビルドスクリプト
└── l10n-validator.ts                  # ローカリゼーション検証スクリプト
```

### create-unitypackage.ts

Unityパッケージ（.unitypackage）を作成するスクリプト：

- `Assets/OpenFitter/`をパッケージ化
- メタデータの生成
- 出力ファイルの配置

### l10n-validator.ts

ローカリゼーション文字列の検証：

- すべての言語で同じキーが定義されているか確認
- 未使用のキーを検出
- 欠落しているキーを報告

## 設定ファイル

### package.json

Node.js依存関係とスクリプト定義：

```json
{
  "scripts": {
    "docs:dev": "vitepress dev docs",
    "docs:build": "vitepress build docs",
    "create-package": "ts-node scripts/create-unitypackage.ts",
    "check-l10n": "ts-node scripts/l10n-validator.ts"
  }
}
```

### tsconfig.json

TypeScriptコンパイラ設定：

```json
{
  "compilerOptions": {
    "target": "ES2020",
    "module": "commonjs",
    "strict": true
  }
}
```

### OpenFitter.asmdef

Unity アセンブリ定義：

```json
{
  "name": "OpenFitter",
  "includePlatforms": ["Editor"],
  "references": []
}
```

## ファイル命名規則

### C# ファイル

- **クラス名とファイル名を一致**: `OpenFitterWindow.cs` → `OpenFitterWindow` クラス
- **インターフェース**: `I` プレフィックス（例: `IWizardView.cs`）
- **基底クラス**: `Base` サフィックス（例: `WizardStepPresenterBase.cs`）

### UXML ファイル

- **Pascal Case**: `SourceSelectionStep.uxml`
- **説明的な名前**: ステップやコンポーネントの役割を表す

### TypeScript ファイル

- **kebab-case**: `create-unitypackage.ts`
- **説明的な名前**: スクリプトの目的を表す

## コード構成のベストプラクティス

### 1. 単一責任の原則

各クラスは単一の責任のみを持つ：

- ✅ `BlenderDownloader` - Blenderのダウンロードのみ
- ❌ `BlenderDownloaderAndInstaller` - 複数の責任

### 2. インターフェースの分離

大きなインターフェースを小さく分割：

- ✅ `ISourceSelectionStepView` - ソース選択ステップ専用
- ❌ `IAllStepsView` - すべてのステップを含む

### 3. 依存性の逆転

具象クラスではなくインターフェースに依存：

```csharp
// ✅ 良い例
public ExecutionStepPresenter(IFittingService fittingService)

// ❌ 悪い例
public ExecutionStepPresenter(FittingService fittingService)
```

### 4. ディレクトリ構造の一貫性

関連するファイルを同じディレクトリに配置：

- ビューとそのUXMLファイルを同じディレクトリに
- プレゼンターとビューを対応するディレクトリに

## まとめ

OpenFitterUnityUIのプロジェクト構造は以下の原則に基づいて整理されています：

- **明確な責務分離**: 各ディレクトリが明確な役割を持つ
- **MVPアーキテクチャ**: Controllers（Presenters）、Views、Servicesに分割
- **拡張性**: 新しい機能を容易に追加できる構造
- **保守性**: コードが見つけやすく理解しやすい
- **ライセンスの分離**: MIT部分とGPL-3部分が物理的に分離

---

## English

# Project Structure

This document explains the directory structure and file organization of OpenFitter.

## Directory Overview

```
OpenFitter/
├── Assets/
│   └── OpenFitter/
│       ├── Editor/                    # Main code for Unity Editor extension
│       └── OpenFitter.asmdef          # Assembly definition file
├── BlenderTools/                      # Downloaded at runtime (not included in repo)
│   ├── blender/                       # Blender portable installation
│   ├── open-fitter-core/              # Core fitting algorithm (GPL-3)
│   └── addons/                        # Blender add-ons
├── docs/                              # VitePress documentation
│   ├── .vitepress/                    # VitePress configuration
│   ├── development/                   # Developer documentation
│   └── *.md                           # Documentation files
├── scripts/                           # Build and development tools
│   ├── create-unitypackage.ts         # Unity package build script
│   └── l10n-validator.ts              # Localization validation script
├── package.json                       # Node.js dependencies
├── package-lock.json
├── tsconfig.json                      # TypeScript configuration
├── LICENSE                            # MIT License
└── README.md                          # Project overview
```

## Assets/OpenFitter/Editor/

Main code for Unity Editor extension. All licensed under MIT License.

### Controllers/

Presenter (Controller) layer of MVP pattern.

```
Controllers/
├── IRootPresenter.cs                  # Root presenter interface
├── IWizardPresenter.cs                # Wizard presenter interface
├── OpenFitterRootPresenter.cs         # Root window presenter
├── OpenFitterWizardPresenter.cs       # Main wizard presenter
└── WizardSteps/                       # Presenters for each step
    ├── WizardStepPresenterBase.cs     # Base class for step presenters
    ├── SourceSelectionStepPresenter.cs
    ├── TargetSelectionStepPresenter.cs
    ├── EnvironmentSetupStepPresenter.cs
    ├── BlendShapeStepPresenter.cs
    ├── AdvancedOptionsStepPresenter.cs
    ├── ExecutionStepPresenter.cs
    ├── CompletionStepPresenter.cs
    └── SetupItemPresenter.cs          # Setup item presenter
```

**Responsibilities**:
- Process user input
- Update views
- Coordinate business logic

### Views/

UIToolkit-based view layer.

```
Views/
├── IRootView.cs                       # Root view interface
├── IWizardView.cs                     # Wizard view interface
├── OpenFitterRootView.cs              # Root window view
├── OpenFitterWizardView.cs            # Main wizard view
├── WizardShell.uxml                   # Main wizard layout
├── WizardStepParts.uxml               # Common step parts
├── OpenFitterSetupStep.uxml           # Setup step layout
└── WizardSteps/                       # Views for each step
    ├── ISourceSelectionStepView.cs
    ├── ITargetSelectionStepView.cs
    ├── IEnvironmentSetupStepView.cs
    ├── IBlendShapeStepView.cs
    ├── IAdvancedOptionsStepView.cs
    ├── IExecutionStepView.cs
    ├── ICompletionStepView.cs
    ├── ISetupItemView.cs
    ├── SourceSelectionStepView.cs
    ├── SourceSelectionStep.uxml
    ├── TargetSelectionStepView.cs
    ├── TargetSelectionStep.uxml
    ├── EnvironmentSetupStepView.cs
    ├── EnvironmentSetupStep.uxml
    ├── BlendShapeStepView.cs
    ├── BlendShapeStep.uxml
    ├── BlendShapeRow.uxml
    ├── AdvancedOptionsStepView.cs
    ├── ExecutionStepView.cs
    ├── ExecutionStep.uxml
    ├── CompletionStepView.cs
    ├── CompletionStep.uxml
    └── SetupItemView.cs
```

**Responsibilities**:
- Display and update UI
- Accept user input
- Notify presenter of events

### Services/

Service layer implementing business logic.

```
Services/
├── IOpenFitterEnvironmentService.cs   # Environment service interface
├── OpenFitterEnvironmentService.cs    # Environment setup service
├── OpenFitterSetupCoordinator.cs      # Setup task coordinator
├── OpenFitterFittingRunner.cs         # Fitting execution manager
├── FittingService.cs                  # Fitting service
├── FittingProgressParser.cs           # Progress parser
├── OpenFitterState.cs                 # Application state
├── SetupResult.cs                     # Setup result
├── SetupResultHandler.cs              # Setup result handler
├── ISetupTask.cs                      # Setup task interface
├── EnvironmentValidationTask.cs       # Environment validation task
└── Strategies/                        # Fitting strategies
    ├── IFittingStrategy.cs            # Strategy interface
    ├── ContinuousFittingStrategy.cs   # Continuous fitting
    └── SingleStepFittingStrategy.cs   # Single-step fitting
```

**Responsibilities**:
- Implement business logic
- Manage external processes
- Data persistence and retrieval

### Downloaders/

Download handling for Blender and dependencies.

```
Downloaders/
├── BlenderDownloader.cs               # Blender downloader
├── BlenderAddonDownloader.cs          # Add-on downloader
├── OpenFitterCoreDownloader.cs        # open-fitter-core downloader
└── ZipExtractionUtility.cs            # ZIP extraction utility
```

**Responsibilities**:
- Download required components
- Extract and verify files
- Report download progress

### Installers/

Installation handling for downloaded components.

```
Installers/
├── BlenderInstaller.cs                # Blender installer
├── BlenderAddonInstaller.cs           # Add-on installer
└── OpenFitterCoreInstaller.cs         # open-fitter-core installer
```

**Responsibilities**:
- Install components
- Configure environment
- Verify installation status

### Resources/

Python scripts and resource files.

```
Resources/
├── openfitter_wrapper.py              # Main wrapper script
├── check_addon_status.py              # Add-on status check
└── install_addon_dependencies.py      # Add-on dependency installation
```

**Responsibilities**:
- Integration with Blender
- Execute fitting process
- Report progress

### Root Files

```
Editor/
├── OpenFitterWindow.cs                # Main window entry point
├── OpenFitterModels.cs                # Data model definitions
├── OpenFitterDelegates.cs             # Delegate definitions
├── OpenFitterConstants.cs             # Constant definitions
├── OpenFitterWizardStepEnum.cs        # Wizard step enumeration
├── I18n.cs                            # Localization
├── ISetupStep.cs                      # Setup step interface
├── OpenFitterCommandBuilder.cs        # Command-line builder
├── OpenFitterCommandRunner.cs         # Command executor
├── OpenFitterFileUtility.cs           # File utility
├── OpenFitterPathUtility.cs           # Path utility
├── OpenFitterAssetPostprocessor.cs    # Asset postprocessor
└── ProjectFilePostprocessor.cs        # Project file postprocessor
```

### Tests/

Unit tests and integration tests.

```
Tests/
├── OpenFitter.Editor.Tests.asmdef     # Test assembly definition
└── OpenFitterWizardPresenterTests.cs  # Wizard presenter tests
```

**Responsibilities**:
- Implement unit tests
- Define mock objects
- Test helper functions

## BlenderTools/

**Note**: This directory is not included in the repository and is automatically downloaded and configured during initial setup.

### blender/

Portable Blender installation.

```
blender/
├── blender.exe                        # Blender executable (Windows)
├── [version]/                         # Blender version number
│   ├── python/                        # Blender's built-in Python
│   └── scripts/                       # Blender scripts
└── ...
```

### open-fitter-core/

GPL-3 licensed core fitting algorithm.

```
open-fitter-core/
├── README.md                          # Core algorithm documentation
├── fitting.py                         # Main fitting logic
├── rbf_deform.py                      # RBF deformation implementation
└── ...
```

### addons/

Blender add-ons.

```
addons/
├── addon1/                            # Add-on 1
├── addon2/                            # Add-on 2
└── ...
```

## docs/

VitePress documentation.

```
docs/
├── .vitepress/                        # VitePress configuration
│   ├── config.ts                      # Site configuration
│   └── theme/                         # Custom theme (optional)
├── development/                       # Developer documentation
│   ├── index.md                       # Development guide
│   ├── architecture.md                # Architecture
│   ├── technology.md                  # Core technology
│   ├── project-structure.md           # Project structure (this file)
│   └── building.md                    # Building
├── index.md                           # Home page
├── setup.md                           # Setup guide
├── usage.md                           # Usage guide
├── troubleshooting.md                 # Troubleshooting
└── license.md                         # License
```

## scripts/

Build and development tools implemented in Node.js and TypeScript.

```
scripts/
├── create-unitypackage.ts             # Unity package build script
└── l10n-validator.ts                  # Localization validation script
```

### create-unitypackage.ts

Script to create Unity package (.unitypackage):

- Package `Assets/OpenFitter/`
- Generate metadata
- Place output file

### l10n-validator.ts

Validate localization strings:

- Check if same keys are defined in all languages
- Detect unused keys
- Report missing keys

## Configuration Files

### package.json

Node.js dependencies and script definitions:

```json
{
  "scripts": {
    "docs:dev": "vitepress dev docs",
    "docs:build": "vitepress build docs",
    "create-package": "ts-node scripts/create-unitypackage.ts",
    "check-l10n": "ts-node scripts/l10n-validator.ts"
  }
}
```

### tsconfig.json

TypeScript compiler configuration:

```json
{
  "compilerOptions": {
    "target": "ES2020",
    "module": "commonjs",
    "strict": true
  }
}
```

### OpenFitter.asmdef

Unity assembly definition:

```json
{
  "name": "OpenFitter",
  "includePlatforms": ["Editor"],
  "references": []
}
```

## File Naming Conventions

### C# Files

- **Match class name with file name**: `OpenFitterWindow.cs` → `OpenFitterWindow` class
- **Interfaces**: `I` prefix (e.g., `IWizardView.cs`)
- **Base classes**: `Base` suffix (e.g., `WizardStepPresenterBase.cs`)

### UXML Files

- **Pascal Case**: `SourceSelectionStep.uxml`
- **Descriptive names**: Represent role of step or component

### TypeScript Files

- **kebab-case**: `create-unitypackage.ts`
- **Descriptive names**: Represent purpose of script

## Code Organization Best Practices

### 1. Single Responsibility Principle

Each class has only one responsibility:

- ✅ `BlenderDownloader` - Only downloads Blender
- ❌ `BlenderDownloaderAndInstaller` - Multiple responsibilities

### 2. Interface Segregation

Split large interfaces into smaller ones:

- ✅ `ISourceSelectionStepView` - Specific to source selection step
- ❌ `IAllStepsView` - Contains all steps

### 3. Dependency Inversion

Depend on interfaces, not concrete classes:

```csharp
// ✅ Good
public ExecutionStepPresenter(IFittingService fittingService)

// ❌ Bad
public ExecutionStepPresenter(FittingService fittingService)
```

### 4. Consistent Directory Structure

Place related files in the same directory:

- Views and their UXML files in the same directory
- Presenters and views in corresponding directories

## Summary

OpenFitter's project structure is organized based on the following principles:

- **Clear Separation of Responsibilities**: Each directory has a clear role
- **MVP Architecture**: Divided into Controllers (Presenters), Views, and Services
- **Extensibility**: Structure that allows easy addition of new features
- **Maintainability**: Code is easy to find and understand
- **License Separation**: MIT and GPL-3 portions are physically separated
