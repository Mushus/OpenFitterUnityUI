#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace OpenFitter.Editor
{
    public static class I18n
    {
        private static Dictionary<string, string>? translations;
        private const string PoFilePath = "Assets/OpenFitter/Editor/Localization/ja.po";

        public static string Tr(string source)
        {
            if (translations == null)
            {
                LoadTranslations();
            }

            if (translations != null && translations.TryGetValue(source, out var translated))
            {
                return translated;
            }

            return source;
        }

        public static void Reload()
        {
            translations = null;
        }

        private static void LoadTranslations()
        {
            translations = new Dictionary<string, string>();

            // 日本語環境でない場合はロードしない（必要に応じて条件変更）
            // ここでは簡易的に常にロードを試みる、またはEditorの言語設定を見るなどが考えられるが
            // User request is specifically about missing Japanese translation, so we load if file exists.

            if (!File.Exists(PoFilePath))
            {
                // AssetDatabase経由で探す（パスが異なる可能性があるため）
                var guid = AssetDatabase.FindAssets("ja t:TextAsset");
                // 正確なパス解決は今回は簡易実装で固定パスを優先し、見つからなければ何もしない
                return;
            }

            try
            {
                var lines = File.ReadAllLines(PoFilePath);
                string? currentMsgid = null;

                foreach (var line in lines)
                {
                    var trimmed = line.Trim();
                    if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#")) continue;

                    if (trimmed.StartsWith("msgid "))
                    {
                        currentMsgid = ExtractValue(trimmed);
                    }
                    else if (trimmed.StartsWith("msgstr ") && currentMsgid != null)
                    {
                        var msgstr = ExtractValue(trimmed);
                        if (!string.IsNullOrEmpty(currentMsgid) && !string.IsNullOrEmpty(msgstr))
                        {
                            translations[currentMsgid] = msgstr;
                        }
                        currentMsgid = null;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[OpenFitter] Failed to load translations: {e.Message}");
            }
        }

        private static string ExtractValue(string line)
        {
            var firstQuote = line.IndexOf('"');
            var lastQuote = line.LastIndexOf('"');

            if (firstQuote >= 0 && lastQuote > firstQuote)
            {
                return line.Substring(firstQuote + 1, lastQuote - firstQuote - 1);
            }

            return string.Empty;
        }
    }
}
