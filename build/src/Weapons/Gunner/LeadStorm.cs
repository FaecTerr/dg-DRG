using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.DeepDuckGalactic
{
    public class LeadStorm : Gun
    {
        SpriteMap _sprite;


        bool isFiring;
        bool overheated;
        float overheat;
        float decay;
        float spinning;
        float maxAccuracy;
        float accuracyDelay;

        float maxAccuracyDuration = 0.5f;
        float maxAccuracyDrain = 3.5f;
        float CoolingRate = 1.5f;
        float SpinupTime = 0.7f;
        float SpindownTime = 2f;
        float overheatDuration = 10;
        float CoolingDelay = 0.3f;
        public LeadStorm(float x, float y) : base(x, y)
        {
            _ammoType = new AT9mm();
            _ammoType.range = 170f;
            _ammoType.accuracy = 0.5f;
            wideBarrel = true;
            barrelInsertOffset = new Vec2(0f, 0f);
            _type = "gun";
            _sprite = new SpriteMap("chaingun", 42, 28, false);
            graphic = _sprite;
            center = new Vec2(14f, 14f);
            collisionOffset = new Vec2(-8f, -3f);
            collisionSize = new Vec2(24f, 10f);
            _barrelOffsetTL = new Vec2(39f, 14f);
            _fireSound = "pistolFire";
            _fullAuto = true;
            _fireWait = 0.45f;
            _kickForce = 1f;
            _fireRumble = RumbleIntensity.Kick;
            weight = 8f;
            _holdOffset = new Vec2(0f, 2f);
            editorTooltip = "Like a chaingun, but for adults. Fires mean pointy metal things.";
        }
        public override void Update()
        {
            base.Update();
            if (isFiring || overheated)
            {
                if (spinning >= 60 * SpinupTime)
                {
                    weight = 8;
                    overheat += 0.0166666f;
                    _ammoType.accuracy = 0.5f + maxAccuracy * 0.5f;
                    Fire();
                    decay = CoolingDelay * 60;

                    accuracyDelay = maxAccuracyDuration * 60;
                    if(maxAccuracy < 1)
                    {
                        maxAccuracy += 0.1f;
                    }
                    else
                    {
                        maxAccuracy = 1;
                    }

                    if (overheat >= overheatDuration)
                    {
                        overheated = true;
                    }
                }
                else
                {
                    spinning++;
                }
            }
            else
            {
                if (accuracyDelay > 0)
                {
                    accuracyDelay--;
                }
                else
                {
                    if (maxAccuracy > 0)
                    {
                        maxAccuracy -= 1 / maxAccuracyDrain;
                    }
                    else
                    {
                        maxAccuracy = 0;
                    }
                }

                weight = 2;
                if (decay > 0)
                {
                    decay--;
                }
                else
                {
                    if (overheat > 0)
                    {
                        float cooldownSpeed = CoolingRate;
                        if (overheated)
                        {
                            cooldownSpeed = 1;
                        }
                        overheat -= cooldownSpeed * 0.0166666f;
                    }
                    else
                    {
                        overheat = 0;
                        overheated = false;
                    }
                }
                if (spinning > 0)
                {
                    spinning -= SpinupTime / SpindownTime;
                }
            }
            ammo = 99;
        }
        public override void OnPressAction()
        {
            base.OnPressAction();
            isFiring = true;
        }
        public override void OnReleaseAction()
        {
            base.OnReleaseAction();
            isFiring = false;
        }
    }
}
