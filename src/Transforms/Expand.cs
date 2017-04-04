using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShapeEditor
{
    public class Expand : ITransform
    {
        Point old_left_botton, old_right_top, center;
        double kx, ky;
        int bottom_rigth_top_left; // в какую сторону тянем
        public Expand(Point _old_left_botton, Point _old_rigth_top, double delta_x, double delta_y, int _bottom_rigth_top_left)
        {
            old_left_botton = _old_left_botton;
            old_right_top = _old_rigth_top;
            bottom_rigth_top_left = _bottom_rigth_top_left;
            if (bottom_rigth_top_left > 4 || bottom_rigth_top_left < 1)
                throw new Exception("Expand: ITransform: bottom_rigth_top_left must be in {1, 2, 3, 4}");
            center.X = (old_right_top.X + old_left_botton.X) * 0.5;
            center.Y = (old_right_top.Y + old_left_botton.Y) * 0.5;
            kx = delta_x / (old_right_top.X - old_left_botton.X) * 0.5;
            ky = delta_y / (old_right_top.Y - old_left_botton.Y) * 0.5;
        }
        public Point Transform(Point p)
        {
            Point res;
            switch (bottom_rigth_top_left)
            {
                case 1:
                    res = new Point(kx * p.X + (1 - kx) * center.X, ky * p.Y + (1 - ky) * center.Y);
                    break;
                case 2:
                    res = new Point((1 - kx) * p.X + kx * center.X, ky * p.Y + (1 - ky) * center.Y);
                    break;
                case 3:
                    res = new Point((1 - kx) * p.X + kx * center.X, (1 - ky) * p.Y + ky * center.Y);
                    break;
                case 4:
                    res = new Point(kx * p.X + (1 - kx) * center.X, (1 - ky) * p.Y + ky * center.Y);
                    break;
                default:
                    res = new Point(kx * p.X + (1 - kx) * center.X, ky * p.Y + (1 - ky) * center.Y);
                    break;
            }
            return res;
        }
    }
}
