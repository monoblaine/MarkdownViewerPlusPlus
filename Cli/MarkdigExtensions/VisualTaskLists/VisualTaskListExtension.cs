using Markdig;
using Markdig.Extensions.TaskLists;
using Markdig.Parsers.Inlines;
using Markdig.Renderers;
using Markdig.Renderers.Normalize;

namespace com.insanitydesign.MarkdownViewerPlusPlus.MarkdigExtensions.VisualTaskLists {
    public class VisualTaskListExtension : IMarkdownExtension {
        public void Setup (MarkdownPipelineBuilder pipeline) {
            if (!pipeline.InlineParsers.Contains<TaskListInlineParser>()) {
                // Insert the parser after the code span parser
                pipeline.InlineParsers.InsertBefore<LinkInlineParser>(new TaskListInlineParser());
            }
        }

        public void Setup (MarkdownPipeline pipeline, IMarkdownRenderer renderer) {
            if (renderer is HtmlRenderer htmlRenderer) {
                htmlRenderer.ObjectRenderers.AddIfNotAlready<HtmlVisualTaskListRenderer>();
            }

            if (renderer is NormalizeRenderer normalizeRenderer) {
                normalizeRenderer.ObjectRenderers.AddIfNotAlready<NormalizeTaskListRenderer>();
            }
        }
    }
}
