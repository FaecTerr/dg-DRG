using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.DeepDuckGalactic
{
    [EditorGroup("Faecterr's|Guns|Grenades")]
    public class CryoGrenade : Gun
    {
        public bool pin;
        float radius = 48;
        float delay = 0.15f;
        public CryoGrenade(float x, float y) : base(x, y)
        {
            ammo = 1;
            graphic = new SpriteMap(GetPath("sprites/grenades/CryoGrenade.png"), 12, 12);
            center = new Vec2(6, 6);

            collisionSize = new Vec2(10, 10);
            collisionOffset = new Vec2(-5, -5);

            bouncy = 0.7f;
        }

        public override void Update()
        {
            base.Update();
            if (!pin)
            {
                if (grounded)
                {
                    Activation();
                }
            }
            else
            {
                if (delay > 0)
                {
                    delay -= 0.01f;
                }
                else
                {
                    Trigger();
                }
            }
        }

        public void Trigger()
        {
            SFX.Play(GetPath("sfx/grenades/FreezeGrenade.wav"));
            foreach (Holdable h in Level.CheckCircleAll<Holdable>(position, radius))
            {
                if (h != this && !(h is IceBlock))
                {
                    h.onFire = false;
                    h.heat = -1;
                    IceBlock ice = new IceBlock(h.position.x, h.position.y);
                    ice.contains = h.GetType();
                    Level.Remove(h);
                    Level.Add(ice);
                }
            }
            for (int i = 0; i < 72; i++)
            {
                float range = Rando.Float(radius * 0.2f, radius);
                float angle = 360 / 36 * i;
                SnowFallParticle snowParticle = new SnowFallParticle(position.x + (float)Math.Cos(angle) * range, position.y + (float)Math.Sin(angle) * range, 
                    new Vec2(Rando.Float(-1, 1), Rando.Float(-1, 1)));
                Level.Add(snowParticle);

                BreathSmoke smoke = BreathSmoke.New(position.x + (float)Math.Cos(angle) * range, position.y + (float)Math.Sin(angle) * range);
                Level.Add(smoke);
            }
            foreach (Duck d in Level.CheckCircleAll<Duck>(position, radius))
            {
                d.heat = -1;
                d.onFire = false;
            }

            Level.Remove(this);
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            base.OnSoftImpact(with, from);
        }

        public void Activation()
        {
            if (prevOwner != null)
            {
                pin = true;
                frame = 1;
            }
        }

        public override void Draw()
        {
            base.Draw();
        }
        public override void Fire()
        {

        }
    }
}
