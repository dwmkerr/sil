using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AcceptanceModel
{
    public class ClassWithNestedClass
    {
        public void DoSomething()
        {
            

        }

        //  TODO test with acceptance model
        public class ClassNestedInClass
        {
            //  TODO test with acceptance model
            public class DeepNestedClassInClassNestedInClass
            {
                //  TODO test with acceptance model.
                public struct VeryDeepNestedStructInDeepNestedClassInClassNestedInClass
                {
                    //  TODO test with acceptance model.
                    public delegate void DelegateInVeryDeepNestedStructInDeepNestedClassInClassNestedInClass(string stringParameter);
                }
            }
        }
    }
}
