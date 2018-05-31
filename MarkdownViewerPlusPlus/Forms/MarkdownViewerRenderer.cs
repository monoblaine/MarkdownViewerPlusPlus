using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Threading;
using static com.insanitydesign.MarkdownViewerPlusPlus.MarkdownViewer;

/// <summary>
/// 
/// </summary>
namespace com.insanitydesign.MarkdownViewerPlusPlus.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public class MarkdownViewerRenderer : AbstractRenderer
    {
        /// <summary>
        /// 
        /// </summary>
        public MarkdownViewerHtmlPanel markdownViewerHtmlPanel;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="markdownViewer"></param>
        public MarkdownViewerRenderer(MarkdownViewer markdownViewer) : base(markdownViewer)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Init()
        {
            base.Init();
            //
            this.markdownViewerHtmlPanel = new MarkdownViewerHtmlPanel();
            //Add to view
            this.Controls.Add(this.markdownViewerHtmlPanel);
            this.Controls.SetChildIndex(this.markdownViewerHtmlPanel, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fileInfo"></param>
        public override void Render(string text, FileInformation fileInfo)
        {
            base.Render(text, fileInfo);
            this.markdownViewerHtmlPanel.Text = BuildHtml(ConvertedText, fileInfo.FileName);
        }

        /// <summary>
        /// Scroll the rendered panel vertically based on the given ration
        /// taken from Notepad++
        /// </summary>
        /// <param name="scrollRatio"></param>
        public override void ScrollByRatioVertically(double scrollRatio)
        {
            LastScrollRatio = scrollRatio;
            this.markdownViewerHtmlPanel.Text = BuildHtml(ConvertedText, FileInfo.FileName);
            this.markdownViewerHtmlPanel.ScrollByRatioVertically(scrollRatio);
        }
    }
}
