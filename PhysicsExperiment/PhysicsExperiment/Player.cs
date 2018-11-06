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

        public double speed = 250;
        public double jump = 600;

        private bool takeoff = false;
        private int takeOffChecks = 0;
        private bool inAir = false;

        private BoxCollider hitbox;


        private Rectangle avatar;

        // Projectile motion programmed using these formulas.
        // https://formulas.tutorvista.com/physics/projectile-motion-formula.html

        private double initialY;
        private double initialAirVelocity;
        private double airTime;

        public Player(Rectangle playerHitBox)
        {
            avatar = playerHitBox;
            hitbox = new BoxCollider(avatar);
        }

        public void Update(object sender, EventArgs e)
        {
            Vector movement = new Vector(0, -.001);


            bool floorBelow = false;
            if (hitbox.ProjectionCast(new Vector(0, 0.4)))
            {
                floorBelow = true;
            }

            if (inAir)
            {
                if (floorBelow)
                {
                    if (takeoff)
                    {
                        takeoff = false;

                        movement.Y += UpdateElevation();
                    }
                    else
                    {
                        inAir = false;
                    }
                }
                else
                {
                    movement.Y += UpdateElevation();
                }
            }
            else
            {
                if (floorBelow)
                {
                    if (Keyboard.IsKeyDown(Key.Space))
                    {
                        initialY = hitbox.topEdge;
                        initialAirVelocity = jump;
                        airTime = 0;

                        takeoff = true;
                        takeOffChecks = 0;
                        inAir = true;
                    }
                }
                else
                {
                    initialY = hitbox.topEdge;
                    initialAirVelocity = 0;
                    airTime = 0;

                    inAir = true;
                }
            }

            if (Keyboard.IsKeyDown(Key.D))
            {
                movement.X += speed * Game.deltaTime;
            }

            if (Keyboard.IsKeyDown(Key.A))
            {
                movement.X -= speed * Game.deltaTime;
            }

            hitbox.Move(movement);

            BoxCollider collision = Collision.RectangleCast(hitbox);
            if (collision != null)
            {
                BoxCollider.PushOut(ref hitbox, collision);
            }

            DrawPlayer();

        }


        private void DrawPlayer()
        {
            avatar.Margin = new Thickness(hitbox.leftEdge, hitbox.topEdge, avatar.Margin.Right, avatar.Margin.Bottom);
        }

        private double UpdateElevation()
        {
            airTime += Game.deltaTime;

            // The relative Y is calculated for a Y up world.
            return -(initialAirVelocity * airTime - 0.5 * Game.gravity * Math.Pow(airTime, 2.0)) + (initialY - hitbox.topEdge);
        }
    }
}
