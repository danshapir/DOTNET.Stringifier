using Stringify.Attributes;
using System.Collections.Generic;

namespace StringifyUnitTest
{
	public class DummyClass
    {
        [HideLog]
        public string Hidden { get; set; }

        [Mask]
        public string CCNumber { get; set; }

        public string Regular { get; set; }

        public DummyInnerClass Inner { get; set; }
    }

    public class DummyInnerClass
    {
        public string InnerText { get; set; }
    }

    public class DummyClassPerson
    {
        public string Name { get; set; }

        public int Age { get; set; }
    }

    public class DummyClassPersonsList
    {
        public IList<DummyClassPerson> Persons { get; set; }

        public string OneMoreProperty { get; set; }
    }

    public class DummyClassPersonsDictionary
    {
        public IDictionary<int, DummyClassPerson> Persons { get; set; }

        public string OneMoreProperty { get; set; }
    }

    public class DummyClassOnlyWithHidden
    {
        [HideLog]
        public string Hidden { get; set; }
    }
}