# コア技術

OpenFitterで使用されている主要な技術とアルゴリズムについて説明します。

## 概要

OpenFitterは、3Dアバター間での衣装フィッティングを実現するために、複数の技術を組み合わせています。Unity Editor拡張としてのフロントエンドと、Blenderをバックエンドとして使用する処理エンジンで構成されています。

## フィッティングアルゴリズム

### RBF（放射基底関数）変形

コアフィッティングアルゴリズムは **RBF（Radial Basis Function）変形** を使用しています。

#### RBFとは

RBFは、限られた制御点から滑らかな変形を生成する数学的手法です。以下のような特徴があります：

- **局所的な影響**: 各制御点は周囲の領域に主に影響を与えます
- **滑らかな補間**: 制御点間の変形が自然で滑らかです
- **非線形変形**: 複雑な体型の違いにも対応できます

#### フィッティングプロセスでの使用

1. **制御点の設定**: ソースとターゲットのボーン位置を制御点として使用
2. **RBF計算**: ソースからターゲットへの変形を計算
3. **メッシュ変形**: 衣装メッシュの各頂点にRBF変形を適用

### ボーンポーズ転送

アバター間でボーンの姿勢（ポーズ）を転送します。

#### プロセス

1. **ボーンマッピング**: ソースとターゲットのボーン構造を対応付け
2. **ポーズ抽出**: ソースアバターのボーンポーズを抽出
3. **ポーズ適用**: ターゲットアバターにポーズを適用
4. **ウェイト転送**: スキニングウェイトを適切に転送

### メッシュトポロジーの保持

フィッティング後もメッシュの基本構造を維持します：

- **頂点順序**: 元の頂点順序を保持
- **ポリゴン構造**: ポリゴンの接続関係を維持
- **UV座標**: テクスチャ座標を保持
- **マテリアル**: マテリアル割り当てを維持

## Blender統合

### Blenderをバックエンドとして使用する理由

OpenFitterはBlenderを3D処理エンジンとして使用しています：

- **強力な3D処理**: Blenderの成熟した3D処理機能を活用
- **Pythonスクリプティング**: 柔軟な自動化が可能
- **GPL-3コードの互換性**: open-fitter-coreのコードをそのまま使用可能
- **オープンソース**: 無料で再配布可能

### ポータブルBlender

ユーザーの既存環境に影響を与えないよう、ポータブル版のBlenderを使用：

- **独立したインストール**: システムのBlenderと干渉しない
- **自動ダウンロード**: 初回セットアップ時に自動取得
- **バージョン管理**: 特定のバージョンを使用して互換性を保証

### Pythonラッパースクリプト

Unity（C#）とBlender（Python）間の橋渡しを行います：

#### openfitter_wrapper.py

メインのラッパースクリプト：

```python
# コマンドライン引数からパラメータを受け取る
# open-fitter-coreのフィッティング関数を呼び出す
# 進捗情報を標準出力に出力
# 結果をファイルに保存
```

#### check_addon_status.py

アドオンのインストール状態を確認：

```python
# Blenderアドオンが正しくインストールされているか確認
# バージョン情報を取得
# 結果をJSON形式で出力
```

#### install_addon_dependencies.py

アドオンの依存関係をインストール：

```python
# pipを使用して必要なPythonパッケージをインストール
# Blenderの内蔵Python環境にインストール
# インストール結果を報告
```

### コマンドライン実行

UnityからBlenderをコマンドラインで起動：

```bash
blender.exe --background --python openfitter_wrapper.py -- [arguments]
```

- `--background`: UIなしのバックグラウンドモード
- `--python`: 実行するPythonスクリプト
- `--`: 以降の引数をスクリプトに渡す

### OpenFitterCore パラメータ

Blenderバックエンド（`openfitter_wrapper.py`を介したコアスクリプト）が受け取る主要な引数は以下の通りです。

