using LMS.Application.CustomAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Test.CustomAttributeValidations
{
    public class RoleValidationAttributeTests
    {
        private RoleValidationAttribute attribute;

        [SetUp]
        public void Setup()
        {
            attribute = new RoleValidationAttribute();
        }

        [Test]
        public void RoleValidation_Returns_True()
        {
            //Assert
            var role = "Instructor";

            // Act
            bool isValid = attribute.IsValid(role);

            // Assert
            Assert.AreEqual(true, isValid);
        }

        [Test]
        public void RoleValidation_Returns_False()
        {
            //Assert
            var role = "";

            // Act
            bool isValid = attribute.IsValid(role);

            // Assert
            Assert.AreEqual(false, isValid);
        }

    }
}