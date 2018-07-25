using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Markdig;

namespace Cli {
    class Program {
        private static readonly MarkdownPipeline markdownPipeline = new MarkdownPipelineBuilder()
            .UseBootstrap()
            .UseAbbreviations()
            .UseAutoIdentifiers()
            .UseCitations()
            .UseCustomContainers()
            .UseDefinitionLists()
            .UseEmphasisExtras()
            .UseFigures()
            .UseFooters()
            .UseFootnotes()
            .UseGridTables()
            .UseMathematics()
            .UseMediaLinks()
            .UsePipeTables()
            .UseListExtras()
            .Use<com.insanitydesign.MarkdownViewerPlusPlus.MarkdigExtensions.VisualTaskLists.VisualTaskListExtension>()
            .UseDiagrams()
            .UseAutoLinks()
            .UseGenericAttributes()
            .UseSoftlineBreakAsHardlineBreak()
            .Build();

        public static void Main (String[] args) {
            var markdown = File.ReadAllText(args[0], Encoding.UTF8);
            var htmlBody = Markdown.ToHtml(markdown, markdownPipeline);

            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine(
                $@"
<!DOCTYPE html>
<html>
    <head>
        <meta charset=""utf-8"">
        <title></title>
        <style type=""text/css"">{_getCSS()}</style>
        <script src=""https://unpkg.com/mermaid@8.0.0-rc.8/dist/mermaid.min.js""></script>
        <script>mermaid.initialize({{startOnLoad:true}});</script>
      </head>
    <body class=""numbered-section"">{htmlBody}</body>
</html>
"
            );
        }

        private static String _getCSS () => File.ReadAllText(
            AppDomain.CurrentDomain.BaseDirectory + "../../../style/md.css",
            Encoding.UTF8
        );
    }
}
