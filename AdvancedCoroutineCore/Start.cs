using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdvancedCoroutines.Core;

namespace AdvancedCoroutineCore
{
    class Start
    {
        static void Main()
        {
            Test t = new Test();
        }

        public class Test
        {
            public Test()
            {
                var _dll = new AdvancedCoroutinesCoreDll(null);
                var r = _dll.StartCoroutine(enumer(), this);
                _dll.StopCoroutine(r);
            }

            private IEnumerator enumer()
            {
                yield break;
            }
        }
    }
}
