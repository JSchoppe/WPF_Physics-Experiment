using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Input;

namespace PhysicsExperiment
{
    //public class PlayerInputBindings
    //{
    //    public List<Key> action1 = new List<Key>()
    //    {
    //        Key.Space
    //    };

    //    public List<Key>

    //}

    //public class PlayerStats
    //{

    //}


    public class Player
    {

        public double speed = 120;
        public double jump = 250;

        private bool takeoff = false;
        private int takeOffChecks = 0;
        private bool inAir = false;

        private Rectangle hitBox;

        // Projectile motion programmed using these formulas.
        // https://formulas.tutorvista.com/physics/projectile-motion-formula.html

        private double initialY;
        private double initialAirVelocity;
        private double airTime;

        public Player(Rectangle playerHitBox)
        {
            hitBox = playerHitBox;
        }

        public void Update(object sender, EventArgs e)
        {
            bool floorBelow = Collision.RectangleDownCast(hitBox);

            if (inAir)
            {
                if (floorBelow)
                {
                    if (takeoff)
                    {
                        takeoff = false;

                        UpdateElevation();
                    }
                    else
                    {
                        inAir = false;
                    }
                }
                else
                {
                    UpdateElevation();
                }
            }
            else
            {
                if (floorBelow)
                {
                    if (Keyboard.IsKeyDown(Key.Space))
                    {
                        initialY = hitBox.Margin.Top;
                        initialAirVelocity = jump;
                        airTime = 0;

                        takeoff = true;
                        takeOffChecks = 0;
                        inAir = true;
                    }
                }
                else
                {
                    initialY = hitBox.Margin.Top;
                    initialAirVelocity = 0;
                    airTime = 0;

                    inAir = true;
                }
            }

            if (Keyboard.IsKeyDown(Key.D))
            {
                hitBox.Margin = new Thickness(hitBox.Margin.Left + Game.deltaTime *speed, hitBox.Margin.Top, hitBox.Margin.Right, hitBox.Margin.Bottom);
            }

            if (Keyboard.IsKeyDown(Key.A))
            {
                hitBox.Margin = new Thickness(hitBox.Margin.Left - Game.deltaTime * speed, hitBox.Margin.Top, hitBox.Margin.Right, hitBox.Margin.Bottom);
            }
        }

        private void UpdateElevation()
        {
            airTime += Game.deltaTime;

            // The relative Y is calculated for a Y up world.
            double relativeY = initialAirVelocity * airTime - 0.5 * Game.gravity * Math.Pow(airTime, 2.0);

            hitBox.Margin = new Thickness(hitBox.Margin.Left, initialY - relativeY, hitBox.Margin.Right, hitBox.Margin.Bottom);
        }
    }
}
