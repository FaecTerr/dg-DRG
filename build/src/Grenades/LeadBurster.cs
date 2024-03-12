using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.DeepDuckGalactic
{
    [EditorGroup("Faecterr's|Guns|Grenades")]
    public class LeadBurster : Gun
    {
        float delay = 2.8f;
        int sprays = 3;
        int shots = 18;
        bool sticked;
        float angleDegr;
        float yOffset;
        public LeadBurster(float x, float y) : base(x, y)
        {
            ammo = 1;
            graphic = new SpriteMap(GetPath("sprites/grenades/leadBurster.png"), 12, 18);
            center = new Vec2(6, 6);

            collisionSize = new Vec2(10, 10);
            collisionOffset = new Vec2(-5, -5);
        }

        public override void Update()
        {
            base.Update();
            if (!sticked)
            {
                if (grounded && prevOwner != null)
                {
                    sticked = true;
                    angleDegrees = 0;
                    angleDegr = angleDegrees;
                    enablePhysics = false;
                }
            }
            else
            {
                angleDegrees = angleDegr;
                if (delay > 0)
                {
                    delay -= 0.1f;
                }
                else
                {
                    canPickUp = false;
                    float dir = ((float)angleDegrees + 10 * shots - 180 + Rando.Float(5f));
                    ATShrapnel shrap = new ATShrapnel();
                    shrap.range = 240f + Rando.Float(18f);
                    Bullet bullet = new Bullet(position.x + (float)(Math.Cos(Maths.DegToRad(dir)) * 6.0), position.y + yOffset - (float)(Math.Sin((double)Maths.DegToRad(dir)) * 6.0), shrap, -dir, null, false, -1f, false, true);
                    bullet.firedFrom = this;
                    firedBullets.Add(bullet);
                    Level.Add(bullet);

                    if (shots % 2 == 0)
                    {
                        SFX.Play(GetPath("sfx/grenades/GrenadeNeedSprayer.wav"), 0.8f);
                    }

                    if (Network.isActive)
                    {
                        Send.Message(new NMFireGun(this, firedBullets, bulletFireIndex, false, 4, false), NetMessagePriority.ReliableOrdered);
                        firedBullets.Clear();
                    }
                    shots--;
                    if (shots <= 0)
                    {
                        if (sprays > 0)
                        {
                            shots = 18;
                            delay = 1.2f;
                            sprays--;
                        }
                        else
                        {
                            Level.Remove(this);
                        }
                    }
                }
            }
        }
        public override void OnSolidImpact(MaterialThing with, ImpactedFrom from)
        {
            if (with != null && !sticked && prevOwner != null)
            {
                sticked = true;
                if(from == ImpactedFrom.Left)
                {
                    angleDegrees = 90;
                }
                if (from == ImpactedFrom.Right)
                {
                    angleDegrees = -90;
                }
                if (from == ImpactedFrom.Bottom)
                {
                    angleDegrees = 0;
                    yOffset = -4f;
                }
                if (from == ImpactedFrom.Top)
                {
                    angleDegrees = 180;
                    yOffset = 1;
                }
                enablePhysics = false;
                hSpeed = 0;
                vSpeed = 0;
                angleDegr = angleDegrees;
            }
            base.OnSolidImpact(with, from);
        }
        public override void Fire()
        {

        }
    }
}
