# OutfitRetargetingSystem（もちふぃったー）概要

OutfitRetargetingSystem は、テンプレートとなる衣装メッシュ（`Template.fbx`）を任意のベースアバターへ「着せ替え」するための Unity エディタ拡張／データセットです。本プロジェクトでは **もちふぃったー** という名称で呼ばれており、`Assets/OutfitRetargetingSystem/Editor/OutfitRetargetingSystem.dll` に格納されたエディタ GUI／処理パイプライン（メッシュ変形アルゴリズムもこの DLL 内の C# スクリプトにコンパイル済み）と、各種 JSON/NPZ リソース群を組み合わせて動作します。アルゴリズムのソースコードはリポジトリに含まれていないため、処理内容を追跡する際は DLL を逆アセンブルするか、以下に挙げたデータファイル群とログを頼りに挙動を推測します。

## Unity プロジェクトへの組み込み
- **設定アセット**：`Assets/OutfitRetargetingSettings.asset` に UI で選択したターゲット FBX、ベース FBX、使用する設定 JSON、ターゲット／ソースアバター名、出力用フォルダなどが保存されます。ここで選んだ内容が dll 内のウィザードに渡され、処理が実行されます。
- **Editor 拡張 DLL**：`Assets/OutfitRetargetingSystem/Editor/OutfitRetargetingSystem.dll` が実際のメッシュ変形やファイル I/O、Unity 側 UI（ウィンドウ・ボタン等）を司ります。ソースは同梱されていないため、本ドキュメントでは外部リソースの役割を中心に整理します。
- **テスト用アセット**：`Assets/OutfitRetargetingSystem/Editor/TestingDatasets` 配下には Beryl アバター向けの FBX／衣装 Prefab 群があり、動作確認や QA テストに利用できます。

## 主要データセット
OutfitRetargetingSystem は大量の補助データを参照しながら、衣装メッシュをターゲットアバターに合わせてリターゲットします。ここでは役割ごとにファイル群を整理します。

### 1. アバターデータ (`avatar_data_*.json`)
- 例：`avatar_data_shinano5.json`、`avatar_data_template.json`。
- 各ファイルにはアバター名、デフォルトで参照する FBX、処理対象メッシュ名、身長、基準ポーズファイル、既定のブレンドシェイプ値、Humanoid ボーン ↔ 実ボーン名のマッピング、追加ボーンやミラー／スケール情報などが詳述されています。
- これにより dll はターゲットと衣装テンプレートの骨格・メッシュ構造を正しく解釈し、後述のポーズ・変形データを正しい座標空間で適用できます。

### 2. コンフィグ (`config_*.json`)
- 例：`config_template2shinano.json`、`config_template2manuka.json`。
- 構成要素：
  - `poseDataPath`：姿勢差分データ（`posediff_*.json`）のファイル名。
  - `fieldDataPath`：基本の変形ベクトルフィールド（`deformation_*.npz`）のファイル名。
  - `sourceBlendShapeSettings` / `targetBlendShapeSettings`：処理前後に固定値で設定するブレンドシェイプ。
  - `blendShapeFields`：胸・ヒールなど特定領域を追加調整するときの複数エントリ。各エントリは `path`（追加フィールド）、`maskBones`（影響を与えるボーン群）、`source/target` 側でのシェイプ値を定義します。
  - `baseAvatarDataPath` / `clothingAvatarDataPath`：ターゲットアバターと衣装テンプレートに使うアバターデータ JSON。
  - `clothingBlendShapeSettings` / `clothingBlendShapeSettingsInv`：衣装メッシュ側に適用する補正。
- これらはプリセット化された「テンプレート → ターゲット」の変換テーブルであり、ユーザーは UI 上で適切なコンフィグを選ぶことで即座にリターゲットを走らせられます。

