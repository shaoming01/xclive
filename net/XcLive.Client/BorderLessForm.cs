namespace Frame;

public partial class BorderLessForm : Form
{
    private bool _isResizing = false;
    private Point _lastMousePosition;
    private Label _resizeIcon;

    public BorderLessForm()
    {
        InitializeComponent();
    }

    public void SetNoBorder(bool noBorder)
    {
        FormBorderStyle = noBorder ? FormBorderStyle.None : FormBorderStyle.Sizable; // 隐藏边框
        IniResizeIcon();
    }

    private void IniResizeIcon()
    {
        if (_resizeIcon != null)
        {
            return;
        }

        _resizeIcon = new Label()
        {
            Cursor = Cursors.SizeNWSE, // 鼠标变为调整大小
            Size = new Size(16, 16),
            //Location = new Point(2, 2),
            //Image = Image.FromFile("resize.png"), // 替换为你的图片
            Text = "\u2725", //✥✠
            BackColor = Color.FromArgb(255, 255, 255),
        };

        // 绑定鼠标事件
        _resizeIcon.MouseDown += ResizeGrip_MouseDown;
        _resizeIcon.MouseMove += ResizeGrip_MouseMove;
        _resizeIcon.MouseUp += ResizeGrip_MouseUp;
        this.Controls.Add(_resizeIcon);

        // 窗口大小改变时调整拖拽角位置
        this.SizeChanged += (s, e) => PositionResizeGrip();
        PositionResizeGrip();
        _resizeIcon.BringToFront();
    }

    private void PositionResizeGrip()
    {
        _resizeIcon.Location = new Point(this.ClientSize.Width - _resizeIcon.Width - 2,
            this.ClientSize.Height - _resizeIcon.Height - 2);
    }

    private void ResizeGrip_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            _isResizing = true;
            _lastMousePosition = Cursor.Position;
        }
    }

    private void ResizeGrip_MouseMove(object sender, MouseEventArgs e)
    {
        if (_isResizing)
        {
            Point currentMousePosition = Cursor.Position;
            int dx = currentMousePosition.X - _lastMousePosition.X;
            int dy = currentMousePosition.Y - _lastMousePosition.Y;

            // 计算新的宽度和高度
            int newWidth = Width + dx;
            int newHeight = Height + dy;

            // 不能小于 MinimumSize
            if (newWidth < MinimumSize.Width)
                newWidth = MinimumSize.Width;
            else
                _lastMousePosition.X = currentMousePosition.X; // 只有当尺寸真的变化时才更新鼠标位置

            if (newHeight < MinimumSize.Height)
                newHeight = MinimumSize.Height;
            else
                _lastMousePosition.Y = currentMousePosition.Y; // 只有当尺寸真的变化时才更新鼠标位置

            // 更新窗口大小
            this.Size = new Size(newWidth, newHeight);
        }
    }

    private void ResizeGrip_MouseUp(object sender, MouseEventArgs e)
    {
        _isResizing = false;
    }
}