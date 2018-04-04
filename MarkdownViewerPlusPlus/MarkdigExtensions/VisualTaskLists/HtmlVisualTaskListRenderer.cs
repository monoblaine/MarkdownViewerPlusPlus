using Markdig.Extensions.TaskLists;
using Markdig.Renderers;

namespace com.insanitydesign.MarkdownViewerPlusPlus.MarkdigExtensions.VisualTaskLists {
    public class HtmlVisualTaskListRenderer : HtmlTaskListRenderer {
        protected override void Write (HtmlRenderer renderer, TaskList obj) {
            if (renderer.EnableHtmlForInline) {
                renderer.Write("<span").WriteAttributes(obj).Write(@" class=""task-checkbox");

                if (obj.Checked) {
                    renderer.Write(" task-checkbox--checked");
                }

                renderer.Write(@"""><span class=""task-checkbox-sign");

                if (!obj.Checked) {
                    renderer.Write(" task-checkbox-sign--invisible");
                }

                renderer.Write(@""">✔</span></span>");
            }
            else {
                base.Write(renderer, obj);
            }
        }
    }
}
