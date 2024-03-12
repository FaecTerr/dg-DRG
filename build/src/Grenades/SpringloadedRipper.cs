using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.DeepDuckGalactic
{
    [EditorGroup("Faecterr's|Guns|Grenades")]
    public class SpringloadedRipper : Gun
    {
        public SpringloadedRipper(float x, float y) : base(x, y)
        {
            ammo = 3;
            graphic = new SpriteMap(GetPath("sprites/grenades/SpringloadedRippers.png"), 28, 16);
            center = new Vec2(14, 8);

            collisionSize = new Vec2(20, 14);
            collisionOffset = new Vec2(-10, -7);
        }

        public override void Draw()
        {
            base.Draw();
            //Graphics.DrawLine(position, position + new Vec2((float)Math.Cos(0), (float)Math.Sin(0)) * 20, Color.Blue);
            //Graphics.DrawLine(position, position + new Vec2((float)Math.Cos(Math.PI * 0.5f), (float)Math.Sin(Math.PI * 0.5f)) * 20, Color.Red);
        }

        public override void Fire()
        {
            if (ammo > 0)
            {
                SFX.Play(GetPath("sfx/grenades/WallSawGrenadeActivate.wav"));
                Level.Add(new Rippers(position.x + 7 * offDir, position.y - 3) { hSpeed = 3 * offDir, vSpeed = -1, offDir = (sbyte)offDir, _prevOwner = owner });
                ammo -= 1;
            }
        }
    }

    public class Rippers : PhysicsObject
    {
        Sprite sprite;
        Vec2 dir;
        float speed = 4;

        Vec2 prevPosition;
        Vec2 prevPrevPosition;

        int failsafes;
        float lockdown;
        int stuck;
        Thing stickedTo;

        float angled;
        float angleDir;
        sbyte rememberedOffDir;
        float rotationDir;
        public Rippers(float x, float y) : base(x, y)
        {
            sprite = new Sprite(GetPath("sprites/grenades/Rippers.png"));
            sprite.center = new Vec2(7.5f, 7.5f);
            graphic = sprite;
            center = new Vec2(7.5f, 7.5f);

            collisionSize = new Vec2(13, 13);
            collisionOffset = new Vec2(-6.5f, -6.5f);
            friction = 0;

            weight = 0;
            thickness = 0;

            prevPosition = position;
        }

        public bool isEmpty()
        {
            Vec2 sizeMove = new Vec2((float)Math.Cos(angleDir - (float)Math.PI * 0.25f * offDir * rotationDir), (float)Math.Sin(angleDir - (float)Math.PI * 0.25f * offDir * rotationDir));
            for (float i = 0; i < collisionSize.x; i++)
            {
                for (float j = 0; j < collisionSize.y; j++)
                {
                }
            }
            if(Level.CheckRect<Block>(topLeft + sizeMove, bottomRight + sizeMove) != null)
            {
                return false;
            }
            return true;
        }

        public override void Update()
        {
            angled += 0.16f * offDir * speed;
            sprite.angle = angled;
            angle = angled;
            _skipPlatforms = true;
            base.Update();
            if (grounded)
            {
                if (dir == new Vec2(0, 0))
                {
                    dir = new Vec2(offDir, 0);
                    stickedTo = Level.CheckPoint<Block>(position.x, position.y + 10);
                    angleDir = (float)Math.PI * 0.25f * offDir;
                    rememberedOffDir = offDir;
                }
            }
            if (dir != new Vec2(0, 0))
            {
                Spark spark = Spark.New(position.x, position.y, new Vec2(0, 0), 0.01f);
                spark.hSpeed = Rando.Float(0.2f, 1) * -hSpeed - 0.5f * Math.Abs(dir.y) * speed;
                spark.vSpeed = Rando.Float(0.4f, 1f) * -vSpeed - 0.5f * Math.Abs(dir.x) * speed;
                Level.Add(spark);

                foreach (Platform platform in Level.current.things[typeof(Platform)])
                {
                    if(!clip.Contains(platform))
                    clip.Add(platform);
                }
                _skipPlatforms = true;
                offDir = rememberedOffDir;
                dir.x = (float)Math.Cos(angleDir);
                dir.y = (float)Math.Sin(angleDir);

                hSpeed = dir.x * speed;
                vSpeed = dir.y * speed;
                if(Math.Abs(hSpeed) > speed)
                {
                    hSpeed = speed * Math.Sign(hSpeed);
                }
                if (Math.Abs(vSpeed) > speed)
                {
                    vSpeed = speed * Math.Sign(vSpeed);
                }

                if (isEmpty() && lockdown <= 0)
                { 
                    angleDir -= (float)Math.PI * 0.25f * offDir * rotationDir;
                    failsafes++;
                }

                if(failsafes >= 2)
                {
                    lockdown = 6;
                    failsafes = 0;
                }
                if(lockdown > 0)
                {
                    lockdown--;
                }


                if (position == prevPosition)
                {
                    stuck++;
                    if (stuck >= 2)
                    {
                        angleDir += (float)Math.PI * 0.25f * offDir * rotationDir;
                        stuck = 0;
                        lockdown = 6;
                    }
                }
                foreach(Duck d in Level.CheckLineAll<Duck>(position, prevPosition))
                {
                    d.Kill(new DTImpale(this));
                }
                foreach(Holdable h in Level.CheckLineAll<Holdable>(position, prevPosition))
                {
                    h.Hurt(1f);
                }
                    prevPrevPosition = prevPosition;
                prevPosition = position;
            }
        }

        public override void SolidImpact(MaterialThing with, ImpactedFrom from)
        {
            base.SolidImpact(with, from);
            if(with is Block)
            {
                if(dir == new Vec2(0, 0))
                {
                    SFX.Play(GetPath("sfx/grenades/WallSawGrenadeRunning.wav"), 0.4f, 0, 0, true);
                    if (from == ImpactedFrom.Left)
                    {
                        dir.y = -1;
                        angleDir = (float)Math.PI * 0.25f * Math.Sign(vSpeed) * offDir + (float)Math.PI;
                        if ((Math.Sign(vSpeed) > 0 && offDir > 0) || (Math.Sign(vSpeed) < 0 && offDir < 0))
                        {
                            rotationDir = -1f;
                        }
                        else
                        {
                            rotationDir = 1f;
                        }
                    }
                    if (from == ImpactedFrom.Right)
                    {
                        dir.y = 1;
                        angleDir = (float)Math.PI * 0.25f * Math.Sign(vSpeed) * offDir; 
                        if ((Math.Sign(vSpeed) > 0 && offDir > 0) || (Math.Sign(vSpeed) < 0 && offDir < 0))
                        {
                            rotationDir = -1f;
                        }
                        else
                        {
                            rotationDir = 1f;
                        }
                    }
                    if(from == ImpactedFrom.Top)
                    {
                        dir.x = offDir;
                        angleDir = (float)Math.PI * 0.25f * offDir + (float)Math.PI * 1.5f;
                        if(offDir > 0)
                        {
                            rotationDir = 1;
                        }
                        else
                        {
                            rotationDir = -1;
                        }
                    }
                    if(from == ImpactedFrom.Bottom)
                    {
                        dir.x = -offDir;
                        angleDir = (float)Math.PI * 0.25f * -offDir + (float)Math.PI * 0.5f; 
                        if (offDir > 0)
                        {
                            rotationDir = -1;
                        }
                        else
                        {
                            rotationDir = -1;
                        }
                    }
                    stickedTo = with;
                    gravMultiplier = 0;
                    rememberedOffDir = offDir;
                }
            }
        }

        public override void Draw()
        {
            base.Draw();
            Vec2 changes = position - prevPrevPosition;
            Vec2 offsetPower = new Vec2(0.2f, 0.2f);
            if(changes.x != 0)
            {
                offsetPower.x = 1;
            }
            if(changes.y != 0)
            {
                offsetPower.y = 1;
            }
            Graphics.Draw(sprite, position.x - dir.x * 3 * offsetPower.x, position.y - dir.y * 3 * offsetPower.x, depth - 1);
            Graphics.Draw(sprite, position.x - dir.x * 6 * offsetPower.x, position.y - dir.y * 6 * offsetPower.x, depth - 2);

            //Graphics.DrawLine(position, position + new Vec2((float)Math.Cos(angleDir) * 40, (float)Math.Sin(angleDir) * 40), Color.White);
            //Graphics.DrawLine(position, position + new Vec2((float)Math.Cos(angleDir + (float)Math.PI * 0.25f * offDir) * 40, (float)Math.Sin(angleDir + (float)Math.PI * 0.25f * offDir) * 40), Color.Red);
            //Graphics.DrawLine(position, position + new Vec2((float)Math.Cos(angleDir - (float)Math.PI * 0.25f * offDir) * 40, (float)Math.Sin(angleDir - (float)Math.PI * 0.25f * offDir) * 40), Color.Blue);
        }
    }
}