| 引数 | 形式 | 必須 | 説明 |
| :--- | :--- | :---: | :--- |
| `--input` | パス | YES | 入力となる衣装FBXの絶対パス。 |
| `--output` | パス | YES | 出力先FBXの絶対パス。 |
| `--base` | パス | YES | 基準となる `.blend` ファイルのパス。 |
| `--base-fbx` | パスリスト | YES | ベースキャラクターのFBXパス。複数ある場合は `;` で区切ります。 |
| `--config` | パスリスト | YES | 設定JSONのパス。複数ある場合は `;` で区切ります。 |
| `--init-pose` | パス | YES | 初期ポーズ情報のJSONパス。 |
| `--hips-position` | 文字列 | NO | Hipsボーンの位置指定。 |
| `--blend-shapes` | 文字列リスト | NO | 適用するブレンドシェイプ名のリスト（`;` 区切り）。 |
| `--blend-shape-values` | 数値リスト | NO | 各ブレンドシェイプの適用値（`;` 区切り）。 |
| `--blend-shape-mappings`| 文字列リスト | NO | ブレンドシェイプのマッピング定義。 |
| `--target-meshes` | 文字列リスト | NO | 処理対象とするメッシュ名のリスト。 |
| `--mesh-renderers` | 文字列リスト | NO | メッシュレンダラーの対応設定。 |
| `--name-conv` | 文字列リスト | NO | 名称変換ルールのリスト。 |
| `--preserve-bone-names`| フラグ | NO | ボーン名を維持するかどうかの指定。 |
| `--no-subdivision` | フラグ | NO | メッシュの自動細分化を無効にします。 |
| `--no-triangle` | フラグ | NO | 三角面化を無効にします。 |

## Unity Editor統合

### UIToolkit

モダンなUI開発フレームワークを使用：

#### UXML（UIレイアウト）

XMLベースのレイアウト記述：

```xml
<ui:UXML>
    <ui:VisualElement class="step-container">
        <ui:Label text="Source Selection" class="step-title"/>
        <ui:ObjectField name="source-field" type="UnityEngine.GameObject"/>
    </ui:VisualElement>
</ui:UXML>
```

#### USS（スタイル）

CSSライクなスタイリング：

```css
.step-container {
    padding: 10px;
    margin: 5px;
}

.step-title {
    font-size: 16px;
    -unity-font-style: bold;
}
```

#### C#バインディング

C#からUIを操作：

```csharp
var sourceField = rootElement.Q<ObjectField>("source-field");
sourceField.RegisterValueChangedCallback(evt => {
    // 値変更時の処理
});
```

### 非同期処理

長時間実行される操作には非同期処理を使用：

```csharp
public async Task<bool> ExecuteFittingAsync()
{
    await Task.Run(() => {
        // Blenderプロセスの実行
    });
    return true;
}
```

### 進捗パーシング

Blenderからの出力をパースして進捗を表示：

```csharp
public class FittingProgressParser
{
    public void ParseLine(string line)
    {
        // "PROGRESS: 50%" のような出力をパース
        // 進捗イベントを発火
    }
}
```

## データ形式

### FBXフォーマット

入出力に使用する3Dモデルフォーマット：

- **互換性**: 多くの3Dツールでサポート
- **完全な情報**: メッシュ、ボーン、アニメーション、マテリアルを含む
- **Unity統合**: Unityのネイティブサポート

### JSON設定ファイル

フィッティング設定の保存：

```json
{
  "sourceAvatar": "Assets/Avatars/Source.fbx",
  "targetAvatar": "Assets/Avatars/Target.fbx",
  "blendShapes": [
    {"name": "vrc.blink_left", "preserve": true},
    {"name": "vrc.blink_right", "preserve": true}
  ]
}
```

### MochiFitterデータ互換性

MochiFitterのデータフォーマットに準拠：

- **設定ファイル**: 同じJSON構造を使用
- **出力形式**: 互換性のあるFBX出力
- **ブレンドシェイプ命名**: VRChat標準に準拠

## 最適化技術

### 並列処理

複数のタスクを並列実行：

```csharp
var tasks = new List<Task<SetupResult>>
{
    downloadBlenderTask,
    downloadCoreTask,
    downloadAddonTask
};

await Task.WhenAll(tasks);
```

### キャッシング

ダウンロードしたファイルをキャッシュ：

