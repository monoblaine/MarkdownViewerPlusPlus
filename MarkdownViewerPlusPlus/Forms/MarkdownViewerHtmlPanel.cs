using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Toolkit.Win32.UI.Controls.WinForms;

/// <summary>
/// 
/// </summary>
namespace com.insanitydesign.MarkdownViewerPlusPlus.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public class MarkdownViewerHtmlPanel : Control
    {
        private WebView _webView;
        private String _text;

        /// <summary>
        /// 
        /// </summary>
        public MarkdownViewerHtmlPanel()
        {
            BackColor = SystemColors.Window;
            DoubleBuffered = true;
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            _webView = new WebView {
                Dock = DockStyle.Fill,
                IsJavaScriptEnabled = true,
                CausesValidation = false,
                IsIndexedDBEnabled = false,
                IsPrivateNetworkClientServerCapabilityEnabled = false,
                IsScriptNotifyAllowed = false,
                TabIndex = 0
            };

            Controls.Add(_webView);

            this.AllowDrop = false;
            this.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Location = new System.Drawing.Point(0, 24);
            this.MinimumSize = new System.Drawing.Size(20, 20);
            this.Name = "markdownViewerHtmlPanel";
            this.Size = new System.Drawing.Size(284, 237);
            this.TabIndex = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        public override string Text {
            get { return _text; }
            set {
                _text = value;

                Task.Delay(100).ContinueWith(t => {
                    if (!IsDisposed && !_webView.IsDisposed && _webView.Visible) {
                        _webView.NavigateToString(_text);
                    }
                });
            }
        }

        /// <summary>
        /// Scroll by the given ratio, calculated with max and page
        /// </summary>
        /// <param name="scrollRatio"></param>
        public void ScrollByRatioVertically(double scrollRatio)
        {
            if (!IsDisposed)
            {
            }
        }
    }
}
