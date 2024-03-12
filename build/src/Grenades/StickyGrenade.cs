using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.DeepDuckGalactic
{
    [EditorGroup("Faecterr's|Guns|Grenades")]
    public class StickyGrenade : Gun
    {
        private bool _explosionCreated;
        public bool _pin = true;
        public StateBinding _pinBinding = new StateBinding("_pin", -1, false, false);
        Vec2 anchorOffset;
        float angld;
        public float _timer = 1.2f;
        public StateBinding _timerBinding = new StateBinding("_timer", -1, false, false);
        public StickyGrenade(float xval, float yval) : base(xval, yval)
        {
            ammo = 1;
            graphic = new SpriteMap(GetPath("sprites/grenades/stickyGrenade.png"), 12, 12);
            center = new Vec2(6, 6);

            collisionSize = new Vec2(10, 10);
            collisionOffset = new Vec2(-5, -5);
        }

        public override void OnPressAction()
        {
            if (_pin)
            {
                _pin = false;
                Level.Add(new GrenadePin(x, y)
                {
                    hSpeed = (float)(-(float)offDir) * (1.5f + Rando.Float(0.5f)),
                    vSpeed = -2f
                });
                if (base.duck != null)
                {
                    RumbleManager.AddRumbleEvent(base.duck.profile, new RumbleEvent(this._fireRumble, RumbleDuration.Pulse, RumbleFalloff.None, RumbleType.Gameplay));
                }
                SFX.Play("pullPin", 1f, 0f, 0f, false);
            }
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if(with != null && !(with is Platform) && !(with is AutoPlatform) && !_pin && anchor == null)
            {
                anchor = with;
                anchorOffset = position - with.position;
                angld = angle;
                canPickUp = false;
                frame = 1;
            }
            base.OnSoftImpact(with, from);
        }
        public override void Update()
        {
            base.Update();
            if (anchor != null)
            {
                position = anchor.position + anchorOffset;
                angle = angld;
            }
            if (!_pin)
            {
                frame = 1;
                _timer -= 0.01f;

                if(_timer <= 0)
                {
                    SFX.Play(GetPath("sfx/grenades/StickyGrenadeExplosion.wav"));
                    CreateExplosion(position);
                    Level.Remove(this);
                }
            }
        }
        public void CreateExplosion(Vec2 pos)
        {
            if (!this._explosionCreated)
            {
                float cx = pos.x;
                float cy = pos.y - 2f;
                Level.Add(new ExplosionPart(cx, cy, true));
                int num = 6;
                if (Graphics.effectsLevel < 2)
                {
                    num = 3;
                }
                for (int i = 0; i < num; i++)
                {
                    float dir = (float)i * 60f + Rando.Float(-10f, 10f);
                    float dist = Rando.Float(12f, 20f);
                    Level.Add(new ExplosionPart(cx + (float)(Math.Cos((double)Maths.DegToRad(dir)) * (double)dist), cy - (float)(Math.Sin((double)Maths.DegToRad(dir)) * (double)dist), true));
                }
                for (int i = 0; i < 20; i++)
                {
                    float dir = (float)i * 18f - 5f + Rando.Float(10f);
                    ATShrapnel shrap = new ATShrapnel();
                    shrap.range = 60f + Rando.Float(18f);
                    Bullet bullet = new Bullet(cx + (float)(Math.Cos((double)Maths.DegToRad(dir)) * 6.0), cy - (float)(Math.Sin((double)Maths.DegToRad(dir)) * 6.0), shrap, dir, null, false, -1f, false, true);
                    bullet.firedFrom = this;
                    this.firedBullets.Add(bullet);
                    Level.Add(bullet);
                }
                _explosionCreated = true;
                //SFX.Play("explode", 1f, 0f, 0f, false);
                RumbleManager.AddRumbleEvent(pos, new RumbleEvent(RumbleIntensity.Heavy, RumbleDuration.Short, RumbleFalloff.Medium, RumbleType.Gameplay));
            }
        }
    }
}
