using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.DeepDuckGalactic
{
    [EditorGroup("Faecterr's|Guns|Grenades")]
    public class ImpactAxe : Gun
    {
        float hold;
        float swing;
        bool stuck;
        float bangle;

        bool swinging;

        float swingTarget = -0.4f;
        public override float angle
        {
            get
            {
                return base.angle + (swing + hold) * offDir;
            }
            set
            {
                _angle = value;
            }
        }

        public ImpactAxe(float x, float y) : base(x, y)
        {
            ammo = 1;
            graphic = new Sprite(GetPath("sprites/grenades/Axe.png"));
            center = new Vec2(14, 32);

            collisionSize = new Vec2(26, 32);
            collisionOffset = new Vec2(-13, -19 - 12);
            _barrelOffsetTL = new Vec2(14f, 1f);
            _holdOffset = new Vec2(-2, 1);
        }
        public virtual Vec2 barrelStartPos
        {
            get
            {
                if (owner == null)
                {
                    return position - (Offset(barrelOffset) - position).normalized * 6f;
                }
                return position + (Offset(barrelOffset) - position).normalized * 2f;
            }
        }

        public override void Update()
        {
            if (stuck)
            {
                base.angle = bangle;
            }
            base.Update();
            if(owner != null)
            {
                enablePhysics = true;
                hold = Maths.LerpTowards(hold, swingTarget, 0.4f);
                swing = 0;
                stuck = false;
                if (swinging)
                {
                    if (hold >= 2)
                    {
                        if(duck != null && duck.grounded)
                        {
                            for (int i = 0; i < 12; i++)
                            {
                                Spark spark = Spark.New(position.x, position.y, barrelStartPos, 0.01f);
                                spark.hSpeed = Rando.Float(-2, 2);
                                spark.vSpeed = Rando.Float(-4, -2);

                                Level.Add(spark);
                            }
                        }
                        swinging = false;
                        swingTarget = -0.4f;
                    }
                    IEnumerable<IAmADuck> hit = Level.CheckLineAll<IAmADuck>(barrelStartPos, barrelPosition); 
                    using (IEnumerator<IAmADuck> enumerator = hit.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            IAmADuck d4 = enumerator.Current;
                            if (d4 != duck && d4 != prevOwner)
                            {
                                MaterialThing realThing4 = d4 as MaterialThing;
                                if (realThing4 != null)
                                {
                                    Duck realDuck = realThing4 as Duck;
                                    if (realDuck != null && !realDuck.destroyed && duck != null)
                                    {
                                        RumbleManager.AddRumbleEvent(base.duck.profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Pulse, RumbleFalloff.Short, RumbleType.Gameplay));
                                        StatBinding swordKills = Global.data.swordKills;
                                        int valueInt = swordKills.valueInt;
                                        swordKills.valueInt = valueInt + 1;
                                    }
                                    realThing4.Destroy(new DTImpale(this));
                                }
                            }
                        }
                        return;
                    }
                }
            }
            else
            {
                hold = 0;
            }
            if (canPickUp == false)
            {
                alpha = 0.4f;
            }
        }

        public override void OnSolidImpact(MaterialThing with, ImpactedFrom from)
        {
            if(with != null)
            {
                if(from == ImpactedFrom.Right)
                {
                    enablePhysics = false;
                    swing = 0.5f;
                }
                if(from == ImpactedFrom.Left)
                {
                    enablePhysics = false;
                    swing = 0.5f;
                }
                SFX.Play(GetPath("sfx/grenades/AxeGrenadeDullImpact.wav"));
                stuck = true;
                bangle = base.angle;
            }
            base.OnSolidImpact(with, from);
        }
        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if(with != null)
            {
                if(owner == null && with != prevOwner && (Math.Abs(hSpeed) + Math.Abs(vSpeed)) > 2f)
                {
                    if(with is Duck)
                    {
                        SFX.Play(GetPath("sfx/grenades/AxeGrenade.wav"));
                        (with as Duck).Kill(new DTCrush(this));
                        canPickUp = false;
                    }
                }
            }
            base.OnSoftImpact(with, from);
        }

        public override void Draw()
        {
            base.Draw();
            if(owner != null)
            {
                //Graphics.DrawLine(barrelStartPos, barrelPosition, Color.White, 1, depth + 1);
            }
        }

        public override void Fire()
        {

        }

        public override void OnPressAction()
        {
            base.OnPressAction();
            if (hold <= -0.4f)
            {
                SFX.Play(GetPath("sfx/grenades/AxeGrenadeFoldOut.wav"), 0.8f);
                swingTarget = 2.0f;
                swinging = true;
            }
        }
    }
}
