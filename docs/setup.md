# セットアップガイド

OpenFitterUnityUIの導入とセットアップ方法を詳しく説明します。

## 必要要件

OpenFitterを使用するには、以下の環境が必要です：

### ソフトウェア要件

- **Unity 2022.3以降**: Unity Editor（LTS版推奨）
- **Windows OS**: Blender自動化のためにWindows環境が必要
- **インターネット接続**: 初回セットアップ時のダウンロードに必要
- **空きディスク容量**: 約2GB以上（Blenderと依存関係のため）

### 推奨環境

- **Unity 2022.3 LTS**: 最も安定した動作
- **Windows 10/11**: 64bit版
- **8GB以上のRAM**: 大きなモデルを扱う場合
- **SSD**: 処理速度向上のため

## インストール方法

OpenFitterには2つのインストール方法があります。初めての方には **方法1（Unityパッケージ）** を推奨します。

### 方法1: Unityパッケージ（推奨）

最も簡単で推奨される方法です。

#### 1. パッケージのダウンロード

1. [GitHubのリリースページ](https://github.com/Mushus/OpenFitterUnityUI/releases)にアクセス
2. 最新リリースを選択
3. `OpenFitter.unitypackage`をダウンロード

#### 2. Unityプロジェクトへのインポート

1. Unityで対象のプロジェクトを開く
2. メニューから`Assets > Import Package > Custom Package...`を選択
3. ダウンロードした`OpenFitter.unitypackage`を選択
4. インポートダイアログで、すべての項目にチェックが入っていることを確認
5. `Import`ボタンをクリック



#### 3. インストールの確認

Unityのメニューバーに`Tools > OpenFitter`が表示されていれば、インストールは成功です。

### 方法2: 手動インストール

開発者や、ソースコードから直接インストールしたい場合の方法です。

#### 1. リポジトリのクローン

```bash
git clone https://github.com/Mushus/OpenFitterUnityUI.git
```

または、GitHubから[ZIPファイルをダウンロード](https://github.com/Mushus/OpenFitterUnityUI/archive/refs/heads/main.zip)して展開します。

#### 2. ファイルのコピー

1. ダウンロードまたはクローンしたフォルダ内の`Assets/OpenFitter`フォルダを見つける
2. Unityプロジェクトの`Assets`フォルダに`OpenFitter`フォルダをコピー

```
YourUnityProject/
└── Assets/
    └── OpenFitter/  ← ここにコピー
```

#### 3. Unityでの再コンパイル

1. Unityに戻る
2. Unityが自動的にスクリプトをコンパイルします
3. コンソールにエラーがないことを確認

## 初回セットアップ

インストール後、初めてOpenFitterを使用する前に環境セットアップが必要です。

### 1. OpenFitterウィンドウを開く

Unityメニューから`Tools > OpenFitter`を選択します。



### 2. 環境セットアップステップ

初回起動時、またはセットアップが完了していない場合、「環境セットアップ」ステップが表示されます。



#### セットアップ項目

以下の3つのコンポーネントが自動的にセットアップされます：

1. **Blender**: 3D処理エンジン（ポータブル版）
2. **OpenFitter Core**: コアフィッティングアルゴリズム
3. **Blenderアドオン**: 必要なBlenderアドオン

### 3. 「すべてインストール」の実行

1. 「すべてインストール」ボタンをクリック
2. ダウンロードとインストールが自動的に開始されます
3. 進捗バーで進行状況を確認できます




### 4. セットアップ完了の確認

すべてのセットアップ項目に緑色のチェックマークが表示されれば完了です。



## セットアップのトラブルシューティング

### ダウンロードが失敗する

**症状**: セットアップ中にエラーが発生する

**解決方法**:
1. インターネット接続を確認
2. ファイアウォールやアンチウイルスを確認
3. `BlenderTools/`フォルダを削除して再試行
4. 管理者としてUnityを実行

### ディスク容量不足

**症状**: 「ディスク容量が不足しています」エラー

**解決方法**:
1. 約2GB以上の空き容量を確保
2. 不要なファイルを削除
3. 別のドライブにプロジェクトを移動

### インストールが途中で止まる

**症状**: 進捗バーが長時間進まない

**解決方法**:
1. しばらく（5-10分）待つ
2. Unityコンソールでエラーを確認
3. Unityを再起動して再試行
4. 手動でコンポーネントをダウンロード

## 手動セットアップ（上級者向け）

自動セットアップが使えない場合や、特定のバージョンを使用したい場合の手動セットアップ方法です。

### 1. Blenderの手動インストール

1. [Blender公式サイト](https://www.blender.org/)からポータブル版をダウンロード
2. `BlenderTools/blender/`に展開
3. `blender.exe`が`BlenderTools/blender/blender.exe`にあることを確認

### 2. open-fitter-coreの手動インストール

1. [open-fitter-core](https://github.com/tallcat4/open-fitter-core)をダウンロード
2. `BlenderTools/open-fitter-core/`に配置

### 3. Blenderアドオンの手動インストール

詳細は[トラブルシューティング](./troubleshooting.md)ページを参照してください。

## アンインストール

OpenFitterをアンインストールする場合：

### 1. Unityパッケージの削除

1. Unityプロジェクトの`Assets/OpenFitter`フォルダを削除
2. Unityが自動的に関連するメタファイルも削除します

### 2. ダウンロードしたファイルの削除（オプション）

プロジェクトルートの`BlenderTools/`フォルダを削除します（約2GB）。

```
YourUnityProject/
└── BlenderTools/  ← 削除（オプション）
```

## アップデート

新しいバージョンのOpenFitterにアップデートする場合：

### 方法1: 新しいパッケージのインポート

1. 新しい`OpenFitter.unitypackage`をダウンロード
2. `Assets > Import Package > Custom Package...`からインポート
3. 「Replace」オプションを選択して既存のファイルを上書き

### 方法2: 手動アップデート

1. `Assets/OpenFitter`フォルダを削除
2. 新しいバージョンの`OpenFitter`フォルダをコピー

**注意**: 設定ファイルが上書きされる可能性があるため、重要な設定はバックアップしてください。

## 次のステップ

セットアップが完了したら、[基本的な使い方](./usage.md)ガイドで実際にフィッティングを試してみましょう。

---

## English

# Setup Guide

Detailed instructions for installing and setting up OpenFitter.

## Requirements

To use OpenFitter, you need the following environment:

### Software Requirements

- **Unity 2022.3 or later**: Unity Editor (LTS version recommended)
- **Windows OS**: Required for Blender automation
- **Internet Connection**: Required for initial setup downloads
- **Free Disk Space**: At least 2GB (for Blender and dependencies)

### Recommended Environment

- **Unity 2022.3 LTS**: Most stable operation
- **Windows 10/11**: 64-bit
- **8GB+ RAM**: For handling large models
- **SSD**: For improved processing speed

## Installation Methods

OpenFitter offers two installation methods. **Method 1 (Unity Package)** is recommended for first-time users.

### Method 1: Unity Package (Recommended)

The easiest and recommended method.

#### 1. Download Package

1. Visit [GitHub releases page](https://github.com/Mushus/OpenFitterUnityUI/releases)
2. Select the latest release
3. Download `OpenFitter.unitypackage`

#### 2. Import to Unity Project

1. Open your target project in Unity
2. Select `Assets > Import Package > Custom Package...` from menu
3. Select the downloaded `OpenFitter.unitypackage`
4. Verify all items are checked in the import dialog
5. Click `Import` button



#### 3. Verify Installation

If `Tools > OpenFitter` appears in Unity's menu bar, installation was successful.

### Method 2: Manual Installation

For developers or those who want to install directly from source code.

#### 1. Clone Repository

```bash
git clone https://github.com/Mushus/OpenFitterUnityUI.git
```

Or [download ZIP file from GitHub](https://github.com/Mushus/OpenFitterUnityUI/archive/refs/heads/main.zip) and extract.

#### 2. Copy Files

1. Find the `Assets/OpenFitter` folder in the downloaded or cloned folder
2. Copy the `OpenFitter` folder to your Unity project's `Assets` folder

```
YourUnityProject/
└── Assets/
    └── OpenFitter/  ← Copy here
```

#### 3. Recompile in Unity

1. Return to Unity
2. Unity will automatically compile scripts
3. Verify no errors in console

## Initial Setup

After installation, environment setup is required before first use of OpenFitter.

### 1. Open OpenFitter Window

Select `Tools > OpenFitter` from Unity menu.



### 2. Environment Setup Step

On first launch or if setup is not complete, the "Environment Setup" step will be displayed.



#### Setup Items

The following three components will be automatically set up:

1. **Blender**: 3D processing engine (portable version)
2. **OpenFitter Core**: Core fitting algorithm
3. **Blender Add-ons**: Required Blender add-ons

### 3. Execute "Install All"

1. Click "Install All" button
2. Download and installation will start automatically
3. Progress can be monitored via progress bar




During installation:
- Maintain internet connection
- Do not close Unity
- Do not put PC in sleep mode

### 4. Verify Setup Completion

Setup is complete when all setup items show green checkmarks.



## Setup Troubleshooting

### Download Fails

**Symptom**: Error occurs during setup

**Solution**:
1. Check internet connection
2. Check firewall and antivirus
3. Delete `BlenderTools/` folder and retry
4. Run Unity as administrator

### Insufficient Disk Space

**Symptom**: "Insufficient disk space" error

**Solution**:
1. Ensure at least 2GB free space
2. Delete unnecessary files
3. Move project to different drive

### Installation Stops Midway

**Symptom**: Progress bar doesn't advance for long time

**Solution**:
1. Wait a while (5-10 minutes)
2. Check Unity console for errors
3. Restart Unity and retry
4. Manually download components

## Manual Setup (Advanced)

Manual setup method for cases where automatic setup is unavailable or specific versions are desired.

### 1. Manual Blender Installation

1. Download portable version from [Blender official site](https://www.blender.org/)
2. Extract to `BlenderTools/blender/`
3. Verify `blender.exe` is at `BlenderTools/blender/blender.exe`

### 2. Manual open-fitter-core Installation

1. Download [open-fitter-core](https://github.com/tallcat4/open-fitter-core)
2. Place in `BlenderTools/open-fitter-core/`

### 3. Manual Blender Add-on Installation

See [Troubleshooting](./troubleshooting.md) page for details.

## Uninstallation

To uninstall OpenFitter:

### 1. Delete Unity Package

1. Delete `Assets/OpenFitter` folder in Unity project
2. Unity will automatically delete related meta files

### 2. Delete Downloaded Files (Optional)

Delete `BlenderTools/` folder in project root (approx. 2GB).

```
YourUnityProject/
└── BlenderTools/  ← Delete (optional)
```

## Updating

To update to a new version of OpenFitter:

### Method 1: Import New Package

1. Download new `OpenFitter.unitypackage`
2. Import via `Assets > Import Package > Custom Package...`
3. Select "Replace" option to overwrite existing files

### Method 2: Manual Update

1. Delete `Assets/OpenFitter` folder
2. Copy new version of `OpenFitter` folder

**Note**: Configuration files may be overwritten, so backup important settings.

## Next Steps

Once setup is complete, try actual fitting with the [Basic Usage](./usage.md) guide.
