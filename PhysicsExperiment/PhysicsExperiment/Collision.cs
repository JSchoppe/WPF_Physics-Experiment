using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows;
using static System.Math;

namespace PhysicsExperiment
{
    public static class Collision
    {
        public static List<BoxCollider> sceneColliders = new List<BoxCollider>();

        public static BoxCollider RectangleCast(BoxCollider toCast)
        {
            foreach (BoxCollider collider in sceneColliders)
            {
                if (BoxCollider.Overlapping(collider, toCast))
                {
                    return collider;
                }
            }
            return null;
        }

        public static void SetCollidersFromNormalizedSet(BoxCollider[] colliders)
        {
            sceneColliders = colliders.ToList();

            foreach (BoxCollider collider in sceneColliders)
            {
                collider.leftEdge = WindowManager.UnitarySpaceToWindowSpace(collider.leftEdge);
                collider.rightEdge = WindowManager.UnitarySpaceToWindowSpace(collider.rightEdge);
                collider.topEdge = WindowManager.UnitarySpaceToWindowSpace(collider.topEdge);
                collider.bottomEdge = WindowManager.UnitarySpaceToWindowSpace(collider.bottomEdge);
            }
        }



        public static void AddColliderFromRectangle(Rectangle rectangleObject)
        {
            sceneColliders.Add(new BoxCollider(rectangleObject));
        }

    }

    public class BoxCollider
    {
        public double topEdge = 0;
        public double bottomEdge = 0;
        public double leftEdge = 0;
        public double rightEdge = 0;

        public double width = 0;
        public double height = 0;

        public BoxCollider (Rectangle rect)
        {
            topEdge = rect.Margin.Top;
            bottomEdge = rect.Margin.Top + rect.Height;

            leftEdge = rect.Margin.Left;
            rightEdge = rect.Margin.Left + rect.Width;

            width = rect.Width;
            height = rect.Height;
        }

        public BoxCollider(double left, double top, double right, double bottom)
        {
            topEdge = top;
            bottomEdge = bottom;

            leftEdge = left;
            rightEdge = right;

            width = right - left;
            height = bottom - top;
        }

        public void Move(Vector offset)
        {
            topEdge += offset.Y;
            bottomEdge += offset.Y;

            leftEdge += offset.X;
            rightEdge += offset.X;
        }

        public bool ProjectionCast(Vector offset)
        {
            Move(offset);

            BoxCollider collision = Collision.RectangleCast(this);

            Move(-offset);

            if (collision != null)
            {
                // Calculate the magnitude of push required to seperate these colliders in each direction.
                double rightPush = Abs(collision.rightEdge - this.leftEdge);
                double leftPush = Abs(collision.leftEdge - this.rightEdge);
                double upPush = Abs(collision.topEdge - this.bottomEdge);
                double downPush = Abs(collision.bottomEdge - this.topEdge);

                Direction willPushTo = CommonUtilities.PolarizeVectorDirection(offset);

                double[] pushVals = { rightPush, leftPush, upPush, downPush };

                switch (CommonUtilities.IndexOfSmallest(pushVals))
                {
                    // Push out to the right.
                    case 0:
                        return willPushTo == Direction.Right;

                    // Push out to the left.
                    case 1:
                        return willPushTo == Direction.Left;

                    // Push out to the top.
                    case 2:
                        return willPushTo == Direction.Up;

                    // Push out to the bottom.
                    case 3:
                        return willPushTo == Direction.Down;
                }
            }
            return false;
        }

        /// <summary>Determines if two box colliders are overlapping</summary>
        /// <param name="A">Box A</param>
        /// <param name="B">Box B</param>
        /// <returns>True if these two boxes are overlapping</returns>
        public static bool Overlapping(BoxCollider A, BoxCollider B)
        {
            // Collider intersection is based on coordinate overlap.
            //
            //       o----------o
            //       Al         Ar
            //                     Bl  Br
            //                     o---o
            //
            // If both Bleft and Bright are greater than Aright, or the inverse,
            // then the colliders are not overlapping on this axis.

            if (B.leftEdge >= A.rightEdge && B.rightEdge >= A.rightEdge)
            {
                return false;
            }
            else if (B.leftEdge <= A.leftEdge && B.rightEdge <= A.leftEdge)
            {
                return false;
            }
            else if (B.bottomEdge <= A.topEdge && B.topEdge <= A.topEdge)
            {
                return false;
            }
            else if (B.bottomEdge >= A.bottomEdge && B.topEdge >= A.bottomEdge)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>Pushes one collider out of another</summary>
        /// <param name="pushThis">The collider that will be moved</param>
        /// <param name="outOfThis">The static collider that will be pushing</param>
        public static void PushOut(ref BoxCollider pushThis, BoxCollider outOfThis)
        {
            // Calculate the magnitude of push required to seperate these colliders in each direction.
            double rightPush = Abs(outOfThis.rightEdge - pushThis.leftEdge);
            double leftPush = Abs(outOfThis.leftEdge - pushThis.rightEdge);
            double upPush = Abs(outOfThis.topEdge - pushThis.bottomEdge);
            double downPush = Abs(outOfThis.bottomEdge - pushThis.topEdge);

            double[] pushVals = { rightPush, leftPush, upPush, downPush };

            switch (CommonUtilities.IndexOfSmallest(pushVals))
            {
                // Push out to the right.
                case 0:
                    pushThis.leftEdge = outOfThis.rightEdge;
                    pushThis.rightEdge = outOfThis.rightEdge + pushThis.width;
                    break;

                // Push out to the left.
                case 1:
                    pushThis.leftEdge = outOfThis.leftEdge - pushThis.width;
                    pushThis.rightEdge = outOfThis.leftEdge;
                    break;

                // Push out to the top.
                case 2:
                    pushThis.topEdge = outOfThis.topEdge - pushThis.height;
                    pushThis.bottomEdge = outOfThis.topEdge;
                    break;

                // Push out to the bottom.
                case 3:
                    pushThis.topEdge = outOfThis.bottomEdge;
                    pushThis.bottomEdge = outOfThis.bottomEdge + pushThis.height;
                    break;
            }
        }
    }
}
