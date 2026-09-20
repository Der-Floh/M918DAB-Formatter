using System.ComponentModel;
using System.Windows.Forms.VisualStyles;

namespace M918DAB_Formatter;

[ToolboxItem(true)]
public class MultiHandleTrackBar : TrackBar
{
    /// <summary>
    /// Values at which an additional, read-only thumb is shown.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public List<int> ExtraValues { get; } = [];

    public MultiHandleTrackBar()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        TabStop = false;
    }

    protected override void OnMouseDown(MouseEventArgs e) { }
    protected override void OnMouseMove(MouseEventArgs e) { }
    protected override void OnMouseUp(MouseEventArgs e) { }
    protected override void OnKeyDown(KeyEventArgs e) { }

    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);

        const int WM_PAINT = 0x000F;
        if (m.Msg == WM_PAINT)
        {
            using var g = Graphics.FromHwnd(Handle);
            DrawExtraThumbs(g);
        }
    }

    private void DrawExtraThumbs(Graphics g)
    {
        if (ExtraValues is null || ExtraValues.Count == 0 || !Application.RenderWithVisualStyles)
            return;

        var rnd = CreateThumbRenderer();
        foreach (var extraValue in ExtraValues)
        {
            if (extraValue < Minimum || extraValue > Maximum)
                continue;
            rnd.DrawBackground(g, ThumbBoundsForValue(extraValue, rnd, g));
        }
    }

    private VisualStyleRenderer CreateThumbRenderer()
    {
        VisualStyleElement element;

        if (Orientation == Orientation.Horizontal)
        {
            var ticksOnTop = TickStyle == TickStyle.TopLeft;
            element = ticksOnTop
                      ? VisualStyleElement.TrackBar.ThumbTop.Normal
                      : VisualStyleElement.TrackBar.ThumbBottom.Normal;
        }
        else
        {
            var ticksOnLeft = TickStyle == TickStyle.TopLeft;
            element = ticksOnLeft
                      ? VisualStyleElement.TrackBar.ThumbLeft.Normal
                      : VisualStyleElement.TrackBar.ThumbRight.Normal;
        }

        return new VisualStyleRenderer(element);
    }

    private Rectangle ThumbBoundsForValue(int value, VisualStyleRenderer renderer, Graphics g)
    {
        // Thumb size as painted by the current visual style
        var thumbSize = renderer.GetPartSize(g, ThemeSizeType.True);

        // Compute pixel coordinate of the value's center.
        var trackLen = Orientation == Orientation.Horizontal ? Width : Height;
        var usable = trackLen - thumbSize.Width;
        var percent = (value - Minimum) / (double)(Maximum - Minimum);
        var center = (int)Math.Round(percent * usable) + (thumbSize.Width / 2);

        return Orientation == Orientation.Horizontal
               ? new Rectangle(center - (thumbSize.Width / 2), (Height / 2) - thumbSize.Height - 1, thumbSize.Width, thumbSize.Height)
               : new Rectangle((Width / 2) - thumbSize.Width - 1, trackLen - center - (thumbSize.Height / 2), thumbSize.Width, thumbSize.Height);
    }
}
