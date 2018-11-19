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
    // Represents a playable character that recieves control from the keyboard.
    public class Player
    {
        // Player stats.
        public double speed = 8;
        public double jump = 15;

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

        private Window currentWindow;

        private byte currentX = 1;
        private byte currentY = 1;

        public Player(Level inWindow)
        {
            currentWindow = inWindow;
            avatar = inWindow.PlayerSprite;

            hitbox = new BoxCollider(12, 3, 13, 5);

            avatar.Width = WindowManager.UnitarySpaceToWindowSpace(1);
            avatar.Height = WindowManager.UnitarySpaceToWindowSpace(2);
        }

        public void Update()
        {
            Vector movement = new Vector(0, .01);

            if (Keyboard.IsKeyDown(Key.Left))
            {
                WindowManager.PushAll(Direction.Left);
            }

            if (Keyboard.IsKeyDown(Key.Right))
            {
                WindowManager.PushAll(Direction.Right);
            }

            if (Keyboard.IsKeyDown(Key.Up))
            {
                WindowManager.PushAll(Direction.Up);
            }

            if (Keyboard.IsKeyDown(Key.Down))
            {
                WindowManager.PushAll(Direction.Down);
            }



            bool floorBelow = hitbox.ProjectionCast(new Vector(0, 0.01));

            Inventory invent= (Inventory)WindowManager.windowMatrix[0];
            if (floorBelow)
            {
                invent.LiveWatchUpdate("Grounded", "true");
            }
            else
            {
                invent.LiveWatchUpdate("Grounded", "false");
            }

            bool ceilingAbove = hitbox.ProjectionCast(new Vector(0, -0.1));

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

            if (hitbox.rightEdge < 0)
            {
                Level levelLeft = (Level)WindowManager.GetWindowDirection(currentWindow, Direction.Left);
                currentWindow = levelLeft;

                if (levelLeft != null)
                {
                    avatar.Width = 0;
                    avatar.Height = 0;
                    avatar = levelLeft.PlayerSprite;

                    avatar.Width = WindowManager.UnitarySpaceToWindowSpace(1);
                    avatar.Height = WindowManager.UnitarySpaceToWindowSpace(2);

                    hitbox.leftEdge += 24;
                    hitbox.rightEdge += 24;

                    currentX--;

                    Collision.SetCollidersFromNormalizedSet(World.maps[currentX, currentY].staticColliders);
                }
            }

            if (hitbox.leftEdge > 24)
            {
                Level levelRight = (Level)WindowManager.GetWindowDirection(currentWindow, Direction.Right);
                currentWindow = levelRight;

                if (levelRight != null)
                {
                    avatar.Width = 0;
                    avatar.Height = 0;
                    avatar = levelRight.PlayerSprite;

                    avatar.Width = WindowManager.UnitarySpaceToWindowSpace(1);
                    avatar.Height = WindowManager.UnitarySpaceToWindowSpace(2);

                    hitbox.leftEdge -= 24;
                    hitbox.rightEdge -= 24;

                    currentX++;

                    Collision.SetCollidersFromNormalizedSet(World.maps[currentX, currentY].staticColliders);
                }
            }

            if (hitbox.topEdge > 12)
            {
                Level levelDown = (Level)WindowManager.GetWindowDirection(currentWindow, Direction.Down);
                currentWindow = levelDown;

                if (levelDown != null)
                {
                    avatar.Width = 0;
                    avatar.Height = 0;
                    avatar = levelDown.PlayerSprite;

                    avatar.Width = WindowManager.UnitarySpaceToWindowSpace(1);
                    avatar.Height = WindowManager.UnitarySpaceToWindowSpace(2);

                    hitbox.topEdge -= 12;
                    hitbox.bottomEdge -= 12;
                    initialY -= 12;

                    currentY++;

                    Collision.SetCollidersFromNormalizedSet(World.maps[currentX, currentY].staticColliders);
                }
            }

            if (hitbox.bottomEdge < 0)
            {
                Level levelUp = (Level)WindowManager.GetWindowDirection(currentWindow, Direction.Up);
                currentWindow = levelUp;

                if (levelUp != null)
                {
                    avatar.Width = 0;
                    avatar.Height = 0;
                    avatar = levelUp.PlayerSprite;

                    avatar.Width = WindowManager.UnitarySpaceToWindowSpace(1);
                    avatar.Height = WindowManager.UnitarySpaceToWindowSpace(2);

                    hitbox.topEdge += 12;
                    hitbox.bottomEdge += 12;
                    initialY += 12;

                    currentY--;

                    Collision.SetCollidersFromNormalizedSet(World.maps[currentX, currentY].staticColliders);
                }
            }
            DrawPlayer();
        }


        private void DrawPlayer()
        {
            avatar.Margin = new Thickness(
                WindowManager.UnitarySpaceToWindowSpace(hitbox.leftEdge),
                WindowManager.UnitarySpaceToWindowSpace(hitbox.topEdge),
                WindowManager.UnitarySpaceToWindowSpace(avatar.Margin.Right),
                WindowManager.UnitarySpaceToWindowSpace(avatar.Margin.Bottom));
        }

        private double UpdateElevation()
        {
            airTime += Game.deltaTime;

            // The relative Y is calculated for a Y up world.
            return -(initialAirVelocity * airTime - 0.5 * Game.gravity * Math.Pow(airTime, 2.0)) + (initialY - hitbox.topEdge);
        }
    }
}
