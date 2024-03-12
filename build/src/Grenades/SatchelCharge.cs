using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.DeepDuckGalactic
{
    public class SatchelCharge : Gun
    {
        bool thrown;
        public SatchelCharge(float x, float y) : base(x, y)
        {

        }
    }

    public class Satchel : PhysicsObject 
    { 

    }
}