- **ローカルキャッシュ**: `BlenderTools/`ディレクトリに保存
- **バージョンチェック**: ファイルが最新か確認
- **再利用**: 既にダウンロード済みの場合はスキップ

### メモリ管理

大きなメッシュデータの効率的な処理：

- **ストリーミング**: ファイルをストリーミングで読み込み
- **早期解放**: 不要になったデータは即座に解放
- **プロセス分離**: Blenderを別プロセスで実行してUnityのメモリ使用を抑制

## セキュリティ

### サンドボックス化

Blenderプロセスを制限：

- **別プロセス**: Unityとは独立したプロセスで実行
- **作業ディレクトリ制限**: 特定のディレクトリ内でのみ動作
- **タイムアウト**: 長時間実行をタイムアウトで制限

### 入力検証

ユーザー入力の検証：

```csharp
public bool ValidateSourceAvatar(GameObject avatar)
{
    if (avatar == null) return false;
    if (!HasValidMesh(avatar)) return false;
    if (!HasValidArmature(avatar)) return false;
    return true;
}
```

## 国際化（i18n）

### ローカリゼーションシステム

多言語サポート：

```csharp
public static class I18n
{
    public static string Get(string key)
    {
        // 現在の言語設定に基づいて文字列を取得
        return translations[currentLanguage][key];
    }
}
```

### サポート言語

- 日本語
- 英語

## テスト技術

### Unity Test Framework

ユニットテストとインテグレーションテスト：

```csharp
[Test]
public void TestSourceSelection()
{
    var presenter = new SourceSelectionStepPresenter(mockView, state);
    presenter.OnSourceSelected(testAvatar);
    Assert.AreEqual(testAvatar, state.Config.SourceAvatar);
}
```

### モックオブジェクト

テスト用のモック実装：

```csharp
public class MockWizardView : IWizardView
{
    public bool NextButtonWasCalled { get; private set; }

    public void ShowNextButton()
    {
        NextButtonWasCalled = true;
    }
}
```

## ビルドとデプロイ

### TypeScriptビルドスクリプト

Node.jsとTypeScriptでビルドツールを作成：

```typescript
// create-unitypackage.ts
import * as fs from 'fs';
import * as path from 'path';

async function createUnityPackage() {
    // Unityパッケージを作成する処理
}
```

### VitePressドキュメント

ドキュメントサイトの生成：

```bash
npm run docs:build
# docs/.vitepress/dist/にビルド結果が生成される
```

## まとめ

OpenFitterは以下の技術を組み合わせて実現されています：

- **RBF変形**: 滑らかで自然な衣装フィッティング
- **Blender統合**: 強力な3D処理エンジン
- **Unity Editor拡張**: ユーザーフレンドリーなインターフェース
- **非同期処理**: レスポンシブなユーザー体験
- **モジュラー設計**: 保守しやすく拡張可能な構造

これらの技術により、高品質な自動衣装フィッティングを実現しています。

---

## English

# Core Technology

This document explains the key technologies and algorithms used in OpenFitter.

## Overview

OpenFitter combines multiple technologies to achieve clothing fitting between 3D avatars. It consists of a frontend as a Unity Editor extension and a processing engine using Blender as a backend.

## Fitting Algorithm

### RBF (Radial Basis Function) Deformation

The core fitting algorithm uses **RBF (Radial Basis Function) deformation**.

#### What is RBF

RBF is a mathematical technique that generates smooth deformation from limited control points. It has the following characteristics:

- **Local Influence**: Each control point primarily affects its surrounding area
- **Smooth Interpolation**: Deformation between control points is natural and smooth
- **Nonlinear Deformation**: Can handle complex body shape differences

#### Use in Fitting Process

1. **Control Point Setup**: Use bone positions of source and target as control points
2. **RBF Calculation**: Calculate deformation from source to target
3. **Mesh Deformation**: Apply RBF deformation to each vertex of clothing mesh

### Bone Pose Transfer

Transfers bone pose between avatars.

#### Process

1. **Bone Mapping**: Map bone structures between source and target
2. **Pose Extraction**: Extract bone pose from source avatar
3. **Pose Application**: Apply pose to target avatar
4. **Weight Transfer**: Appropriately transfer skinning weights

