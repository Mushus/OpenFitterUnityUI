# 開発ガイド

OpenFitterUnityUIの開発に貢献するための情報をまとめたガイドです。

## はじめに

OpenFitterはUnity Editor拡張として開発されたオープンソースプロジェクトです。このガイドでは、プロジェクトの構造、アーキテクチャ、開発ワークフローについて説明します。

## 対象読者

このドキュメントは以下のような方を対象としています：

- OpenFitterに機能追加や改善を行いたい開発者
- バグ修正を行いたいコントリビューター
- コードベースの理解を深めたい方
- フォークして独自の拡張を行いたい方

## 開発環境のセットアップ

### 必要なソフトウェア

- **Unity 2022.3以降**: プロジェクトの開発とテスト
- **Node.js 18以降**: ドキュメントのビルドとスクリプト実行
- **npm**: パッケージ管理とビルドツール
- **Git**: バージョン管理

### リポジトリのクローン

```bash
git clone https://github.com/Mushus/OpenFitterUnityUI.git
cd OpenFitter
```

### 依存関係のインストール

```bash
npm install
```

### Unityでプロジェクトを開く

1. Unity Hubで「開く」を選択
2. クローンしたリポジトリのディレクトリを選択
3. Unity 2022.3以降で開く

## プロジェクト構成

詳細なプロジェクト構造については、[プロジェクト構造](./project-structure.md)ページを参照してください。

主要なディレクトリ：

- `Assets/OpenFitter/Editor/` - Unity Editor拡張のC#コード
- `docs/` - VitePressドキュメント
- `scripts/` - ビルドと開発ツール
- `BlenderTools/` - ランタイムにダウンロードされるBlenderと依存関係（リポジトリには含まれない）

## 開発ワークフロー

### 1. 機能ブランチの作成

```bash
git checkout -b feature/your-feature-name
```

### 2. コードの変更

- C#コードは`Assets/OpenFitter/Editor/`配下に配置
- MVPアーキテクチャに従う（詳細は[アーキテクチャ](./architecture.md)を参照）
- ローカリゼーション文字列は`I18n.cs`に追加

### 3. テストの実行

```bash
# Unity内でTest Runnerを使用してテストを実行
# Window > General > Test Runner
```

### 4. ローカリゼーション検証

```bash
npm run check-l10n
```

### 5. ドキュメントの更新

機能を追加した場合は、適切なドキュメントを更新してください：

```bash
npm run docs:dev
```

ブラウザで `http://localhost:5173` を開いてプレビューを確認できます。

### 6. パッケージのビルド

```bash
npm run create-package
```

プロジェクトルートに`OpenFitter.unitypackage`が生成されます。

### 7. コミットとプルリクエスト

```bash
git add .
git commit -m "Add: your feature description"
git push origin feature/your-feature-name
```

GitHubでプルリクエストを作成してください。

## コーディング規約

### C#スタイルガイド

- **命名規則**:
  - クラス名: PascalCase（例: `OpenFitterWindow`）
  - メソッド名: PascalCase（例: `Initialize`）
  - プライベートフィールド: camelCase（例: `currentStep`）
  - パブリックプロパティ: PascalCase（例: `CurrentStep`）
  - インターフェース: I + PascalCase（例: `IWizardView`）

- **アーキテクチャ**:
  - MVP（Model-View-Presenter）パターンに従う
  - ビューとビジネスロジックを分離する
  - インターフェースを使用して疎結合を維持する

- **ローカリゼーション**:
  - UI文字列はすべて`I18n.cs`を通じて取得する
  - ハードコードされた文字列を使用しない

### コメント

- パブリックAPIにはXMLドキュメントコメントを追加
- 複雑なロジックには説明コメントを追加
- TODOコメントには理由と期限を含める

## テスト

### ユニットテスト

- `Assets/OpenFitter/Editor/Tests/`にテストを配置
- Unity Test Frameworkを使用
- 重要なビジネスロジックはテストでカバーする

### 手動テスト

新しい機能を追加した場合は、以下のシナリオで手動テストを実施してください：

1. 環境セットアップの実行
2. 様々なアバターモデルでのフィッティング
3. ブレンドシェイプ設定のテスト
4. エラーハンドリングの確認

## ドキュメント

### VitePressドキュメント

ドキュメントは`docs/`ディレクトリにMarkdown形式で記述されています。

- 新機能を追加した場合は、対応するドキュメントを更新
- スクリーンショットやコード例を含める
- 日本語と英語の両方を提供

## デバッグ

### Unity Editorでのデバッグ

- Visual Studio CodeまたはVisual Studioを使用
- Unity Debuggerを接続してブレークポイントを設定
- Unityコンソールでログを確認

### Blenderプロセスのデバッグ

- `OpenFitterCommandRunner.cs`で生成されるコマンドラインを確認
- Pythonスクリプト（`Assets/OpenFitter/Editor/Resources/`）に`print`文を追加
- Blenderのログファイルを確認

## コントリビューション

貢献を歓迎します！詳細は以下のページを参照してください：

- [アーキテクチャ](./architecture.md) - システム設計の理解
- [コア技術](./technology.md) - 使用している技術スタック
- [プロジェクト構造](./project-structure.md) - ファイルとディレクトリの構成
- [ビルド方法](./building.md) - パッケージのビルドと配布

