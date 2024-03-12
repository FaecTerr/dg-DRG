using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.DeepDuckGalactic
{
    [EditorGroup("Faecterr's|Guns|Grenades")]
    public class Lure : Gun
    {
        bool pin;
        float delay = 1.2f;
        float stayTime = 10 * 60;
        int frameDelay;
        float range = 32;

        SpriteMap _LURE; 
        Sprite _halo;
        public Lure(float x, float y) : base(x, y)
        {
            _halo = new Sprite("halo", 0f, 0f);
            _halo.CenterOrigin();

            ammo = 1;
            graphic = new Sprite(GetPath("sprites/grenades/Lure.png"));
            _LURE = new SpriteMap(GetPath("sprites/grenades/lureDance.png"), 25, 27);
            _LURE.center = new Vec2(12.5f, 13.5f);
            center = new Vec2(6, 6);

            collisionSize = new Vec2(10, 10);
            collisionOffset = new Vec2(-5, -5);
        }

        public override void Update()
        {
            base.Update();
            if(prevOwner != null && owner == null && !pin)
            {
                pin = true;
                SFX.Play(GetPath("sfx/grenades/GrenadeLure.wav"));
            }
            if (pin)
            {
                canPickUp = false;
                if (delay > 0f)
                {
                    delay -= 0.1f;
                }
                else
                {
                    if (stayTime > 0)
                    {
                        stayTime--;
                        foreach(Duck d in Level.CheckCircleAll<Duck>(position, range))
                        {
                            if(d != prevOwner && owner == null)
                            {
                                d.listening = true;
                                d.listenTime = 5;
                                stayTime--;
                            }
                        }
                    }
                    else
                    {
                        SFX.Play(GetPath("sfx/grenades/LureOutOfPower.wav"));
                        Level.Remove(this);
                    }
                }
            }
        }

        public override void Draw()
        {
            base.Draw();
            if (pin && delay <= 0)
            {
                _LURE.scale = new Vec2(1.5f, 1.5f);
                Graphics.Draw(_LURE, position.x, position.y - 20 - Rando.Float(-1, 1));
                if (frameDelay > 0)
                {
                    frameDelay--;
                }
                else
                {
                    if (_LURE.frame < 7)
                    {
                        _LURE.frame++;
                        _LURE.alpha = Rando.Float(0.7f, 0.9f);
                    }
                    else
                    {
                        _LURE.frame = 0;
                    }
                    frameDelay = 3;
                }
            }
        }

        public override void Fire()
        {
            if (!pin)
            {
                pin = true;
                SFX.Play(GetPath("sfx/grenades/GrenadeLure.wav"));
            }
        }
    }
}
