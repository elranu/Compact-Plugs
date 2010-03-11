using System;
using System.Collections.Generic;
using System.Text;

namespace TestInjection
{
    public class BehaviourISomething : ISomethingToInject
    {

        #region ISomethingToInject Members

        public string DoSomething()
        {
            return "I did something";
        }

        #endregion
    }
}