## コミュニティ

- [GitHub Discussions](https://github.com/Mushus/OpenFitterUnityUI/discussions) - 質問や議論
- [Issue Tracker](https://github.com/Mushus/OpenFitterUnityUI/issues) - バグ報告や機能リクエスト

## ライセンス

このプロジェクトはMIT Licenseで公開されています。貢献したコードもMIT Licenseの下で公開されます。詳細は[ライセンス](../license.md)ページを参照してください。

---

## English

# Development Guide

This guide provides information for contributing to OpenFitter development.

## Introduction

OpenFitter is an open-source project developed as a Unity Editor extension. This guide covers the project structure, architecture, and development workflow.

## Target Audience

This documentation is intended for:

- Developers who want to add features or improvements to OpenFitter
- Contributors who want to fix bugs
- Those who want to deepen their understanding of the codebase
- Those who want to fork and create their own extensions

## Development Environment Setup

### Required Software

- **Unity 2022.3 or later**: For project development and testing
- **Node.js 18 or later**: For documentation builds and script execution
- **npm**: Package management and build tools
- **Git**: Version control

### Clone the Repository

```bash
git clone https://github.com/Mushus/OpenFitterUnityUI.git
cd OpenFitter
```

### Install Dependencies

```bash
npm install
```

### Open Project in Unity

1. Select "Open" in Unity Hub
2. Select the cloned repository directory
3. Open with Unity 2022.3 or later

## Project Structure

For detailed project structure, see the [Project Structure](./project-structure.md) page.

Key directories:

- `Assets/OpenFitter/Editor/` - Unity Editor extension C# code
- `docs/` - VitePress documentation
- `scripts/` - Build and development tools
- `BlenderTools/` - Blender and dependencies downloaded at runtime (not included in repository)

## Development Workflow

### 1. Create a Feature Branch

```bash
git checkout -b feature/your-feature-name
```

### 2. Make Code Changes

- Place C# code under `Assets/OpenFitter/Editor/`
- Follow MVP architecture (see [Architecture](./architecture.md) for details)
- Add localization strings to `I18n.cs`

### 3. Run Tests

```bash
# Use Test Runner in Unity to run tests
# Window > General > Test Runner
```

### 4. Validate Localization

```bash
npm run check-l10n
```

### 5. Update Documentation

If you add features, update the appropriate documentation:

```bash
npm run docs:dev
```

Open `http://localhost:5173` in your browser to preview.

### 6. Build Package

```bash
npm run create-package
```

This generates `OpenFitter.unitypackage` in the project root.

### 7. Commit and Pull Request

```bash
git add .
git commit -m "Add: your feature description"
git push origin feature/your-feature-name
```

Create a pull request on GitHub.

## Coding Conventions

### C# Style Guide

- **Naming Conventions**:
  - Class names: PascalCase (e.g., `OpenFitterWindow`)
  - Method names: PascalCase (e.g., `Initialize`)
  - Private fields: camelCase (e.g., `currentStep`)
  - Public properties: PascalCase (e.g., `CurrentStep`)
  - Interfaces: I + PascalCase (e.g., `IWizardView`)

- **Architecture**:
  - Follow MVP (Model-View-Presenter) pattern
  - Separate views from business logic
  - Maintain loose coupling using interfaces

- **Localization**:
  - Retrieve all UI strings through `I18n.cs`
  - Do not use hardcoded strings

### Comments

- Add XML documentation comments to public APIs
- Add explanatory comments to complex logic
- Include reasons and deadlines in TODO comments

## Testing

### Unit Tests

- Place tests in `Assets/OpenFitter/Editor/Tests/`
- Use Unity Test Framework
- Cover important business logic with tests

### Manual Testing

When adding new features, perform manual testing with the following scenarios:

1. Run environment setup
2. Fit clothing on various avatar models
3. Test blendshape configuration
4. Verify error handling

## Documentation

### VitePress Documentation

Documentation is written in Markdown format in the `docs/` directory.

- Update corresponding documentation when adding new features
- Include screenshots and code examples
- Provide both Japanese and English versions

## Debugging

### Debugging in Unity Editor

- Use Visual Studio Code or Visual Studio
- Connect Unity Debugger and set breakpoints
- Check logs in Unity Console

### Debugging Blender Process

- Check command line generated in `OpenFitterCommandRunner.cs`
- Add `print` statements to Python scripts (`Assets/OpenFitter/Editor/Resources/`)
- Check Blender log files

## Contributing

Contributions are welcome! See the following pages for details:

- [Architecture](./architecture.md) - Understanding system design
- [Core Technology](./technology.md) - Technology stack used
- [Project Structure](./project-structure.md) - File and directory organization
- [Building](./building.md) - Package building and distribution

## Community

- [GitHub Discussions](https://github.com/Mushus/OpenFitterUnityUI/discussions) - Questions and discussions
- [Issue Tracker](https://github.com/Mushus/OpenFitterUnityUI/issues) - Bug reports and feature requests

## License

This project is released under the MIT License. Contributed code will also be released under the MIT License. See the [License](../license.md) page for details.
