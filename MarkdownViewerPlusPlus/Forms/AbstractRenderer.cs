using com.insanitydesign.MarkdownViewerPlusPlus.Properties;
using com.insanitydesign.MarkdownViewerPlusPlus.Windows;
using Kbg.NppPluginNET;
using Kbg.NppPluginNET.PluginInfrastructure;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static com.insanitydesign.MarkdownViewerPlusPlus.Windows.WindowsMessage;
using System.Drawing.Printing;
using System.Xml.Linq;
using System.IO;
using System.Net;
using com.insanitydesign.MarkdownViewerPlusPlus.Helper;
using static com.insanitydesign.MarkdownViewerPlusPlus.MarkdownViewer;
using Markdig;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
namespace com.insanitydesign.MarkdownViewerPlusPlus.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public abstract partial class AbstractRenderer : Form
    {
        /// <summary>
        /// 
        /// </summary>
        protected Icon toolbarIcon;

        /// <summary>
        /// 
        /// </summary>
        protected MarkdownViewer markdownViewer;

        /// <summary>
        /// 
        /// </summary>
        protected string assemblyTitle;

        /// <summary>
        /// 
        /// </summary>
        protected virtual string RawText { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected virtual string ConvertedText { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected virtual FileInformation FileInfo { get; set; }

        protected Double LastScrollRatio;

        /// <summary>
        /// 
        /// </summary>
        protected MarkdownPipeline markdownPipeline = new MarkdownPipelineBuilder()
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
            .Use<MarkdigExtensions.VisualTaskLists.VisualTaskListExtension>()
            .UseDiagrams()
            .UseAutoLinks()
            .UseGenericAttributes()
            .UseSoftlineBreakAsHardlineBreak()
            .Build();

        /// <summary>
        /// 
        /// </summary>
        protected string patternCSSImportStatements = "(@import\\s.+;)";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="markdownViewer"></param>
        public AbstractRenderer(MarkdownViewer markdownViewer)
        {
            this.markdownViewer = markdownViewer;
            InitializeComponent();
            Init();
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Init()
        {
            //
            using (Bitmap newBmp = new Bitmap(16, 16))
            {
                Graphics g = Graphics.FromImage(newBmp);
                ColorMap[] colorMap = new ColorMap[1];
                colorMap[0] = new ColorMap();
                colorMap[0].OldColor = Color.Fuchsia;
                colorMap[0].NewColor = Color.FromKnownColor(KnownColor.ButtonFace);
                ImageAttributes attr = new ImageAttributes();
                attr.SetRemapTable(colorMap);
                g.DrawImage(Resources.markdown_16x16_solid_bmp, new Rectangle(0, 0, 16, 16), 0, 0, 16, 16, GraphicsUnit.Pixel, attr);
                toolbarIcon = Icon.FromHandle(newBmp.GetHicon());
            }
            //Get the AssemblyTitle
            this.assemblyTitle = ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute), false)).Title;
            //
            NppTbData _nppTbData = new NppTbData();
            _nppTbData.hClient = this.Handle;
            _nppTbData.pszName = this.assemblyTitle;
            _nppTbData.dlgID = this.markdownViewer.commandId;
            _nppTbData.uMask = NppTbMsg.DWS_DF_CONT_RIGHT | NppTbMsg.DWS_ICONTAB | NppTbMsg.DWS_ICONBAR;
            _nppTbData.hIconTab = (uint)toolbarIcon.Handle;
            _nppTbData.pszModuleName = Main.PluginName;
            IntPtr _ptrNppTbData = Marshal.AllocHGlobal(Marshal.SizeOf(_nppTbData));
            Marshal.StructureToPtr(_nppTbData, _ptrNppTbData, false);
            //Register dockable window and hide initially
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMREGASDCKDLG, 0, _ptrNppTbData);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMHIDE, 0, this.Handle);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            //Listen for the closing of the dockable panel to toggle the toolbar icon
            switch (m.Msg)
            {
                case (int)WM_NOTIFY:
                    var notify = (NMHDR)Marshal.PtrToStructure(m.LParam, typeof(NMHDR));
                    if (notify.code == (int)DockMgrMsg.DMN_CLOSE)
                    {
                        this.markdownViewer.ToggleToolbarIcon(false);
                    }
                    break;
            }
            //Continue the processing, as we only toggle
            base.WndProc(ref m);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fileInfo"></param>
        public virtual void Render(string text, FileInformation fileInfo)
        {
            FileInfo = fileInfo;
            RawText = text;
            ConvertedText = Markdown.ToHtml(text, this.markdownPipeline);
        }

        /// <summary>
        /// Scroll the rendered panel vertically based on the given ration
        /// taken from Notepad++
        /// </summary>
        /// <param name="scrollRatio"></param>
        public abstract void ScrollByRatioVertically(double scrollRatio);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        protected string BuildHtml(string html = "", string title = "")
        {
            //
            if (title == "") title = this.assemblyTitle;
            //
            return $@"
<!DOCTYPE html>
<html>
    <head>
        <meta charset=""UTF-8"" />
        <meta name=""author"" content=""{this.assemblyTitle}"" />
        <title>{title}</title>
        <style type=""text/css"">
            {this.getCSS()}
        </style>
        <script src=""https://unpkg.com/mermaid@8.0.0-rc.8/dist/mermaid.min.js""></script>
        <script>
            const lastScrollRatio = {LastScrollRatio.ToString(System.Globalization.CultureInfo.InvariantCulture)};

            mermaid.initialize({{startOnLoad:true}});

            window.addEventListener(""load"", function(event) {{
                const visibleHeight = window.innerHeight,
                      scrollHeight = document.scrollingElement.scrollHeight;

                window.scrollTo(0, (scrollHeight - visibleHeight) * lastScrollRatio);
            }});
        </script>
      </head>
    <body class=""numbered-section"">
        {html}
    </body>
</html>
            ";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual String getCSS()
        {
            string cssImportStatements = "";
            
            //Get all @import statements from the custom CSS
            List<Match> matches = Regex.Matches(this.markdownViewer.Options.HtmlCssStyle, this.patternCSSImportStatements).Cast<Match>().ToList();
            matches.ForEach(match => match.Captures.Cast<Capture>().ToList().ForEach(capture => cssImportStatements += capture.Value + Environment.NewLine));            
            
            //Return a CSS with the @import statements in front, the base MarkdownViewer++ CSS and the rest of the custom CSS from the user
            return cssImportStatements + Environment.NewLine + Resources.MarkdownViewerHTML + Environment.NewLine + Regex.Replace(this.markdownViewer.Options.HtmlCssStyle, this.patternCSSImportStatements, "").Trim();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mm"></param>
        /// <returns></returns>
        protected int MilimiterToPoint(int mm)
        {
            return (int)(mm * 2.834646);
        }
    }
}
