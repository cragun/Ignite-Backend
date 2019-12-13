using System;
using System.Collections.Generic;
using System.Linq;
using DataReef.Core.Infrastructure.Repository;
using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using DataReef.TM.Models.DTOs.EPC;
using DataReef.TM.Models.Enums;
using DataReef.TM.Services.Services;
using DataReef.TM.Services.Tests.Builders;
using Moq;
using NUnit.Framework;

namespace DataReef.TM.Services.Tests
{
    [TestFixture]
    public class ActionItemServiceTests
    {
        private Mock<ILogger> _loggerMock;
        private Mock<IOUService> _ouServiceMock;
        private Mock<IPropertyService> _propertyServiceMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;

        [SetUp]
        public void Init()
        {
            _loggerMock = new Mock<ILogger>();
            _ouServiceMock = new Mock<IOUService>();
            _propertyServiceMock = new Mock<IPropertyService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
        }

        #region UploadActionItems

        [Test]
        [Category("UploadActionItems")]
        public void UploadActionItems_InvalidOuid_Throws()
        {
            //  arrange
            var service = new ActionItemService(_loggerMock.Object, () => _ouServiceMock.Object,
                () => _propertyServiceMock.Object, () => _unitOfWorkMock.Object);

            //  act, assert
            var exception =
                Assert.Throws<ArgumentException>(
                    () => service.UploadActionItems(Guid.Empty, new List<ActionItemInput>()));

            //  assert
            Assert.AreEqual("ouid", exception.Message);
        }

        [Test]
        [Category("UploadActionItems")]
        public void UploadActionItems_InvalidActionItems_Throws()
        {
            //  arrange
            var service = new ActionItemService(_loggerMock.Object, () => _ouServiceMock.Object,
                () => _propertyServiceMock.Object, () => _unitOfWorkMock.Object);

            //  act, assert
            var exception = Assert.Throws<ArgumentException>(() => service.UploadActionItems(Guid.NewGuid(), null));

            //  assert
            Assert.AreEqual("actionItems", exception.Message);
        }

        [Test]
        [Category("UploadActionItems")]
        public void UploadActionItems_InvalidPropertyTerritory_Throws()
        {
            //  arrange
            _propertyServiceMock.Setup(
                ps =>
                    ps.GetMany(It.IsAny<IEnumerable<Guid>>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                        It.IsAny<bool>()))
                .Returns(new List<Property>
                {
                    new CorePropertyBuilder(),
                    new CorePropertyBuilder().WithTerritory(new TerritoryBuilder().WithGuid())
                });
            var service = new ActionItemService(_loggerMock.Object, () => _ouServiceMock.Object,
                () => _propertyServiceMock.Object, () => _unitOfWorkMock.Object);

            //  act
            var exception =
                Assert.Throws<ApplicationException>(
                    () => service.UploadActionItems(Guid.NewGuid(), new List<ActionItemInput>()));

            //  assert
            Assert.AreEqual("Invalid property territory", exception.Message);
        }

