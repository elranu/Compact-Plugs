using System;
using System.Collections.Generic;
using System.Text;

namespace TestInjection
{
    public class DemoPresenter
    {

        private DemoPresenter()
        {

        }
        private ISomethingToInject _iSomething;
        private int _Inte;
        private string _str;
        private Object2 _obj2;

        public Object2 Obj2
        {
            get { return _obj2; }
            set { _obj2 = value; }
        }
	

        public string Str
        {
            get { return _str; }
            set { _str = value; }
        }
	

        public int Inte
        {
            get { return _Inte; }
            set { _Inte = value; }
        }
	

        public ISomethingToInject Something
        {
            get { return _iSomething; }
            set { _iSomething = value; }
        }

        public override string ToString()
        {
            return _iSomething.DoSomething();
        }

        public List<string> Lista { get; set; }
        
        public Dictionary<Int32,string> Dict { get; set; }



	
    
    }
}
