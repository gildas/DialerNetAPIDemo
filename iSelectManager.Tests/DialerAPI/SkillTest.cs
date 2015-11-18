using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iSelectManager.Tests.DialerAPI
{
    [TestClass]
    public class SkillTest
    {
        [TestMethod]
        public void CanFindWhenExists()
        {
            Models.Skill skill = Models.Skill.find_or_create("English");
        }
    }
}
