# OpenFitterUnityUI

[日本語](#japanese) | [English](#english)

<a name="japanese"></a>

## 概要

OpenFitterUnityUIは、3Dアバターモデルへの衣装の自動フィッティングを行うオープンソースのUnity Editorツールです。VRChatアバタークリエイターやアバター改変者が、異なる体型間で衣装を転送するための合理化されたワークフローを提供します。

Nine GatesのGPL-3ライセンスコンポーネントを基に構築されたOpenFitterUnityUIは、RBF（放射基底関数）変形とボーンポーズ転送を使用した複雑な衣装フィッティングプロセスを自動化する、使いやすいUnity Editorインターフェースを提供します。

## 機能

- **自動衣装フィッティング**: 異なる体型のアバター間で衣装を転送
- **Unity Editor統合**: Unity内で直接動作する直感的なウィザードベースのインターフェース
- **自動環境セットアップ**: Blender、Python依存関係、必要なアドオンを自動的にダウンロード・設定
- **ブレンドシェイプ管理**: フィッティングプロセス中にカスタムブレンドシェイプを設定・保持
- **進捗トラッキング**: フィッティング操作中のリアルタイム進捗監視
- **データフォーマット互換性**: MochiFitterのデータフォーマットに対応

## 必要要件

- Unity 2022.3以降
- Windows OS（Blender自動化のため）
- インターネット接続（初期セットアップとダウンロードのため）
- 約2GBの空きディスク容量（Blenderと依存関係のため）

## インストール

### 方法1: Unityパッケージ（推奨）

1. [GitHubリリース](https://github.com/Mushus/OpenFitterUnityUI/releases)から最新の`OpenFitterUnityUI.unitypackage`をダウンロード
2. `Assets > Import Package > Custom Package`からUnityプロジェクトにパッケージをインポート
3. `Tools > OpenFitter`にOpenFitterUnityUIメニューが表示されます

### 方法2: 手動インストール

1. このリポジトリをクローンまたはダウンロード
2. `Assets/OpenFitter`フォルダをUnityプロジェクトの`Assets`フォルダにコピー
3. Unityが自動的にエディタースクリプトをコンパイルします

## クイックスタート

1. Unity内で`Tools > OpenFitter`からOpenFitterウィンドウを開く
2. セットアップウィザードに従います：
   - **環境セットアップ**: 「すべてインストール」をクリックして、Blenderと依存関係を自動的にセットアップ
   - **ソース選択**: ソースアバター（衣装を持つアバター）を選択
   - **ターゲット選択**: ターゲットアバター（衣装を受け取るアバター）を選択
   - **ブレンドシェイプ設定**: （オプション）保持するブレンドシェイプを設定
   - **実行**: フィッティングプロセスを実行
3. プロセスが完了するまで待ち、出力を確認

詳細な手順については、[使い方ガイド](./docs/usage.md)を参照してください。

## ドキュメント

詳細なドキュメントは[docsディレクトリ](./docs/)にあります：

- **利用者向け**:
  - [セットアップガイド](./docs/setup.md) - インストールと初期設定
  - [使い方](./docs/usage.md) - 基本的な操作方法
  - [トラブルシューティング](./docs/troubleshooting.md) - よくある問題と解決方法
  - [ライセンス](./docs/license.md) - ライセンス情報

- **開発者向け**:
  - [開発ガイド](./docs/development/) - 開発環境のセットアップ
  - [アーキテクチャ](./docs/development/architecture.md) - システム設計
  - [コア技術](./docs/development/technology.md) - 使用技術
  - [プロジェクト構造](./docs/development/project-structure.md) - ファイル構成
  - [ビルド方法](./docs/development/building.md) - パッケージのビルド

### ドキュメントサイトの起動

```bash
npm install
npm run docs:dev
```

[http://localhost:5173](http://localhost:5173)にアクセスしてドキュメントを閲覧してください。

## 貢献

貢献を歓迎します！詳細は[開発ガイド](./docs/development/)を参照してください。

1. リポジトリをフォーク
2. 機能ブランチを作成
3. 変更を加える
4. すべてのテストが通ることを確認
5. プルリクエストを送信

## ライセンス

このプロジェクトは **MIT License** でライセンスされています。

ただし、実行時に自動ダウンロードされる以下のコンポーネントは、それぞれ独自のライセンスの下で配布されています：
- **Blender**: GPL-2.0 License
- **open-fitter-core**: GPL-3.0 License ([Tallcat4's Repository](https://github.com/tallcat4/open-fitter-core))
- **Blenderアドオン**: 各種ライセンス

詳細な情報（コンプライアンス、開発方針、免責事項など）については、[ライセンスドキュメント](./docs/license.md)を参照してください。

## 謝辞

- GPL-3ソースコードコンポーネントを公開してくださったNine Gates
- オープンソースのコアアルゴリズム実装を提供する[open-fitter-core](https://github.com/tallcat4/open-fitter-core)プロジェクトのtallcat4氏
- Blenderソフトウェアを提供するBlender Foundation
- インスピレーションとフィードバックをくださったVRChatコミュニティ

## リンク

- [ドキュメント](./docs/)
- [コアアルゴリズム（ローカル）](./BlenderTools/open-fitter-core/README.md)
- [open-fitter-core (GitHub)](https://github.com/tallcat4/open-fitter-core)
- [Issue Tracker](https://github.com/Mushus/OpenFitterUnityUI/issues)
- [GitHub Discussions](https://github.com/Mushus/OpenFitterUnityUI/discussions)

---

<a name="english"></a>

## Overview

OpenFitterUnityUI is an open-source Unity Editor tool for automatically fitting clothing onto 3D avatar models. It provides a streamlined workflow for VRChat avatar creators and avatar modifiers to transfer clothing between different body shapes.

Built on GPL-3 licensed components from Nine Gates, OpenFitterUnityUI offers a user-friendly Unity Editor interface that automates the complex process of clothing fitting using RBF (Radial Basis Function) deformation and bone pose transfer.

## Features

- **Automated Clothing Fitting**: Transfer clothing from one avatar to another with different body shapes
- **Unity Editor Integration**: Intuitive wizard-based interface directly in Unity
- **Automatic Environment Setup**: Automatically downloads and configures Blender, Python dependencies, and required add-ons
- **BlendShape Management**: Configure and preserve custom blendshapes during the fitting process
- **Progress Tracking**: Real-time progress monitoring during fitting operations
- **Data Format Compatibility**: Compatible with MochiFitter data formats

## Requirements

- Unity 2022.3 or later
- Windows operating system (for Blender automation)
- Internet connection (for initial setup and downloads)
- Approximately 2GB of free disk space (for Blender and dependencies)

## Installation

### Method 1: Unity Package (Recommended)

1. Download the latest `OpenFitterUnityUI.unitypackage` from [GitHub Releases](https://github.com/Mushus/OpenFitterUnityUI/releases)
2. Import the package into your Unity project via `Assets > Import Package > Custom Package`
3. The OpenFitterUnityUI menu will appear under `Tools > OpenFitter`

### Method 2: Manual Installation

1. Clone or download this repository
2. Copy the `Assets/OpenFitter` folder into your Unity project's `Assets` folder
3. Unity will automatically compile the editor scripts

## Quick Start

1. Open the OpenFitter window via `Tools > OpenFitter` in Unity
2. Follow the setup wizard:
   - **Environment Setup**: Click "Install All" to automatically set up Blender and dependencies
   - **Source Selection**: Select your source avatar (the one with the clothing)
   - **Target Selection**: Select your target avatar (the one to receive the clothing)
   - **BlendShape Configuration**: (Optional) Configure blendshapes to preserve
   - **Execute**: Run the fitting process
3. Wait for the process to complete and review the output

For detailed instructions, see the [Usage Guide](./docs/usage.md).

## Documentation

Detailed documentation is available in the [docs directory](./docs/):

- **For Users**:
  - [Setup Guide](./docs/setup.md) - Installation and initial setup
  - [Usage](./docs/usage.md) - Basic operation
  - [Troubleshooting](./docs/troubleshooting.md) - Common issues and solutions
  - [License](./docs/license.md) - License information

- **For Developers**:
  - [Development Guide](./docs/development/) - Development environment setup
  - [Architecture](./docs/development/architecture.md) - System design
  - [Core Technology](./docs/development/technology.md) - Technologies used
  - [Project Structure](./docs/development/project-structure.md) - File organization
  - [Building](./docs/development/building.md) - Package building

### Running Documentation Site

```bash
npm install
npm run docs:dev
```

Visit [http://localhost:5173](http://localhost:5173) to browse the documentation.

## Contributing

Contributions are welcome! See the [Development Guide](./docs/development/) for details.

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Ensure all tests pass
5. Submit a pull request

## License

This project is licensed under the **MIT License**.

However, the following components automatically downloaded at runtime are distributed under their own respective licenses:
- **Blender**: GPL-2.0 License
- **open-fitter-core**: GPL-3.0 License ([Tallcat4's Repository](https://github.com/tallcat4/open-fitter-core))
- **Blender Add-ons**: Various licenses

For detailed information (compliance, development policy, disclaimer, etc.), see the [License Documentation](./docs/license.md).

## Acknowledgements

- Nine Gates for releasing the GPL-3 source code components
- tallcat4 for the [open-fitter-core](https://github.com/tallcat4/open-fitter-core) project, providing the open-source core algorithm implementation
- The Blender Foundation for the Blender software
- The VRChat community for inspiration and feedback

## Links

- [Documentation](./docs/)
- [Core Algorithm (Local)](./BlenderTools/open-fitter-core/README.md)
- [open-fitter-core (GitHub)](https://github.com/tallcat4/open-fitter-core)
- [Issue Tracker](https://github.com/Mushus/OpenFitterUnityUI/issues)
- [GitHub Discussions](https://github.com/Mushus/OpenFitterUnityUI/discussions)
