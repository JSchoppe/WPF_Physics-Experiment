using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace PhysicsExperiment
{

    public class Player
    {

        public double speed = 250;
        public double jump = 600;

        private bool takeoff = false;
        private bool ceilingHit = false;
        private bool inAir = false;

        private BoxCollider hitbox;


        private Image avatar;

        // Projectile motion programmed using these formulas.
        // https://formulas.tutorvista.com/physics/projectile-motion-formula.html

        private double initialY;
        private double initialAirVelocity;
        private double airTime;

        public Player(Windows.Level inWindow)
        {

            avatar = inWindow.PlayerSprite;

            hitbox = new BoxCollider(
                WindowManager.UnitarySpaceToWindowSpace(12),
                WindowManager.UnitarySpaceToWindowSpace(3),
                WindowManager.UnitarySpaceToWindowSpace(13),
                WindowManager.UnitarySpaceToWindowSpace(5)
            );

            avatar.Width = WindowManager.UnitarySpaceToWindowSpace(1);
            avatar.Height = WindowManager.UnitarySpaceToWindowSpace(2);
        }

        public void Update()
        {
            Vector movement = new Vector(0, -.001);


            bool floorBelow = hitbox.ProjectionCast(new Vector(0, 0.1));

            bool ceilingAbove = hitbox.ProjectionCast(new Vector(0, -0.5));

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
                        ceilingHit = false;
                        inAir = false;
                    }
                }
                else if (ceilingAbove)
                {
                    if (ceilingHit)
                    {
                        movement.Y += UpdateElevation();
                    }
                    else
                    {
                        initialY = hitbox.topEdge;
                        initialAirVelocity = 0;
                        airTime = 0;

                        ceilingHit = true;
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