### 3. ポーズ基準・差分 (`pose_basis_*.json` / `posediff_*.json`)
- `pose_basis_*.json` は各アバター固有の基準姿勢（ボーンごとの location/rotation/scale と変換行列）を保持します。
- `posediff_source_to_target.json` はテンプレートとターゲットの骨配列を一致させるための差分データで、リターゲット前に骨姿勢を揃えるステップで使用されます。

### 4. 変形フィールド (`deformation_*.npz`)
- `fieldDataPath` や `blendShapeFields[].path` に指定される `.npz` は、NumPy 形式のベクトルフィールドです。テンプレートメッシュの各頂点がターゲットでどこに移動すべきか、または特定ブレンドシェイプを有効化した際の補正ベクトルを格納しています。
- `_to_template` / `_to_<avatar>` のように命名されており、順方向・逆方向や胸・ヒールなど部位別の補正が揃っています。
- 例として `Assets/OutfitRetargetingSystem/Editor/deformation_template_to_shinano.npz` を展開すると `all_field_points.npy` / `all_delta_positions.npy`（どちらも object 配列でステップごとの対応点群と変位ベクトルを包含）、`world_matrix.npy`（4x4 行列）、`kdtree_query_k.npy`、`rbf_epsilon.npy`、`rbf_smoothing.npy`、`num_steps.npy`、`enable_x_mirror.npy` などのメタデータが入っています。格子状の決め打ち変形ではなく、任意点群＋ RBF（Radial Basis Function）補間でメッシュ頂点を移動させる方式であることが読み取れます。
- `rbf_epsilon` は RBF の形状パラメータで、値が小さいほど影響半径が狭くなり局所的な変形、値が大きいほど広範囲に影響する傾向があります。`rbf_smoothing` は補間時の正則化項で、完全一致させる代わりに微小な平滑化（ノイズ抑制）を入れるかどうかを制御する係数です。

#### 変形フィールドの適用ロジック（推定）
1. DLL 側が `all_field_points` を読み込み、各ステップごとのサンプル点群を KD-tree 化（`kdtree_query_k` が近傍点数、`enable_x_mirror` で X 対称を許可するかを制御）。
2. テンプレート衣装の各頂点を `world_matrix` で同一座標空間へ変換し、KD-tree から近傍点を取得。
3. `rbf_epsilon` と `rbf_smoothing` を使って RBF ウェイトを計算し、対応する `all_delta_positions` の変位ベクトルを加重平均することで頂点オフセットを得ます。`num_steps` が複数ステップの場合は粗→細の順で繰り返し適用され、最終形状が安定するようになっています。
4. この処理は衣装全頂点および `blendShapeFields` ごとのマスク領域に対して行われるため、最終的には「周囲の点群から変形ベクトルを取得して移動させる」挙動になります（ただし実際の適用コードは `OutfitRetargetingSystem.dll` 内にコンパイル済み）。

### 5. ブレンドシェイプ・頂点マスク関連
- `template_vertex_group_weights.json` および `vertex_group_weights_*.json` は、特定頂点グループのウエイトマップを JSON 化したものです。エディタ側はここからマスクを生成し、胸・股関節・表面など部位別の補間やスムージングに使います。
- `HumanoidBoneNamePatterns.json` は人型ボーン名のヒューリスティックマップで、自動ボーン認識やマスクボーン判定の基礎データになっています。

### 6. 付帯ブレンドシェイプセット
- `pose_basis_template_a.json` のような追加姿勢、`avatar_data_*` 内 `blendshapes`、`clothingBlendShapeSettingsInv` などにより、衣装・ボディ両方のシェイプを同期させるシナリオが実現されています。

