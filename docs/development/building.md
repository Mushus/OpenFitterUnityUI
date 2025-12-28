# ビルド方法

OpenFitterUnityUIのビルド、パッケージング、配布方法について説明します。

## 前提条件

ビルドを実行する前に、以下がインストールされていることを確認してください：

- **Node.js** 18以降
- **npm** (Node.jsに含まれます)
- **Unity** 2022.3以降
- **Git** (バージョン管理用)

## 開発環境のセットアップ

### 1. リポジトリのクローン

```bash
git clone https://github.com/Mushus/OpenFitterUnityUI.git
cd OpenFitter
```

### 2. Node.js依存関係のインストール

```bash
npm install
```

これにより、以下がインストールされます：
- VitePress（ドキュメント生成）
- TypeScript（ビルドスクリプト）
- ts-node（TypeScript実行環境）
- その他の開発依存関係

### 3. Unityでプロジェクトを開く

1. Unity Hubを起動
2. 「開く」をクリック
3. クローンしたリポジトリのディレクトリを選択
4. Unity 2022.3以降で開く

## Unityパッケージのビルド

### npmスクリプトを使用

最も簡単な方法は、npmスクリプトを使用することです：

```bash
npm run create-package
```

このコマンドは：
1. `Assets/OpenFitter/`ディレクトリをスキャン
2. 必要なファイルを収集
3. `OpenFitter.unitypackage`をプロジェクトルートに生成

### ビルドスクリプトの詳細

`scripts/create-unitypackage.ts`は以下の処理を行います：

1. **ファイルの収集**:
   - `Assets/OpenFitter/`配下のすべてのファイル
   - `.meta`ファイルを含む

2. **除外されるファイル**:
   - `.DS_Store`（macOS）
   - `.git`関連ファイル
   - 一時ファイル

3. **パッケージの生成**:
   - Unity の`ExportPackage` APIを使用
   - GUIDを保持してインポート時の参照を維持

### 手動ビルド（Unity内）

Unity Editor内から手動でパッケージを作成することもできます：

1. Unity Editorでプロジェクトを開く
2. メニューから `Assets > Export Package...` を選択
3. `Assets/OpenFitter`フォルダを選択
4. `Include dependencies`のチェックを外す（すべて自己完結しているため）
5. `Export...`をクリック
6. 保存場所とファイル名を指定
7. `Save`をクリック

## ドキュメントのビルド

### 開発サーバーの起動

ドキュメントをローカルでプレビューする：

```bash
npm run docs:dev
```

ブラウザで `http://localhost:5173` を開いてプレビューできます。

### 本番ビルド

静的サイトをビルドする：

```bash
npm run docs:build
```

ビルド結果は`docs/.vitepress/dist/`に生成されます。

### ドキュメントのデプロイ

#### GitHub Pagesへのデプロイ

1. **ビルド**:
```bash
npm run docs:build
```

2. **dist/をGitHub Pagesにデプロイ**:
```bash
cd docs/.vitepress/dist
git init
git add -A
git commit -m "Deploy documentation"
git push -f git@github.com:yourusername/OpenFitter.git master:gh-pages
```

3. **GitHub設定**:
   - リポジトリの Settings > Pages
   - Source: `gh-pages` ブランチを選択

#### Netlifyへのデプロイ

1. Netlifyアカウントにログイン
2. 「New site from Git」をクリック
3. リポジトリを接続
4. ビルド設定:
   - **Build command**: `npm run docs:build`
   - **Publish directory**: `docs/.vitepress/dist`

## テストの実行

### ローカリゼーション検証

すべてのローカリゼーション文字列を検証：

```bash
npm run check-l10n
```

このスクリプトは以下をチェックします：
- すべての言語で同じキーが定義されているか
- 未使用のキーがないか
- 欠落しているキーがないか

### Unity Test Runner

Unity Editor内でテストを実行：

