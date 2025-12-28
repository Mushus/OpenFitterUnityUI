# 基本的な使い方

OpenFitterを使用して衣装をフィッティングする詳細な手順を説明します。

## クイックスタート

初めてOpenFitterを使用する方のための最も基本的なワークフローです。

### 前提条件

- OpenFitterがインストールされていること
- 環境セットアップが完了していること
- ソースアバター（衣装を持つアバター）のFBXファイル
- ターゲットアバター（衣装を受け取るアバター）のFBXファイル

### ステップ1: OpenFitterウィンドウを開く

1. Unityメニューから `Tools > OpenFitter` を選択
2. OpenFitterウィザードウィンドウが開きます



## ウィザードの流れ

OpenFitterは以下のステップで構成されたウィザード形式です：

1. [環境セットアップ](#ステップ2-環境セットアップ)
2. [ソース選択](#ステップ3-ソース選択)
3. [ターゲット選択](#ステップ4-ターゲット選択)
4. [ブレンドシェイプ設定](#ステップ5-ブレンドシェイプ設定)
5. [高度なオプション](#ステップ6-高度なオプション)
6. [実行](#ステップ7-実行)
7. [完了](#ステップ8-完了)

### ステップ2: 環境セットアップ

初回または環境が未セットアップの場合、このステップが表示されます。



#### 操作

1. 各セットアップ項目の状態を確認：
   - **緑色のチェックマーク**: インストール済み
   - **黄色の警告**: 未インストールまたは更新が必要
   - **赤色のエラー**: インストール失敗

2. 「すべてインストール」ボタンをクリック（必要な場合）
3. すべて緑色になったら「次へ」をクリック

### ステップ3: ソース選択

フィッティングする衣装を持つアバター（ソースアバター）を選択します。



#### 操作

1. **ソースアバターの選択**:
   - オブジェクトフィールドをクリック
   - Projectウィンドウからソースアバターの FBXファイルをドラッグ&ドロップ
   - または、ピッカーボタンをクリックして選択

2. **検証**:
   - 選択したアバターが有効かどうか自動的に検証されます
   - エラーがある場合は、メッセージが表示されます

3. **次へ** をクリック

#### ソースアバターの要件

- 有効なFBXフォーマット
- アーマチュア（ボーン構造）を含む
- フィッティングしたい衣装メッシュを含む
- 適切にウェイトペイントされている

### ステップ4: ターゲット選択

衣装を受け取るアバター（ターゲットアバター）を選択します。



#### 操作

1. **ターゲットアバターの選択**:
   - ソース選択と同様の方法でFBXファイルを選択

2. **検証**:
   - ターゲットアバターが有効かどうか検証されます
   - ボーン構造の互換性がチェックされます

3. **次へ** をクリック

#### ターゲットアバターの要件

- 有効なFBXフォーマット
- アーマチュア（ボーン構造）を含む
- ソースアバターと互換性のあるボーン命名規則

### ステップ5: ブレンドシェイプ設定

フィッティング後に保持したいブレンドシェイプを設定します（オプション）。




### ステップ6: 高度なオプション

フィッティングパラメータを調整します（通常はデフォルトで問題ありません）。



#### 利用可能なオプション

- **RBF平滑化係数**: 変形の滑らかさを調整（デフォルト: 1.0）
- **ウェイト転送方法**: ウェイトの転送アルゴリズム
- **メッシュ最適化**: 出力メッシュの最適化レベル

#### 操作

1. 必要に応じてパラメータを調整
2. デフォルト値で問題ない場合はそのまま「次へ」をクリック

### ステップ7: 実行

フィッティングプロセスを実行します。



#### 操作

1. **設定の確認**:
   - 最終確認のために設定の概要が表示されます

2. **「実行」ボタンをクリック**:
   - フィッティングプロセスが開始されます

3. **進捗の監視**:
   - 進捗バーで進行状況を確認できます
   - ログウィンドウで詳細な処理内容を確認できます




処理中は：
- Unityを閉じないでください
- PCをスリープモードにしないでください
- 他の重い処理を実行しないでください

### ステップ8: 完了

フィッティングが完了しました。



#### 操作

1. **結果の確認**:
   - 出力ファイルのパスが表示されます
   - 通常は `Assets/OpenFitterOutput/` に保存されます

2. **結果をシーンに配置**:
   - 「シーンに追加」ボタンをクリック
   - または、Projectウィンドウから手動で配置

3. **結果の検証**:
   - Scene ビューで結果を確認
   - 衣装が正しくフィッティングされているか確認
   - ブレンドシェイプが保持されているか確認

4. **完了**:
   - 「閉じる」ボタンをクリックしてウィザードを終了
   - または「新しいフィッティング」で新規フィッティングを開始

## 出力結果の確認

### シーンビューでの確認

1. 出力されたFBXファイルをシーンに配置
2. アバターの姿勢を確認
3. 衣装のフィット具合を確認

### ブレンドシェイプの確認

1. Hierarchyで出力アバターを選択
2. Inspectorで SkinnedMeshRenderer を確認
3. ブレンドシェイプのスライダーを動かして動作確認

### マテリアルの確認

1. マテリアルが正しく割り当てられているか確認
2. 必要に応じてマテリアルを再設定

## よくある使用例

### 例1: VRChatアバター用の衣装変換

1. **ソース**: 衣装付きのアバターA
2. **ターゲット**: 自分のアバターB
3. **ブレンドシェイプ**: VRChat標準の表情を選択
4. **実行**: フィッティング実行
5. **結果**: アバターBに衣装がフィッティングされた状態で出力

### 例2: 複数の衣装をまとめてフィッティング

同じターゲットアバターに対して複数の衣装をフィッティングする場合：

1. 最初の衣装でフィッティング実行
2. 完了後、「新しいフィッティング」をクリック
3. 別のソースアバターを選択（同じターゲット）
4. 繰り返し

### 例3: 体型調整後の衣装再フィッティング

アバターの体型を変更した後、既存の衣装を再フィッティング：

1. **ソース**: 元の衣装
2. **ターゲット**: 体型変更後のアバター
3. **ブレンドシェイプ**: 必要なものを選択
4. **実行**: 再フィッティング

## トラブルシューティング

フィッティング中に問題が発生した場合は、[トラブルシューティング](./troubleshooting.md)ページを参照してください。

### よくある問題

- 衣装が正しくフィットしない → ボーン命名規則を確認
- ブレンドシェイプが失われる → ブレンドシェイプステップで正しく選択されているか確認
- 処理が途中で止まる → Unityコンソールでエラーを確認

## 次のステップ

OpenFitterUnityUIの基本的な使い方を理解できたら：

- 高度な使い方（準備中）で、より詳細なカスタマイズ方法を学ぶ
- [トラブルシューティング](./troubleshooting.md)で問題解決方法を確認
- [開発ガイド](./development/)で、OpenFitterUnityUIの拡張方法を学ぶ

---

## English

# Basic Usage

Detailed instructions for fitting clothing using OpenFitter.

## Quick Start

The most basic workflow for first-time OpenFitter users.

### Prerequisites

- OpenFitter is installed
- Environment setup is complete
- Source avatar (avatar with clothing) FBX file
- Target avatar (avatar to receive clothing) FBX file

### Step 1: Open OpenFitter Window

1. Select `Tools > OpenFitter` from Unity menu
2. OpenFitter wizard window opens



## Wizard Flow

OpenFitter consists of a wizard with the following steps:

1. [Environment Setup](#step-2-environment-setup)
2. [Source Selection](#step-3-source-selection)
3. [Target Selection](#step-4-target-selection)
4. [BlendShape Configuration](#step-5-blendshape-configuration)
5. [Advanced Options](#step-6-advanced-options)
6. [Execution](#step-7-execution)
7. [Completion](#step-8-completion)

### Step 2: Environment Setup

This step appears on first use or if environment is not yet set up.



#### Actions

1. Check status of each setup item:
   - **Green checkmark**: Installed
   - **Yellow warning**: Not installed or update needed
   - **Red error**: Installation failed

2. Click "Install All" button (if needed)
3. Click "Next" when all are green

### Step 3: Source Selection

Select the avatar with clothing to fit (source avatar).



#### Actions

1. **Select Source Avatar**:
   - Click object field
   - Drag and drop source avatar FBX file from Project window
   - Or click picker button to select

2. **Validation**:
   - Selected avatar is automatically validated
   - Error message displayed if there are issues

3. Click **Next**

#### Source Avatar Requirements

- Valid FBX format
- Contains armature (bone structure)
- Contains clothing mesh to fit
- Properly weight painted

### Step 4: Target Selection

Select the avatar to receive clothing (target avatar).



#### Actions

1. **Select Target Avatar**:
   - Select FBX file same way as source selection

2. **Validation**:
   - Target avatar is validated
   - Bone structure compatibility is checked

3. Click **Next**

#### Target Avatar Requirements

- Valid FBX format
- Contains armature (bone structure)
- Bone naming convention compatible with source avatar

### Step 5: BlendShape Configuration

Configure blendshapes to preserve after fitting (optional).




### Step 6: Advanced Options

Adjust fitting parameters (defaults are usually fine).



#### Available Options

- **RBF Smoothing Factor**: Adjust deformation smoothness (default: 1.0)
- **Weight Transfer Method**: Weight transfer algorithm
- **Mesh Optimization**: Output mesh optimization level

#### Actions

1. Adjust parameters as needed
2. If default values are fine, click "Next" as is

### Step 7: Execution

Execute the fitting process.



#### Actions

1. **Review Settings**:
   - Summary of settings displayed for final confirmation

2. **Click "Execute" Button**:
   - Fitting process starts

3. **Monitor Progress**:
   - Progress can be monitored via progress bar
   - Detailed processing can be viewed in log window




During processing:
- Do not close Unity
- Do not put PC in sleep mode
- Do not run other heavy processes

### Step 8: Completion

Fitting is complete.



#### Actions

1. **Review Results**:
   - Output file path is displayed
   - Usually saved to `Assets/OpenFitterOutput/`

2. **Place Results in Scene**:
   - Click "Add to Scene" button
   - Or manually place from Project window

3. **Validate Results**:
   - Review results in Scene view
   - Verify clothing is correctly fitted
   - Verify blendshapes are preserved

4. **Complete**:
   - Click "Close" button to exit wizard
   - Or "New Fitting" to start new fitting

## Reviewing Output Results

### Review in Scene View

1. Place output FBX file in scene
2. Check avatar pose
3. Check clothing fit

### Review BlendShapes

1. Select output avatar in Hierarchy
2. Check SkinnedMeshRenderer in Inspector
3. Move blendshape sliders to verify operation

### Review Materials

1. Verify materials are correctly assigned
2. Reassign materials as needed

## Common Use Cases

### Example 1: Convert Clothing for VRChat Avatar

1. **Source**: Avatar A with clothing
2. **Target**: Your avatar B
3. **BlendShapes**: Select VRChat standard expressions
4. **Execute**: Run fitting
5. **Result**: Clothing fitted to avatar B

### Example 2: Batch Fit Multiple Clothing Items

When fitting multiple clothing items to the same target avatar:

1. Execute fitting for first clothing item
2. After completion, click "New Fitting"
3. Select different source avatar (same target)
4. Repeat

### Example 3: Refit Clothing After Body Shape Adjustment

Refit existing clothing after changing avatar body shape:

1. **Source**: Original clothing
2. **Target**: Avatar after body shape change
3. **BlendShapes**: Select as needed
4. **Execute**: Refit

## Troubleshooting

If problems occur during fitting, see the [Troubleshooting](./troubleshooting.md) page.

### Common Issues

- Clothing doesn't fit correctly → Check bone naming conventions
- BlendShapes are lost → Verify correct selection in blendshape step
- Processing stops midway → Check Unity console for errors

## Next Steps

Once you understand basic OpenFitter usage:

- Learn more detailed customization in Advanced Usage (coming soon)
- Check [Troubleshooting](./troubleshooting.md) for problem-solving methods
- Learn how to extend OpenFitter in [Development Guide](./development/)