## 補助ツール：`smoothing_processor.py`
- エディタ処理中に頂点グループのスムージングが必要な場合、`Assets/OutfitRetargetingSystem/Editor/smoothing_processor.py` をサブプロセスとして起動します。
- NumPy + SciPy（`cKDTree`）で最近傍探索を行い、ガウシアン／線形ウェイト付き平均でウエイトマップを平滑化します。単一グループ／複数グループの2モードを備え、マルチプロセスで高速化しています。
- `.npz` 入力には頂点座標・元ウエイト・パラメータ群が含まれており、処理結果を再び `.npz` / JSON に戻してエディタへ返す想定です。
- Python 環境を用意できない場合は、`Assets/Mushus/MochiFitter/VertexWeightSmoothing.cs` で同等ロジックを Pure C# で呼び出せます。こちらは愚直な O(N²) 走査で最近傍を探す実装ですが、Unity 内で完結するためバッチ処理や自動化シーンに組み込みやすくなっています。

## 想定ワークフロー
1. **準備**：ターゲットアバター用 FBX を `Assets/<Avatar>/FBX/...` に配置し、対応する `avatar_data_*.json` を作成または既存のものを編集します。
2. **テンプレート衣装調整**：`Template.fbx` に衣装を追加し、必要に応じて `vertex_group_weights_*.json` やブレンドシェイプ初期値を更新します。
3. **Unity 上でのリターゲット**：`Tools > 着せ替え` から専用ウィンドウ（`Assets/Mushus/MochiFitter/Editor/MochiFitterWindow.cs`）を開き、衣装オブジェクト／転送元アバター／転送先アバター／Config JSON（`config_template2*.json` を TextAsset 化したもの）を指定して [Retarget Outfit] を実行します。ウィンドウ側で JSON を読み込み、`poseDataPath` や `fieldDataPath` などの概要を表示したうえで SkinnedMeshRenderer の骨をターゲット階層へ張り替えます。必要に応じて `VertexWeightSmoothing` を追加で呼び出せます。
4. **リターゲット実行**：  
   - **ポーズ差分**：`config_*` が指す `poseDataPath`（例：`posediff_template_to_shinano.json`）を読み込み、衣装テンプレートのボーン姿勢をターゲットの `avatar_data_*` に合わせて補正します。これにより両メッシュが同一ローカルポーズで比較できる状態になります。  
   - **基本フィールド適用**：`fieldDataPath`（例：`deformation_template_to_shinano.npz`）のベクトルフィールドを衣装メッシュ全頂点へ適用し、体格差による大まかな変形を一括で反映します。  
   - **ブレンドシェイプ別フィールド**：`blendShapeFields` に列挙された胸・ヒールなどの追加補正を順番に適用します。各項目は `maskBones` で指定されたボーン領域だけに影響するマスクを生成し、`source/targetBlendShapeSettings` を反映しながら対応する `.npz` をブレンドします。  
   - **頂点グループスムージング**：最終的な頂点ウエイトにムラがあれば `smoothing_processor.py` が呼び出され、`vertex_group_weights_*.json` を基準に局所的な cKDTree スムージングを行います。  
   - **出力**：すべての補正が終わると、`OutfitRetargetingSettings.asset` の `outputFolderName`（既定は `Outputs`）以下に FBX・Prefab などが生成され、`tempFolderName` は途中生成物の退避に使われます。
5. **検証**：`TestingDatasets` の Prefab やターゲットシーンで衣装を確認し、必要ならブレンドシェイプの固定値やマスクを調整します。

## 補足・運用ヒント
- `.npz` や JSON はキャラクターごとのプリセットとして完備されていますが、新アバターに対応するには「テンプレート ↔ 新アバター」のポーズ差分と変形フィールドを別途生成する必要があります。
- `HumanoidBoneNamePatterns` のカバレッジが高いため、ボーン命名規則を大きく逸脱しないようにしておくとマスクや自動割り当ての成功率が上がります。
- `OutfitRetargetingSettings.asset` の `outputFolderName` / `tempFolderName` を切り替えることでバッチ処理や比較検証用の出力先を簡単に分けられます。

本ドキュメントはプロジェクトルートの `docs/` に配置しているため、Unity を起動せずに OutfitRetargetingSystem の全体像を把握したい場合に参照してください。
