using System.Drawing.Drawing2D;

namespace HorseRace
{
  
    public class ColoredProgressBar : ProgressBar
    {
        public Color BarColor { get; set; } = Color.Red;

        public ColoredProgressBar()
        {
            SetStyle(ControlStyles.UserPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rect = ClientRectangle;
            using var backBrush = new SolidBrush(Color.WhiteSmoke);
            e.Graphics.FillRectangle(backBrush, rect);

            if (Maximum > Minimum)
            {
                double percent = (double)(Value - Minimum) / (Maximum - Minimum);
                int fillWidth = (int)(rect.Width * percent);

                if (fillWidth > 0)
                {
                    Rectangle fillRect = new Rectangle(rect.X, rect.Y, fillWidth, rect.Height);
                    using var fillBrush = new SolidBrush(BarColor);
                    e.Graphics.FillRectangle(fillBrush, fillRect);
                }
            }

            using var pen = new Pen(Color.Silver);
            e.Graphics.DrawRectangle(pen, 0, 0, rect.Width - 1, rect.Height - 1);
        }
    }
}