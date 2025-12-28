import { defineConfig } from 'vitepress'

// https://vitepress.dev/reference/site-config
export default defineConfig({
    title: "OpenFitterUnityUI",
    base: '/OpenFitterUnityUI/',
    description: "Unity向け衣装変換ツール OpenFitterUnityUIの解説ドキュメント",
    ignoreDeadLinks: true,
    themeConfig: {
        // https://vitepress.dev/reference/default-theme-config
        nav: [
            { text: 'Home', link: '/' },
            { text: 'ガイド', link: '/setup' },
            { text: '開発', link: '/development/' }
        ],

        sidebar: [
            {
                text: '利用者向けガイド',
                items: [
                    { text: 'セットアップ', link: '/setup' },
                    { text: '基本的な使い方', link: '/usage' },
                    { text: 'トラブルシューティング', link: '/troubleshooting' },
                    { text: 'ライセンス', link: '/license' }
                ]
            },
            {
                text: '開発者向け',
                items: [
                    { text: '開発ガイド', link: '/development/' },
                    { text: 'アーキテクチャ', link: '/development/architecture' },
                    { text: 'コア技術', link: '/development/technology' },
                    { text: 'プロジェクト構造', link: '/development/project-structure' },
                    { text: 'ビルド方法', link: '/development/building' }
                ]
            }
        ],

        socialLinks: [
            { icon: 'github', link: 'https://github.com/Mushus/OpenFitterUnityUI' }
        ]
    }
})
