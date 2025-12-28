using System.Text.RegularExpressions;
using UnityEditor;

namespace OpenFitter.Editor
{
    public class ProjectFilePostprocessor : AssetPostprocessor
    {
        private static string OnGeneratedCSProject(string path, string content)
        {
            // OpenFitterプロジェクト以外は何もしない
            if (!path.Contains("OpenFitter"))
            {
                return content;
            }

            // Nullable Reference Types を有効化
            content = AddNullableProperty(content);

            // Nullable警告をエラーとして扱う
            content = AddWarningsAsErrors(content);

            // ルート名前空間を設定
            content = AddRootNamespace(content);

            return content;
        }

        private static string AddNullableProperty(string content)
        {
            // すでに <Nullable> タグがある場合はスキップ
            if (content.Contains("<Nullable>"))
            {
                return content;
            }

            // <LangVersion> の直後に <Nullable>enable</Nullable> を挿入
            var pattern = @"(<LangVersion>.*?</LangVersion>)";
            var replacement = "$1\n    <Nullable>enable</Nullable>";

            return Regex.Replace(content, pattern, replacement);
        }

        private static string AddWarningsAsErrors(string content)
        {
            // すでに WarningsAsErrors がある場合はスキップ
            if (content.Contains("<WarningsAsErrors>"))
            {
                return content;
            }

            // Nullable関連の警告番号
            var nullableWarnings =
                "CS8600,CS8601,CS8602,CS8603,CS8604,CS8605,CS8606,CS8607,CS8608,CS8609," +
                "CS8610,CS8611,CS8612,CS8613,CS8614,CS8615,CS8616,CS8617,CS8618,CS8619," +
                "CS8620,CS8621,CS8622,CS8623,CS8624,CS8625,CS8626,CS8627,CS8628,CS8629," +
                "CS8631,CS8632,CS8633,CS8634,CS8635,CS8636,CS8637,CS8638,CS8639,CS8640," +
                "CS8641,CS8642,CS8643,CS8644,CS8645,CS8653,CS8654,CS8655,CS8714,CS8123";

            // <Nullable> の直後に <WarningsAsErrors> を挿入
            var pattern = @"(<Nullable>.*?</Nullable>)";
            var replacement = $"$1\n    <WarningsAsErrors>{nullableWarnings}</WarningsAsErrors>";

            return Regex.Replace(content, pattern, replacement);
        }

        private static string AddRootNamespace(string content)
        {
            // すでに RootNamespace がある場合はスキップ
            if (content.Contains("<RootNamespace>"))
            {
                return content;
            }

            // <WarningsAsErrors> の直後に <RootNamespace> を挿入
            var pattern = @"(<WarningsAsErrors>.*?</WarningsAsErrors>)";
            var replacement = "$1\n    <RootNamespace>OpenFitter</RootNamespace>";

            return Regex.Replace(content, pattern, replacement);
        }
    }
}