1. Unity Editorを開く
2. `Window > General > Test Runner`を選択
3. `EditMode`タブを選択
4. `Run All`をクリック

## リリースプロセス

### 1. バージョンの更新

関連ファイルのバージョン番号を更新：

- `package.json`の`version`フィールド
- `README.md`のバージョン参照（必要に応じて）
- リリースノートの作成

### 2. テストの実行

すべてのテストが通ることを確認：

```bash
# ローカリゼーション検証
npm run check-l10n

# Unity Test Runnerでテスト実行（Unity Editor内で）
```

### 3. Unityパッケージのビルド

```bash
npm run create-package
```

### 4. ドキュメントのビルド（オプション）

```bash
npm run docs:build
```

### 5. GitHubリリースの作成

1. **Gitタグの作成**:
```bash
git tag -a v1.0.0 -m "Release version 1.0.0"
git push origin v1.0.0
```

2. **GitHubでリリースを作成**:
   - GitHubのリポジトリページに移動
   - `Releases` > `Draft a new release`をクリック
   - タグを選択（v1.0.0）
   - リリースタイトルとノートを記入
   - `OpenFitter.unitypackage`をアセットとしてアップロード
   - `Publish release`をクリック

### 6. ドキュメントのデプロイ（オプション）

```bash
# GitHub Pagesへデプロイ
cd docs/.vitepress/dist
git init
git add -A
git commit -m "Deploy v1.0.0 documentation"
git push -f git@github.com:yourusername/OpenFitter.git master:gh-pages
```

## 継続的インテグレーション（CI）

### GitHub Actionsの設定例

`.github/workflows/build.yml`を作成：

```yaml
name: Build and Test

on:
  push:
    branches: [main]
  pull_request:
    branches: [main]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: '18'

      - name: Install dependencies
        run: npm install

      - name: Run localization validation
        run: npm run check-l10n

      - name: Build documentation
        run: npm run docs:build

  release:
    if: startsWith(github.ref, 'refs/tags/v')
    needs: test
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: '18'

      - name: Install dependencies
        run: npm install

      - name: Build Unity package
        run: npm run create-package

      - name: Create Release
        uses: softprops/action-gh-release@v1
        with:
          files: OpenFitter.unitypackage
        env:
          GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
```

## トラブルシューティング

### ビルドエラー

**問題**: `npm run create-package`が失敗する

**解決方法**:
- Node.jsのバージョンを確認（18以降が必要）
- `node_modules/`を削除して`npm install`を再実行
- TypeScriptのエラーを確認して修正

### Unity パッケージのインポートエラー

**問題**: 生成したパッケージがUnityでインポートできない

**解決方法**:
- GUIDが正しく保持されているか確認
- `.meta`ファイルがすべて含まれているか確認
- Unityのバージョンが2022.3以降であることを確認

### ドキュメントビルドエラー

**問題**: `npm run docs:build`が失敗する

**解決方法**:
- Markdownファイルの構文エラーを確認
- VitePressの設定ファイル（`.vitepress/config.ts`）を確認
- 不正なリンクがないか確認

## ベストプラクティス

### バージョン管理

- **セマンティックバージョニング** を使用（major.minor.patch）
  - **Major**: 破壊的な変更
  - **Minor**: 新機能の追加（後方互換性あり）
  - **Patch**: バグ修正

### リリースノート

各リリースには詳細なリリースノートを含める：

```markdown
## v1.0.0 (2024-01-01)

### 新機能
- 自動環境セットアップ機能を追加
- ブレンドシェイプ管理機能を追加

### 改善
- フィッティング速度を30%向上
- UIのレスポンスを改善

### バグ修正
- 大きなメッシュでのクラッシュを修正
- ブレンドシェイプが保持されない問題を修正

### 既知の問題
- 非常に複雑なアーマチュアでパフォーマンスが低下する可能性
```

### ビルド前チェックリスト