        [Test]
        [Category("UploadActionItems")]
        public void UploadActionItems_TerritoryInInvalidOu_Throws()
        {
            //  arrange
            var parentOU = new OuBuilder().WithGuid();
            var ou = new OuBuilder().WithGuid().WithParent(parentOU);

            _ouServiceMock.Setup(
                ous =>
                    ous.GetHierarchicalOrganizationGuids(new List<Guid> {ou.Object.Guid}, It.IsAny<ICollection<Guid>>()))
                .Returns(new List<Guid> {parentOU.Object.Guid, ou.Object.Guid});

            var territory = new TerritoryBuilder().WithGuid().WithOU(ou);
            var property = new CorePropertyBuilder().WithGuid().WithTerritory(territory);
            var invalidTerritory = new TerritoryBuilder().WithGuid().WithOU(new OuBuilder().WithGuid());
            var invalidProperty = new CorePropertyBuilder().WithGuid().WithTerritory(invalidTerritory);

            _propertyServiceMock.Setup(
                ps =>
                    ps.GetMany(new List<Guid> {property.Object.Guid, invalidProperty.Object.Guid}, It.IsAny<string>(),
                        It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new List<Property> {property, invalidProperty});

            var service = new ActionItemService(_loggerMock.Object, () => _ouServiceMock.Object,
                () => _propertyServiceMock.Object, () => _unitOfWorkMock.Object);

            //  act
            var exception = Assert.Throws<ApplicationException>(() => service.UploadActionItems(ou.Object.Guid,
                new List<ActionItemInput>
                {
                    new ActionItemInput {PropertyID = property.Object.Guid},
                    new ActionItemInput {PropertyID = invalidProperty.Object.Guid}
                }));

            //  assert
            Assert.AreEqual("Invalid action items", exception.Message);
        }

        [Test]
        [Category("UploadActionItems")]
        public void UploadActionItems_ValidInput_ActionItemsMerged()
        {
            //  arrange
            var ou = new OuBuilder().WithGuid();

            _ouServiceMock.Setup(
                ous =>
                    ous.GetHierarchicalOrganizationGuids(new List<Guid> {ou.Object.Guid}, It.IsAny<ICollection<Guid>>()))
                .Returns(new List<Guid> {ou.Object.Guid});

            var territory = new TerritoryBuilder().WithGuid().WithOU(ou);
            var property = new CorePropertyBuilder().WithGuid().WithTerritory(territory);

            _propertyServiceMock.Setup(
                ps =>
                    ps.GetMany(It.IsAny<List<Guid>>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                        It.IsAny<bool>()))
                .Returns(new List<Property> {property});

            var person = new PersonBuilder().WithGuid();
            var newPerson = new PersonBuilder().WithGuid();
            var actionItem =
                new PropertyActionItemBuilder().WithGuid()
                    .WithProperty(property)
                    .WithPerson(person)
                    .WithDescription("action1")
                    .WithStatus(ActionItemStatus.Unread);
            _unitOfWorkMock.Setup(uow => uow.Get<PropertyActionItem>())
                .Returns(new List<PropertyActionItem> {actionItem}.AsQueryable());

            PropertyActionItem newActionItem = null;
            _unitOfWorkMock.Setup(uow => uow.Add(It.IsAny<PropertyActionItem>()))
                .Callback<PropertyActionItem>((item) => newActionItem = item);

            var service = new ActionItemService(_loggerMock.Object, () => _ouServiceMock.Object,
                () => _propertyServiceMock.Object, () => _unitOfWorkMock.Object);

            //  act
            service.UploadActionItems(ou.Object.Guid, new List<ActionItemInput>
            {
                new ActionItemInput
                {
                    Guid = actionItem.Object.Guid,
                    Description = "new action1",
                    PersonID = newPerson.Object.Guid,
                    PropertyID = Guid.NewGuid(),
                    Status = ActionItemStatus.Read
                },
                new ActionItemInput
                {
                    Description = "new action2",
                    PersonID = person.Object.Guid,
                    PropertyID = property.Object.Guid,
                    Status = ActionItemStatus.Unread
                }
            });

            //  assert
            Assert.AreEqual(actionItem.Object.Description, "new action1");
            Assert.AreEqual(actionItem.Object.PersonID, newPerson.Object.Guid);
            Assert.AreEqual(actionItem.Object.PropertyID, property.Object.Guid);
            Assert.AreEqual(actionItem.Object.Status, ActionItemStatus.Read);
            _unitOfWorkMock.Verify(uow => uow.Update(actionItem.Object), Times.Once);

            Assert.IsNotNull(newActionItem);
            Assert.AreEqual("new action2", newActionItem.Description);
            Assert.AreEqual(person.Object.Guid, newActionItem.PersonID);
            Assert.AreEqual(property.Object.Guid, newActionItem.PropertyID);
            Assert.AreEqual(ActionItemStatus.Unread, newActionItem.Status);
            _unitOfWorkMock.Verify(uow => uow.Add(newActionItem), Times.Once);

            _unitOfWorkMock.Verify(uow => uow.SaveChanges(), Times.Once);
        }

        [Test]
        [Category("UploadActionItems")]
        public void UploadActionItems_NewActionItemNoPerson_ActionItemNotAdded()
        {
            //  arrange
            var ou = new OuBuilder().WithGuid();

            _ouServiceMock.Setup(
                ous =>
                    ous.GetHierarchicalOrganizationGuids(new List<Guid> {ou.Object.Guid}, It.IsAny<ICollection<Guid>>()))
                .Returns(new List<Guid> {ou.Object.Guid});

            var territory = new TerritoryBuilder().WithGuid().WithOU(ou);
            var property = new CorePropertyBuilder().WithGuid().WithTerritory(territory);

            _propertyServiceMock.Setup(
                ps =>
                    ps.GetMany(It.IsAny<List<Guid>>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                        It.IsAny<bool>()))
                .Returns(new List<Property> {property});

            var service = new ActionItemService(_loggerMock.Object, () => _ouServiceMock.Object,
                () => _propertyServiceMock.Object, () => _unitOfWorkMock.Object);

            //  act
            service.UploadActionItems(ou.Object.Guid, new List<ActionItemInput>
            {
                new ActionItemInput
                {
                    Description = "new action2",
                    PropertyID = property.Object.Guid,
                    Status = ActionItemStatus.Unread
                }
            });

            //  assert
            _unitOfWorkMock.Verify(uow => uow.Add(It.IsAny<PropertyActionItem>()), Times.Never);
        }

        [Test]
        [Category("UploadActionItems")]
        public void UploadActionItems_NewActionItemNoDescription_ActionItemNotAdded()
        {
            //  arrange
            var ou = new OuBuilder().WithGuid();

            _ouServiceMock.Setup(
                ous =>
                    ous.GetHierarchicalOrganizationGuids(new List<Guid> {ou.Object.Guid}, It.IsAny<ICollection<Guid>>()))
                .Returns(new List<Guid> {ou.Object.Guid});

            var territory = new TerritoryBuilder().WithGuid().WithOU(ou);
            var property = new CorePropertyBuilder().WithGuid().WithTerritory(territory);

            _propertyServiceMock.Setup(
                ps =>
                    ps.GetMany(It.IsAny<List<Guid>>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                        It.IsAny<bool>()))
                .Returns(new List<Property> {property});

            var person = new PersonBuilder().WithGuid();
            var service = new ActionItemService(_loggerMock.Object, () => _ouServiceMock.Object,
                () => _propertyServiceMock.Object, () => _unitOfWorkMock.Object);

            //  act
            service.UploadActionItems(ou.Object.Guid, new List<ActionItemInput>
            {
                new ActionItemInput
                {
                    PersonID = person.Object.Guid,
                    PropertyID = property.Object.Guid,
                    Status = ActionItemStatus.Unread
                }
            });

            //  assert
            _unitOfWorkMock.Verify(uow => uow.Add(It.IsAny<PropertyActionItem>()), Times.Never);
        }

        [Test]
        [Category("UploadActionItems")]
        public void UploadActionItems_NewActionItemNoStatus_ActionItemNotAdded()
        {
            //  arrange
            var ou = new OuBuilder().WithGuid();

            _ouServiceMock.Setup(
                ous => ous.GetHierarchicalOrganizationGuids(new List<Guid> { ou.Object.Guid }, It.IsAny<ICollection<Guid>>()))
                .Returns(new List<Guid> { ou.Object.Guid });

            var territory = new TerritoryBuilder().WithGuid().WithOU(ou);
            var property = new CorePropertyBuilder().WithGuid().WithTerritory(territory);

            _propertyServiceMock.Setup(
                ps => ps.GetMany(It.IsAny<List<Guid>>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new List<Property> { property });

            var person = new PersonBuilder().WithGuid();
            var service = new ActionItemService(_loggerMock.Object, () => _ouServiceMock.Object, () => _propertyServiceMock.Object, () => _unitOfWorkMock.Object);

            //  act
            service.UploadActionItems(ou.Object.Guid, new List<ActionItemInput>
            {
                new ActionItemInput
                {
                    Description = "new action2",
                    PersonID = person.Object.Guid,
                    PropertyID = property.Object.Guid
                }
            });

            //  assert
            _unitOfWorkMock.Verify(uow => uow.Add(It.IsAny<PropertyActionItem>()), Times.Never);
        }
    }

    #endregion
}
