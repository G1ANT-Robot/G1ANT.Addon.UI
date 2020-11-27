using FlaUI.Core.AutomationElements;
using FlaUI.Core.Capturing;
using Gma.System.MouseKeyHook;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace G1ANT.Addon.UI.Api
{
    public class InspectUIElement : Form
    {
        public class InspectSelectedElement
        {
            public Bitmap Image { get; }
            public AutomationElement AutomationElement { get; }
            public string WPath { get; }
            public string Name { get; }

            private static WPathBuilder wpathBuilder = new WPathBuilder();

            public InspectSelectedElement(AutomationElement element)
            {
                AutomationElement = element;
                Image = FlaUI.Core.Capturing.Capture.Element(element)?.Bitmap;
                Name = element.Name;
                WPath = wpathBuilder.GetSimpleWPath(element);
            }
        }


        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hwndAfter, int x, int y, int width, int height, int flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SWP_NOACTIVATE = 0x0010;
        private const int SW_SHOWNA = 8;

        private IKeyboardMouseEvents mouseHook;
        private AutomationElement highlitedElement;

        public Color BorderColor { get; set; } = Color.Blue;

        public delegate void FinishedHandler();
        public event FinishedHandler OnFinished;

        public delegate void ElementSelectedHandler(InspectSelectedElement element);
        public event ElementSelectedHandler OnElementClicked;

        public InspectUIElement()
        {
            DoubleBuffered = true;
            ShowInTaskbar = false;
            ControlBox = false;
            TopMost = true;
            FormBorderStyle = FormBorderStyle.None;
            MinimizeBox = MaximizeBox = false;
            TransparencyKey = BackColor = Color.Pink;
        }

        public void Start()
        {
            highlitedElement = null;
            SubscribeEvent();
        }

        public void Stop()
        {
            FinishInspect();
        }

        protected override bool ShowWithoutActivation => true;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x80000 /* WS_EX_LAYERED */ | 0x20 /* WS_EX_TRANSPARENT */ | 0x08 /*WS_EX_TOPMOST*/;
                return cp;
            }
        }

        private Pen GetPen()
        {
            return new Pen(BorderColor, 2);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var rect = ClientRectangle;

            rect.Inflate(-1, -1);
            e.Graphics.DrawRectangle(GetPen(), rect);
        }

        private void FinishInspect()
        {
            Hide();
            UnsubscribeEvent();
            OnFinished?.Invoke();
        }

        private void OnMouseDown(object sender, MouseEventExtArgs e)
        {
            try
            {
                var control = AutomationSingleton.Automation.FromPoint(e.Location);
                if (control != null)
                {
                    Hide();
                    var element = new InspectSelectedElement(control);
                    Show();
                    OnElementClicked?.Invoke(element);
                }
            }
            catch
            {

            }
        }

        private void OnMouseMove(object sender, MouseEventExtArgs e)
        {
            try
            {
                var control = AutomationSingleton.Automation.FromPoint(e.Location);
                HighliteElement(control);
            }
            catch
            {
                HighliteElement(null);
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                FinishInspect();
        }

        private void HighliteElement(AutomationElement control)
        {
            if (control != null)
            {
                if (!control.Equals(highlitedElement))
                {
                    Hide();
                    highlitedElement = control;
                    InitializeRectangleForm(control.BoundingRectangle);
                }
            }
            else
            {
                Hide();
                highlitedElement = null;
            }
        }

        private void InitializeRectangleForm(Rectangle rect)
        {
            Location = rect.Location;
            Size = rect.Size;
            SetWindowPos(Handle, new IntPtr(-1), Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height, SWP_NOACTIVATE);
            ShowWindow(Handle, SW_SHOWNA);
        }

        private void SubscribeEvent()
        {
            mouseHook = Hook.GlobalEvents();
            mouseHook.KeyDown += OnKeyDown;
            mouseHook.MouseDownExt += OnMouseDown;
            mouseHook.MouseMoveExt += OnMouseMove;
        }

        private void UnsubscribeEvent()
        {
            if (mouseHook != null)
            {
                mouseHook.MouseMoveExt -= OnMouseMove;
                mouseHook.MouseDownExt -= OnMouseDown;
                mouseHook.KeyDown -= OnKeyDown;
                mouseHook.Dispose();
            }
        }
    }
}