- [ ] すべてのテストが通る
- [ ] ローカリゼーション検証が通る
- [ ] バージョン番号が更新されている
- [ ] リリースノートが作成されている
- [ ] ドキュメントが最新である
- [ ] 既知の問題が文書化されている

## まとめ

OpenFitterUnityUIのビルドプロセスは以下のステップで構成されています：

1. **開発環境のセットアップ**: Node.jsとUnityのインストール
2. **Unityパッケージのビルド**: npmスクリプトまたは手動ビルド
3. **ドキュメントのビルド**: VitePressで静的サイトを生成
4. **テストの実行**: ローカリゼーション検証とユニットテスト
5. **リリース**: GitHubリリースの作成とパッケージのアップロード

自動化されたビルドスクリプトにより、一貫性のある品質の高いリリースを作成できます。

---

## English

# Building

This document explains how to build, package, and distribute OpenFitter.

## Prerequisites

Before building, ensure the following are installed:

- **Node.js** 18 or later
- **npm** (included with Node.js)
- **Unity** 2022.3 or later
- **Git** (for version control)

## Development Environment Setup

### 1. Clone the Repository

```bash
git clone https://github.com/Mushus/OpenFitterUnityUI.git
cd OpenFitter
```

### 2. Install Node.js Dependencies

```bash
npm install
```

This installs:
- VitePress (documentation generation)
- TypeScript (build scripts)
- ts-node (TypeScript runtime)
- Other development dependencies

### 3. Open Project in Unity

1. Launch Unity Hub
2. Click "Open"
3. Select the cloned repository directory
4. Open with Unity 2022.3 or later

## Building Unity Package

### Using npm Script

The easiest method is using the npm script:

```bash
npm run create-package
```

This command:
1. Scans the `Assets/OpenFitter/` directory
2. Collects necessary files
3. Generates `OpenFitter.unitypackage` in the project root

### Build Script Details

`scripts/create-unitypackage.ts` performs the following:

1. **File Collection**:
   - All files under `Assets/OpenFitter/`
   - Including `.meta` files

2. **Excluded Files**:
   - `.DS_Store` (macOS)
   - `.git` related files
   - Temporary files

3. **Package Generation**:
   - Uses Unity's `ExportPackage` API
   - Preserves GUIDs to maintain references on import

### Manual Build (In Unity)

You can also manually create a package from within Unity Editor:

1. Open project in Unity Editor
2. Select `Assets > Export Package...` from menu
3. Select the `Assets/OpenFitter` folder
4. Uncheck `Include dependencies` (everything is self-contained)
5. Click `Export...`
6. Specify save location and filename
7. Click `Save`

## Building Documentation

### Start Development Server

Preview documentation locally:

```bash
npm run docs:dev
```

Open `http://localhost:5173` in browser to preview.

### Production Build

Build static site:

```bash
npm run docs:build
```

Build output is generated in `docs/.vitepress/dist/`.

### Deploying Documentation

#### Deploy to GitHub Pages

1. **Build**:
```bash
npm run docs:build
```

2. **Deploy dist/ to GitHub Pages**:
```bash
cd docs/.vitepress/dist
git init
git add -A
git commit -m "Deploy documentation"
git push -f git@github.com:yourusername/OpenFitter.git master:gh-pages
```

3. **GitHub Settings**:
   - Repository Settings > Pages
   - Source: Select `gh-pages` branch

#### Deploy to Netlify

1. Log in to Netlify account
2. Click "New site from Git"
3. Connect repository
4. Build settings:
   - **Build command**: `npm run docs:build`
   - **Publish directory**: `docs/.vitepress/dist`

## Running Tests

### Localization Validation

Validate all localization strings:

```bash
npm run check-l10n
```

This script checks:
- Same keys are defined in all languages
- No unused keys
- No missing keys

### Unity Test Runner

Run tests in Unity Editor:

