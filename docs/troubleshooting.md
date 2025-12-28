# トラブルシューティング

OpenFitterUnityUIの使用中に発生する可能性のある一般的な問題と解決方法について説明します。

## 環境セットアップに関する問題

### 環境セットアップが失敗する

**症状**: セットアップウィザードで「すべてインストール」をクリックしても、エラーが発生してセットアップが完了しない。

**解決方法**:
- アクティブなインターネット接続があることを確認してください
- 十分なディスク容量（約2GB以上）があることを確認してください
- 管理者としてUnityを実行してみてください
- ファイアウォールやアンチウイルスソフトがダウンロードをブロックしていないか確認してください

### Blenderのダウンロードが失敗する

**症状**: Blenderのダウンロード中にエラーが発生する。

**解決方法**:
- インターネット接続を確認してください
- `BlenderTools/`フォルダを削除してから再度セットアップを実行してください
- 手動でBlenderをダウンロードして`BlenderTools/blender/`に配置することもできます

## フィッティングプロセスに関する問題

### フィッティングプロセスがハングする

**症状**: 実行ステップで進捗バーが進まず、処理が停止しているように見える。

**解決方法**:
- Unityコンソールでエラーメッセージを確認してください
- ソースとターゲットのモデルが有効なFBXファイルであることを確認してください
- Blenderのインストールが完了していることを確認してください（環境セットアップステップですべてのアイテムが緑色のチェックマークになっている）
- 非常に大きなモデルの場合、処理に時間がかかることがあります。しばらく待ってみてください

### 出力モデルに問題がある

**症状**: フィッティング後の衣装が正しく配置されていない、または変形している。

**解決方法**:
- ブレンドシェイプ設定を確認してください。不要なブレンドシェイプが適用されていないか確認してください
- ソースの衣装が適切にウェイト付けされていることを確認してください
- ボーン命名規則が一致していることを確認してください。VRChatアバターの標準的な命名規則に従っていることを確認してください
- アーマチュアのスケールが正しいことを確認してください

### ブレンドシェイプが保持されない

**症状**: フィッティング後、設定したブレンドシェイプが失われている。

**解決方法**:
- ブレンドシェイプステップで正しくブレンドシェイプが選択されていることを確認してください
- ブレンドシェイプ名が正確に一致していることを確認してください（大文字小文字も区別されます）
- 一部のブレンドシェイプはフィッティングプロセス中に互換性がない場合があります

## モデルに関する問題

### FBXファイルが認識されない

**症状**: ソースまたはターゲット選択ステップでFBXファイルを選択できない。

**解決方法**:
- FBXファイルがUnityプロジェクトのAssetsフォルダ内にあることを確認してください
- FBXファイルがUnityにインポートされていることを確認してください
- FBXファイルが破損していないか確認してください

### アーマチュアが見つからない

**症状**: 「アーマチュアが見つかりません」というエラーが表示される。

**解決方法**:
- FBXファイルにアーマチュア（ボーン構造）が含まれていることを確認してください
- アーマチュアの名前が標準的な命名規則に従っていることを確認してください
- FBXエクスポート時にアーマチュアが含まれていることを確認してください

## パフォーマンスに関する問題

### 処理が遅い

**症状**: フィッティング処理に非常に長い時間がかかる。

**解決方法**:
- モデルのポリゴン数が非常に多い場合、処理に時間がかかります
- 可能であれば、モデルを最適化してポリゴン数を減らしてください
- 高度なオプションでパラメータを調整することで、処理速度と品質のバランスを取ることができます

## その他の問題

### エラーメッセージが表示される

