using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iSelectManager.Tests.DialerAPI
{
    [TestClass]
    public class SkillSetTest
    {
        [TestMethod]
        public void CanFindWhenExists()
        {
            Models.Skill skill = Models.Skill.find("English");
            Models.SkillSet skillset = Models.SkillSet.find_or_create(skill.DisplayName, "SKILL0", 76);

            skillset.add_skill(skill, "MYVALUE_FOR_SKILL0", 12);

        }
    }
}