1. Open Unity Editor
2. Select `Window > General > Test Runner`
3. Select `EditMode` tab
4. Click `Run All`

## Release Process

### 1. Update Version

Update version numbers in related files:

- `version` field in `package.json`
- Version references in `README.md` (if needed)
- Create release notes

### 2. Run Tests

Ensure all tests pass:

```bash
# Localization validation
npm run check-l10n

# Run tests in Unity Test Runner (in Unity Editor)
```

### 3. Build Unity Package

```bash
npm run create-package
```

### 4. Build Documentation (Optional)

```bash
npm run docs:build
```

### 5. Create GitHub Release

1. **Create Git Tag**:
```bash
git tag -a v1.0.0 -m "Release version 1.0.0"
git push origin v1.0.0
```

2. **Create Release on GitHub**:
   - Go to repository page on GitHub
   - Click `Releases` > `Draft a new release`
   - Select tag (v1.0.0)
   - Fill in release title and notes
   - Upload `OpenFitter.unitypackage` as asset
   - Click `Publish release`

### 6. Deploy Documentation (Optional)

```bash
# Deploy to GitHub Pages
cd docs/.vitepress/dist
git init
git add -A
git commit -m "Deploy v1.0.0 documentation"
git push -f git@github.com:yourusername/OpenFitter.git master:gh-pages
```

## Continuous Integration (CI)

### GitHub Actions Example

Create `.github/workflows/build.yml`:

```yaml
name: Build and Test

on:
  push:
    branches: [main]
  pull_request:
    branches: [main]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: '18'

      - name: Install dependencies
        run: npm install

      - name: Run localization validation
        run: npm run check-l10n

      - name: Build documentation
        run: npm run docs:build

  release:
    if: startsWith(github.ref, 'refs/tags/v')
    needs: test
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: '18'

      - name: Install dependencies
        run: npm install

      - name: Build Unity package
        run: npm run create-package

      - name: Create Release
        uses: softprops/action-gh-release@v1
        with:
          files: OpenFitter.unitypackage
        env:
          GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
```

## Troubleshooting

### Build Errors

**Problem**: `npm run create-package` fails

**Solution**:
- Check Node.js version (18 or later required)
- Delete `node_modules/` and run `npm install` again
- Check and fix TypeScript errors

### Unity Package Import Errors

**Problem**: Generated package cannot be imported in Unity

**Solution**:
- Verify GUIDs are correctly preserved
- Check that all `.meta` files are included
- Ensure Unity version is 2022.3 or later

### Documentation Build Errors

**Problem**: `npm run docs:build` fails

**Solution**:
- Check Markdown file syntax errors
- Check VitePress configuration file (`.vitepress/config.ts`)
- Check for invalid links

## Best Practices

### Versioning

- Use **Semantic Versioning** (major.minor.patch)
  - **Major**: Breaking changes
  - **Minor**: New features (backward compatible)
  - **Patch**: Bug fixes

### Release Notes

Include detailed release notes for each release:

```markdown
## v1.0.0 (2024-01-01)

### New Features
- Added automatic environment setup
- Added blendshape management

### Improvements
- Improved fitting speed by 30%
- Improved UI responsiveness

### Bug Fixes
- Fixed crash with large meshes
- Fixed blendshape preservation issue

### Known Issues
- Performance may degrade with very complex armatures
```

### Pre-Build Checklist

- [ ] All tests pass
- [ ] Localization validation passes
- [ ] Version number updated
- [ ] Release notes created
- [ ] Documentation is up-to-date
- [ ] Known issues are documented

## Summary

OpenFitter's build process consists of the following steps:

1. **Development Environment Setup**: Install Node.js and Unity
2. **Build Unity Package**: npm script or manual build
3. **Build Documentation**: Generate static site with VitePress
4. **Run Tests**: Localization validation and unit tests
5. **Release**: Create GitHub release and upload package

Automated build scripts enable consistent, high-quality releases.
