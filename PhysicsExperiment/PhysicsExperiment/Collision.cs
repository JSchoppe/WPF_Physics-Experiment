using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows;

namespace PhysicsExperiment
{
    public static class Collision
    {
        private static List<BoxCollider> sceneColliders = new List<BoxCollider>();


        public static bool RectangleDownCast(Rectangle toCast)
        {
            BoxCollider toCastAsCollider = new BoxCollider(toCast);

            foreach (BoxCollider collider in sceneColliders)
            {
                if (collider.leftEdge < toCastAsCollider.rightEdge && collider.rightEdge > toCastAsCollider.leftEdge)
                {
                    if (toCastAsCollider.bottomEdge >= collider.topEdge && toCastAsCollider.bottomEdge < collider.bottomEdge)
                    {
                        toCast.Margin = new Thickness(toCast.Margin.Left, collider.topEdge - toCast.Height, toCast.Margin.Right, toCast.Margin.Bottom);
                        return true;
                    }
                }
            }

            return false;
        }

        public static void AddColliderFromRectangle(Rectangle rectangleObject)
        {
            sceneColliders.Add(new BoxCollider(rectangleObject));
        }
    }

    class BoxCollider
    {
        public double topEdge = 0;
        public double bottomEdge = 0;
        public double leftEdge = 0;
        public double rightEdge = 0;

        public BoxCollider (Rectangle rect)
        {
            topEdge = rect.Margin.Top;
            bottomEdge = rect.Margin.Top + rect.Height;

            leftEdge = rect.Margin.Left;
            rightEdge = rect.Margin.Left + rect.Width;
        }
    }


}
