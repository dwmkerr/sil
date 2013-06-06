using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AcceptanceModel
{
    public class ClassWithEvents
    {
        public void FireOnSomething()
        {
            var theEvent = OnSomething;
            if (theEvent != null)
                theEvent();
        }

        public event Action OnSomething;
    }
}