**一般的な対処法**:
1. Unityコンソールでエラーの詳細を確認してください
2. エラーメッセージをコピーして、[GitHub Discussions](https://github.com/Mushus/OpenFitterUnityUI/discussions)で検索してください
3. 問題が解決しない場合は、新しいDiscussionを作成して質問してください

### バグ報告

明らかなバグを発見した場合は、[GitHub Discussions](https://github.com/Mushus/OpenFitterUnityUI/discussions)に報告してください。

報告する際は、以下の情報を含めてください：

- **OpenFitterUnityUIのバージョン**: 使用しているバージョン番号
- **Unityのバージョン**: Unity Editorのバージョン（例: 2022.3.10f1）
- **オペレーティングシステム**: OS名とバージョン（例: Windows 11、Windows 10）
- **エラーメッセージの全文**: Unityコンソールからコピー
- **再現手順**: 問題を再現するための詳細な手順
  1. 何をしたか
  2. どのような結果が期待されたか
  3. 実際にどうなったか
- **使用モデルの情報**: 可能な範囲で（ポリゴン数、ボーン数など）
- **スクリーンショット**: 必要に応じて

### 質問や議論

- **使い方の質問**: [GitHub Discussions](https://github.com/Mushus/OpenFitterUnityUI/discussions)で質問してください
- **機能リクエスト**: [GitHub Discussions](https://github.com/Mushus/OpenFitterUnityUI/discussions)で提案してください
- **一般的な議論**: コミュニティとの交流は[GitHub Discussions](https://github.com/Mushus/OpenFitterUnityUI/discussions)で

---

## English

# Troubleshooting

This guide explains common issues that may occur when using OpenFitter and their solutions.

## Environment Setup Issues

### Environment setup fails

**Symptoms**: Clicking "Install All" in the setup wizard results in an error and setup doesn't complete.

**Solutions**:
- Ensure you have an active internet connection
- Check that you have sufficient disk space (at least 2GB)
- Try running Unity as Administrator
- Check if your firewall or antivirus software is blocking the downloads

### Blender download fails

**Symptoms**: Error occurs during Blender download.

**Solutions**:
- Check your internet connection
- Delete the `BlenderTools/` folder and run setup again
- You can manually download Blender and place it in `BlenderTools/blender/`

## Fitting Process Issues

### Fitting process hangs

**Symptoms**: Progress bar doesn't advance during the execution step and the process appears to be stuck.

**Solutions**:
- Check the Unity console for error messages
- Verify that source and target models are valid FBX files
- Ensure Blender installation is complete (all items in environment setup step should have green checkmarks)
- For very large models, processing may take time. Please wait a while

### Output model has issues

**Symptoms**: The fitted clothing is not positioned correctly or is deformed.

**Solutions**:
- Review blendshape configuration. Check if unwanted blendshapes are being applied
- Check that source clothing is properly weighted
- Verify bone naming conventions match. Ensure they follow VRChat avatar standard naming conventions
- Verify that armature scaling is correct

### Blendshapes are not preserved

**Symptoms**: After fitting, configured blendshapes are lost.

**Solutions**:
- Ensure blendshapes are correctly selected in the blendshape step
- Verify that blendshape names match exactly (case-sensitive)
- Some blendshapes may not be compatible during the fitting process

## Model Issues

### FBX file is not recognized

**Symptoms**: Cannot select FBX file in source or target selection step.

**Solutions**:
- Ensure the FBX file is within your Unity project's Assets folder
- Verify the FBX file is imported into Unity
- Check if the FBX file is corrupted

### Armature not found

**Symptoms**: Error message "Armature not found" is displayed.

**Solutions**:
- Ensure the FBX file contains an armature (bone structure)
- Verify the armature name follows standard naming conventions
- Check that the armature was included during FBX export

## Performance Issues

### Processing is slow

**Symptoms**: Fitting process takes a very long time.

**Solutions**:
- If the model has a very high polygon count, processing will take longer
- If possible, optimize the model to reduce polygon count
- Adjust parameters in advanced options to balance processing speed and quality

## Other Issues

### Error messages appear

**General troubleshooting**:
1. Check error details in the Unity console
2. Copy the error message and search [GitHub Discussions](https://github.com/Mushus/OpenFitterUnityUI/discussions)
3. If the problem persists, create a new Discussion to ask for help

### Bug Reports

If you discover a clear bug, please report it to [GitHub Discussions](https://github.com/Mushus/OpenFitterUnityUI/discussions).

When reporting, please include the following information:

- **OpenFitterUnityUI Version**: Version number you are using
- **Unity Version**: Unity Editor version (e.g., 2022.3.10f1)
- **Operating System**: OS name and version (e.g., Windows 11, Windows 10)
- **Full Error Message**: Copy from Unity console
- **Reproduction Steps**: Detailed steps to reproduce the issue
  1. What you did
  2. What you expected to happen
  3. What actually happened
- **Model Information**: If possible (polygon count, bone count, etc.)
- **Screenshots**: If applicable

### Questions and Discussions

- **Usage Questions**: Ask at [GitHub Discussions](https://github.com/Mushus/OpenFitterUnityUI/discussions)
- **Feature Requests**: Suggest at [GitHub Discussions](https://github.com/Mushus/OpenFitterUnityUI/discussions)
- **General Discussion**: Join the community at [GitHub Discussions](https://github.com/Mushus/OpenFitterUnityUI/discussions)
