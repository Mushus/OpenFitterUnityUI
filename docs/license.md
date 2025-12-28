# ライセンスとコンプライアンス

OpenFitterUnityUIのライセンスと外部依存関係について説明します。

## プロジェクトライセンス

このリポジトリに含まれるコードは **MIT License** でライセンスされています。

対象となるコード：
- `Assets/OpenFitter/`配下の全てのC#コード
- ドキュメントとビルドスクリプト
- Unity Editor拡張機能

詳細はプロジェクトルートの [LICENSE](https://github.com/Mushus/OpenFitterUnityUI/blob/main/LICENSE) ファイルを参照してください。

## 外部依存関係とダウンロードコンポーネント

OpenFitterは実行時に以下のコンポーネントを自動的にダウンロードします。これらのコンポーネントは本リポジトリには含まれず、それぞれ独自のライセンスの下で配布されています。

### Blender

- **ライセンス**: GPL-2.0 License
- **取得元**: [Blender公式サイト](https://www.blender.org/)
- **説明**: ポータブル版のBlenderが自動的にダウンロードされ、`BlenderTools/blender/`ディレクトリに配置されます
- **用途**: バックエンドの3D処理エンジンとして使用

### open-fitter-core

- **ライセンス**: GPL-3.0 License
- **取得元**: [Tallcat4's Repository](https://github.com/tallcat4/open-fitter-core)
- **説明**: Nine GatesのGPL-3ソースコードに基づくコアフィッティングアルゴリズム
- **配置場所**: `BlenderTools/open-fitter-core/`ディレクトリ（リポジトリには含まれません）
- **用途**: RBF変形とボーンポーズ転送を実装するコアアルゴリズム

詳細については [open-fitter-coreのREADME](https://github.com/tallcat4/open-fitter-core/blob/main/README.md) を参照してください。

### Blenderアドオン

- **ライセンス**: 各アドオンの個別ライセンスに従います
- **説明**: フィッティングプロセスに必要な各種Blenderアドオンが自動的にインストールされます
- **配置場所**: `BlenderTools/addons/`ディレクトリ

**重要**: `BlenderTools/`ディレクトリ全体はリポジトリには含まれず、初回セットアップ時に自動的にダウンロード・構成されます。

## 開発方針

OpenFitterプロジェクトは以下の原則に基づいて開発されています：

### リバースエンジニアリングなし

このプロジェクトは、プロプライエタリバイナリの **リバースエンジニアリングなし** で開発されています。

すべての実装は以下に基づいています：
- 公開されているドキュメント
- GPL-3.0ライセンスで公開されているソースコード
- 通常使用による動作観察
- 一般的な3D処理技術とアルゴリズム

### 参照としての使用

検証目的でのみ、オリジナルのプロプライエタリ製品（MochiFitter）を参照として利用しています。これは以下の目的に限定されています：

- 機能の動作確認
- 出力結果の品質比較
- ユーザーエクスペリエンスの参考

実装は独自に行われており、プロプライエタリコードの複製や派生ではありません。

## 免責事項

OpenFitterは非公式プロジェクトであり、以下の点にご注意ください：

- Nine Gatesからの承認や認可を受けていません
- MochiFitterからの公式なサポートや承認を受けていません
- オリジナル製品の商標権や著作権を侵害する意図はありません
- 提供される機能は独自の実装であり、互換性を保証するものではありません

## GPL-3.0コンポーネントの使用について

OpenFitterはGPL-3.0ライセンスのコンポーネント（open-fitter-core）を使用していますが、以下の点に注意してください：

1. **分離されたコンポーネント**: GPL-3.0コードは`BlenderTools/`ディレクトリに分離され、Unityパッケージには含まれません
2. **ランタイムダウンロード**: GPL-3.0コンポーネントはランタイムに別途ダウンロードされます
3. **MIT部分の独立性**: `Assets/OpenFitter/`配下のMITライセンスコードはGPL-3.0の影響を受けません
4. **プロセス分離**: Blenderは外部プロセスとして実行され、Unity Editorのプロセス空間とは分離されています

この構成により、Unity Editor拡張部分（MIT License）とコアアルゴリズム部分（GPL-3.0 License）が明確に分離され、それぞれのライセンスが適切に遵守されています。

## 貢献時のライセンス

このプロジェクトに貢献する場合、以下の点に同意したものとみなされます：

- 貢献したコードはMIT Licenseの下で公開されます
- リバースエンジニアリングされたコードや、ライセンスが不明確なコードを含めないこと
- GPL-3.0コンポーネントに貢献する場合は、GPL-3.0の条件に従うこと

## 商標とブランド

- "OpenFitter"は本プロジェクトの名称です
- "MochiFitter"は参照される製品名であり、Nine Gatesの商標である可能性があります
- "Blender"はBlender Foundationの登録商標です
- "Unity"はUnity Technologiesの登録商標です
- "VRChat"はVRChat Inc.の登録商標です

## お問い合わせ

ライセンスに関する質問や懸念がある場合は、GitHubのIssue Trackerを通じてお問い合わせください。

---

## English

# License and Compliance

This document explains the licensing and external dependencies of OpenFitter.

## Project License

The code contained in this repository is licensed under the **MIT License**.

This includes:
- All C# code under `Assets/OpenFitter/`
- Documentation and build scripts
- Unity Editor extension functionality

See the [LICENSE](https://github.com/Mushus/OpenFitterUnityUI/blob/main/LICENSE) file in the project root for details.

## External Dependencies and Downloaded Components

OpenFitter automatically downloads the following components at runtime. These components are not included in this repository and are distributed under their own respective licenses.

### Blender

- **License**: GPL-2.0 License
- **Source**: [Official Blender Website](https://www.blender.org/)
- **Description**: Portable version of Blender is automatically downloaded and placed in the `BlenderTools/blender/` directory
- **Purpose**: Used as the backend 3D processing engine

### open-fitter-core

- **License**: GPL-3.0 License
- **Source**: [Tallcat4's Repository](https://github.com/tallcat4/open-fitter-core)
- **Description**: Core fitting algorithm based on GPL-3 source code from Nine Gates
- **Location**: `BlenderTools/open-fitter-core/` directory (not included in this repository)
- **Purpose**: Core algorithm implementing RBF deformation and bone pose transfer

See the [open-fitter-core README](https://github.com/tallcat4/open-fitter-core/blob/main/README.md) for more details.

### Blender Add-ons

- **License**: Varies by individual add-on
- **Description**: Various Blender add-ons required for the fitting process are automatically installed
- **Location**: `BlenderTools/addons/` directory

**Important**: The entire `BlenderTools/` directory is not included in this repository and is automatically downloaded and configured during initial setup.

## Development Policy

The OpenFitter project is developed based on the following principles:

### No Reverse Engineering

This project is developed **without reverse engineering** of proprietary binaries.

All implementations are based on:
- Publicly available documentation
- Source code released under GPL-3.0 license
- Behavioral observation through normal usage
- General 3D processing techniques and algorithms

### Use as Reference

We utilize the original proprietary product (MochiFitter) as a reference for validation purposes only. This is limited to:

- Verifying feature functionality
- Comparing output quality
- Reference for user experience

Implementations are created independently and are not copies or derivatives of proprietary code.

## Disclaimer

OpenFitter is an unofficial project. Please note the following:

- Has not received approval or endorsement from Nine Gates
- Has not received official support or approval from MochiFitter
- No intent to infringe on trademarks or copyrights of the original product
- Provided functionality is our own implementation and compatibility is not guaranteed

## Regarding Use of GPL-3.0 Components

OpenFitter uses GPL-3.0 licensed components (open-fitter-core), but please note the following:

1. **Separated Components**: GPL-3.0 code is isolated in the `BlenderTools/` directory and is not included in the Unity package
2. **Runtime Download**: GPL-3.0 components are downloaded separately at runtime
3. **Independence of MIT Portion**: MIT licensed code under `Assets/OpenFitter/` is not affected by GPL-3.0
4. **Process Separation**: Blender runs as an external process, separated from Unity Editor's process space

This configuration ensures clear separation between the Unity Editor extension (MIT License) and the core algorithm (GPL-3.0 License), with each license properly respected.

## License for Contributions

When contributing to this project, you agree to the following:

- Contributed code will be released under the MIT License
- Do not include reverse-engineered code or code with unclear licensing
- When contributing to GPL-3.0 components, follow GPL-3.0 terms

## Trademarks and Brands

- "OpenFitter" is the name of this project
- "MochiFitter" is a referenced product name and may be a trademark of Nine Gates
- "Blender" is a registered trademark of the Blender Foundation
- "Unity" is a registered trademark of Unity Technologies
- "VRChat" is a registered trademark of VRChat Inc.

## Contact

If you have questions or concerns about licensing, please contact us through the GitHub Issue Tracker.