### Mesh Topology Preservation

Maintains basic mesh structure after fitting:

- **Vertex Order**: Preserves original vertex order
- **Polygon Structure**: Maintains polygon connectivity
- **UV Coordinates**: Preserves texture coordinates
- **Materials**: Maintains material assignments

## Blender Integration

### Why Use Blender as Backend

OpenFitter uses Blender as its 3D processing engine:

- **Powerful 3D Processing**: Leverages Blender's mature 3D processing capabilities
- **Python Scripting**: Enables flexible automation
- **GPL-3 Code Compatibility**: Can use open-fitter-core code directly
- **Open Source**: Free and redistributable

### Portable Blender

Uses portable version of Blender to avoid affecting user's existing environment:

- **Independent Installation**: Doesn't interfere with system Blender
- **Automatic Download**: Automatically retrieved during initial setup
- **Version Management**: Uses specific version to ensure compatibility

### Python Wrapper Scripts

Bridges between Unity (C#) and Blender (Python):

#### openfitter_wrapper.py

Main wrapper script:

```python
# Receive parameters from command line arguments
# Call fitting function of open-fitter-core
# Output progress information to stdout
# Save results to file
```

#### check_addon_status.py

Check add-on installation status:

```python
# Check if Blender add-on is correctly installed
# Get version information
# Output results in JSON format
```

#### install_addon_dependencies.py

Install add-on dependencies:

```python
# Install required Python packages using pip
# Install into Blender's built-in Python environment
# Report installation results
```

### Command Line Execution

Launch Blender from Unity via command line:

```bash
blender.exe --background --python openfitter_wrapper.py -- [arguments]
```

- `--background`: Background mode without UI
- `--python`: Python script to execute
- `--`: Pass subsequent arguments to script

### OpenFitterCore Parameters

The primary arguments accepted by the Blender backend (the core script via `openfitter_wrapper.py`) are as follows:

| Argument | Format | Required | Description |
| :--- | :--- | :---: | :--- |
| `--input` | Path | YES | Absolute path to the input clothing FBX. |
| `--output` | Path | YES | Absolute path to the output FBX. |
| `--base` | Path | YES | Path to the base `.blend` file. |
| `--base-fbx` | Path List | YES | Path to the base character FBX. Semicolon-separated if multiple. |
| `--config` | Path List | YES | Path to the config JSON. Semicolon-separated if multiple. |
| `--init-pose` | Path | YES | Path to the initial pose JSON. |
| `--hips-position` | String | NO | Specifies the Hips bone position. |
| `--blend-shapes` | String List | NO | List of blend shape names to apply (semicolon-separated). |
| `--blend-shape-values` | Value List | NO | Application values for each blend shape (semicolon-separated). |
| `--blend-shape-mappings`| String List | NO | Blend shape mapping definitions. |
| `--target-meshes` | String List | NO | List of mesh names to be processed. |
| `--mesh-renderers` | String List | NO | Mesh renderer correspondence settings. |
| `--name-conv` | String List | NO | List of name conversion rules. |
| `--preserve-bone-names`| Flag | NO | Specifies whether to preserve bone names. |
| `--no-subdivision` | Flag | NO | Disables automatic mesh subdivision. |
| `--no-triangle` | Flag | NO | Disables triangulation. |

## Unity Editor Integration

### UIToolkit

Uses modern UI development framework:

#### UXML (UI Layout)

XML-based layout description:

```xml
<ui:UXML>
    <ui:VisualElement class="step-container">
        <ui:Label text="Source Selection" class="step-title"/>
        <ui:ObjectField name="source-field" type="UnityEngine.GameObject"/>
    </ui:VisualElement>
</ui:UXML>
```

#### USS (Styles)

CSS-like styling:

```css
.step-container {
    padding: 10px;
    margin: 5px;
}

.step-title {
    font-size: 16px;
    -unity-font-style: bold;
}
```

#### C# Binding

Manipulate UI from C#:

```csharp
var sourceField = rootElement.Q<ObjectField>("source-field");
sourceField.RegisterValueChangedCallback(evt => {
    // Handle value change
});
```

### Asynchronous Processing

Uses asynchronous processing for long-running operations:

```csharp
public async Task<bool> ExecuteFittingAsync()
{
    await Task.Run(() => {
        // Execute Blender process
    });
    return true;
}
```

### Progress Parsing

Parse output from Blender to display progress:

```csharp
public class FittingProgressParser
{
    public void ParseLine(string line)
    {
        // Parse output like "PROGRESS: 50%"
        // Fire progress event
    }
}
```

## Data Formats

### FBX Format

3D model format used for input/output:

- **Compatibility**: Supported by many 3D tools
- **Complete Information**: Includes mesh, bones, animations, materials
- **Unity Integration**: Native Unity support

### JSON Configuration Files

Save fitting configuration:

```json
{
  "sourceAvatar": "Assets/Avatars/Source.fbx",
  "targetAvatar": "Assets/Avatars/Target.fbx",
  "blendShapes": [
    {"name": "vrc.blink_left", "preserve": true},
    {"name": "vrc.blink_right", "preserve": true}
  ]
}
```

### MochiFitter Data Compatibility

Complies with MochiFitter data format:

- **Configuration Files**: Uses same JSON structure
- **Output Format**: Compatible FBX output
- **Blendshape Naming**: Complies with VRChat standards

## Optimization Techniques

### Parallel Processing

Execute multiple tasks in parallel:

```csharp
var tasks = new List<Task<SetupResult>>
{
    downloadBlenderTask,
    downloadCoreTask,
    downloadAddonTask
};

await Task.WhenAll(tasks);
```

### Caching

Cache downloaded files:

- **Local Cache**: Saved in `BlenderTools/` directory
- **Version Check**: Verify if files are up-to-date
- **Reuse**: Skip if already downloaded

### Memory Management

Efficient processing of large mesh data:

- **Streaming**: Stream file loading
- **Early Release**: Immediately release unnecessary data
- **Process Separation**: Run Blender in separate process to reduce Unity memory usage

## Security

### Sandboxing

Restrict Blender process:

- **Separate Process**: Runs independently from Unity
- **Working Directory Restriction**: Operates only within specific directory
- **Timeout**: Limit long-running execution with timeout

### Input Validation

Validate user input:

```csharp
public bool ValidateSourceAvatar(GameObject avatar)
{
    if (avatar == null) return false;
    if (!HasValidMesh(avatar)) return false;
    if (!HasValidArmature(avatar)) return false;
    return true;
}
```

## Internationalization (i18n)

### Localization System

Multi-language support:

```csharp
public static class I18n
{
    public static string Get(string key)
    {
        // Get string based on current language setting
        return translations[currentLanguage][key];
    }
}
```

### Supported Languages

- Japanese
- English

## Testing Technologies

### Unity Test Framework

Unit tests and integration tests:

```csharp
[Test]
public void TestSourceSelection()
{
    var presenter = new SourceSelectionStepPresenter(mockView, state);
    presenter.OnSourceSelected(testAvatar);
    Assert.AreEqual(testAvatar, state.Config.SourceAvatar);
}
```

### Mock Objects

Mock implementations for testing:

```csharp
public class MockWizardView : IWizardView
{
    public bool NextButtonWasCalled { get; private set; }

    public void ShowNextButton()
    {
        NextButtonWasCalled = true;
    }
}
```

## Build and Deploy

### TypeScript Build Scripts

Create build tools with Node.js and TypeScript:

```typescript
// create-unitypackage.ts
import * as fs from 'fs';
import * as path from 'path';

async function createUnityPackage() {
    // Process to create Unity package
}
```

### VitePress Documentation

Generate documentation site:

```bash
npm run docs:build
# Build results generated in docs/.vitepress/dist/
```

## Summary

OpenFitter is realized by combining the following technologies:

- **RBF Deformation**: Smooth and natural clothing fitting
- **Blender Integration**: Powerful 3D processing engine
- **Unity Editor Extension**: User-friendly interface
- **Asynchronous Processing**: Responsive user experience
- **Modular Design**: Maintainable and extensible structure

These technologies enable high-quality automatic clothing fitting.
