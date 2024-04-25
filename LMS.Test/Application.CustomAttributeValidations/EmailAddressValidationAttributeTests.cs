using LMS.Application.CustomAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Test.CustomAttributeValidations
{
    public class EmailAddressValidationAttributeTests
    {
        private EmailAddressValidationAttribute attribute;

        [SetUp]
        public void Setup()
        {
            attribute = new EmailAddressValidationAttribute();
        }

        [Test]
        public void IsValid_Returns_True()
        {
            var email = "user+test@example.com";

            // Act
            bool isValid = attribute.IsValid(email);

            // Assert
            Assert.AreEqual(true, isValid);
        }

        [Test]
        public void IsValid_Returns_False()
        {
            var email = "";

            // Act
            bool isValid = attribute.IsValid(email);

            // Assert
            Assert.AreEqual(false, isValid);
        }
    }
}
