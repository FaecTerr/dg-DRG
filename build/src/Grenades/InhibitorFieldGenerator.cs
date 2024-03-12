using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.DeepDuckGalactic
{
    [EditorGroup("Faecterr's|Guns|Grenades")]
    public class InhibitorFieldGenerator : Gun
    {
        public float slowdown = 0.6f;
        public bool fieldActive;
        float radius = 48;
        float stayTime = 15 * 60;
        public InhibitorFieldGenerator(float x, float y) : base(x, y)
        {
            ammo = 1;
            graphic = new Sprite(GetPath("sprites/grenades/IFD.png"));
            center = new Vec2(6, 6);

            collisionSize = new Vec2(10, 10);
            collisionOffset = new Vec2(-5, -5);
        }

        public override void Update()
        {
            base.Update();
            if (!fieldActive)
            {
                if (grounded)
                {
                    Activation();
                }
            }
            else
            {
                hSpeed = 0;
                vSpeed = 0;
                canPickUp = false;
                enablePhysics = false;
                foreach(Duck d in Level.CheckCircleAll<Duck>(position, radius))
                {
                    d.hSpeed *= slowdown;
                }
                foreach(Holdable h in Level.CheckCircleAll<Holdable>(position, radius))
                {
                    if (!(h is IAmADuck))
                    {
                        h.hSpeed /= slowdown;
                    }
                }
                if (stayTime > 0)
                {
                    stayTime--;
                }
                else 
                {
                    Level.Remove(this);
                }
            }
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            base.OnSoftImpact(with, from);
            if (!(with is Platform))
            {
                Activation();
            }
        }

        public void Activation()
        {
            if (prevOwner != null && !fieldActive)
            {
                SFX.Play(GetPath("sfx/grenades/DamageEnhancer.wav"));
                fieldActive = true;
            }
        }

        public override void Draw()
        {
            base.Draw();
            if (fieldActive)
            {
                Graphics.DrawCircle(position, radius, Color.Aqua, 3, depth, 32);
                float randAngle = Rando.Float((float)Math.PI * 2);
                Vec2 offset = new Vec2((float)Math.Cos(randAngle), (float)Math.Sin(randAngle));
                Graphics.DrawLine(position, position + offset * radius, Color.Aqua, 0.4f);

                foreach (Duck d in Level.CheckCircleAll<Duck>(position, radius))
                {
                    randAngle = Rando.Float((float)Math.PI * 2);
                    offset = new Vec2((float)Math.Cos(randAngle), (float)Math.Sin(randAngle)); 
                    Graphics.DrawLine(position + offset * radius, d.position, Color.Aqua, 0.4f);
                    Graphics.DrawLine(position, d.position, Color.Aqua, 0.4f);
                }
                foreach (Holdable h in Level.CheckCircleAll<Holdable>(position, radius))
                {
                    randAngle = Rando.Float((float)Math.PI * 2);
                    offset = new Vec2((float)Math.Cos(randAngle), (float)Math.Sin(randAngle));
                    Graphics.DrawLine(position + offset * radius, h.position, Color.Aqua, 0.4f);
                    Graphics.DrawLine(position, h.position, Color.Aqua, 0.4f);
                }
            }
        }

        public override void Fire()
        {

        }
    }
}
