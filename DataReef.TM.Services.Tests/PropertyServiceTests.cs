using DataReef.Core.Infrastructure.Repository;
using DataReef.Core.Logging;
using DataReef.Integrations;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Contracts.Services.FinanceAdapters;
using DataReef.TM.Models.DTOs;
using DataReef.TM.Models.Geo;
using DataReef.TM.Services.InternalServices.Geo;
using DataReef.TM.Services.Services;
using DataReef.TM.Services.Tests.Builders;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Geo = DataReef.Integrations.Common.Geo;

namespace DataReef.TM.Services.Tests
{
    [TestFixture]
    public class PropertyServiceTests
    {
        private Mock<ILogger> _loggerMock;
        private Mock<IGeoProvider> _geoProviderMock;
        private Mock<IGeographyBridge> _geographyBridgeMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IDeviceService> _deviceServiceMock;
        private Mock<ISolarSalesTrackerAdapter> _sbAdapter;
        private Mock<IPropertyNotesAdapter> _propertyNotesAdapter;
        private Mock<ISunlightAdapter> _sunlightAdapter;
        private Mock<ISunnovaAdapter> _sunnovaAdapter;
        private Mock<IJobNimbusAdapter> _jobNimbusAdapter;
        private Mock<IOUService> _ouService;
        private Mock<IOUSettingService> _ouSettingService;
        private Mock<ITerritoryService> _territoryService;

        [SetUp]
        public void Init()
        {
            _loggerMock = new Mock<ILogger>();
            _geoProviderMock = new Mock<IGeoProvider>();
            _geographyBridgeMock = new Mock<IGeographyBridge>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _deviceServiceMock = new Mock<IDeviceService>();
            _sbAdapter = new Mock<ISolarSalesTrackerAdapter>();
            _propertyNotesAdapter = new Mock<IPropertyNotesAdapter>();
            _sunlightAdapter = new Mock<ISunlightAdapter>();
            _sunnovaAdapter = new Mock<ISunnovaAdapter>();
            _jobNimbusAdapter = new Mock<IJobNimbusAdapter>();
            _ouService = new Mock<IOUService>();
            _ouSettingService = new Mock<IOUSettingService>();
            _territoryService = new Mock<ITerritoryService>();
        }

        private PropertyService GetService()
        {
            return new PropertyService(_loggerMock.Object,
                _geoProviderMock.Object,
                () => _geographyBridgeMock.Object,
                () => _unitOfWorkMock.Object,
                new Lazy<IDeviceService>(() => _deviceServiceMock.Object),
                new Lazy<ISolarSalesTrackerAdapter>(() => _sbAdapter.Object),
                new Lazy<IPropertyNotesAdapter>(() => _propertyNotesAdapter.Object),
                new Lazy<ISunlightAdapter>(() => _sunlightAdapter.Object),
                new Lazy<ISunnovaAdapter>(() => _sunnovaAdapter.Object),
                new Lazy<IJobNimbusAdapter>(() => _jobNimbusAdapter.Object),
                new Lazy<IOUService>(() => _ouService.Object),
                new Lazy<IOUSettingService>(() => _ouSettingService.Object),
                new Lazy<ITerritoryService>(() => _territoryService.Object),
                null,
                null,
                null,
                null);
        }
        #region GetProperties

        [Test]
        [Category("GetProperties")]
        public void GetProperties_InvalidPropertiesRequest_Throws()
        {
            //  arrange
            var service = GetService();

            //  act, assert
            Assert.Throws<ArgumentNullException>(() => service.GetProperties(null));
        }

        [Test]
        [Category("GetProperties")]
        public void GetProperties_NoRequests_NoPropertiesReturned()
        {
            //  arrange
            _unitOfWorkMock.Setup(uow => uow.Get<Models.Property>()).Returns(new List<Models.Property> { new Models.Property() }.AsQueryable());
            var service = GetService();

            //  act
            var properties = service.GetProperties(new GetPropertiesRequest());

            //  assert
            Assert.IsNotNull(properties);
            Assert.AreEqual(0, properties.Count);
        }

        [Test]
        [Category("GetProperties")]
        public void GetProperties_InvalidTerritory_NoPropertiesReturned()
        {
            //  arrange
            _unitOfWorkMock.Setup(uow => uow.Get<Models.Property>()).Returns(new List<Models.Property> { new Models.Property() }.AsQueryable());
            var service = GetService();

            //  act
            var properties = service.GetProperties(new GetPropertiesRequest
            {
                PropertiesRequest = new ListPropertiesRequest(),
                GeoPropertiesRequest = new List<Geo.PropertiesRequest>(),
                TerritoryID = Guid.Empty
            });

            //  assert
            Assert.IsNotNull(properties);
            Assert.AreEqual(0, properties.Count);
        }

        [Test]
        [Category("GetProperties")]
        public void GetProperties_ValidPropertyRequest_FilteredPropertiesReturned()
        {
            //  arrange
            var territory1 = new TerritoryBuilder().WithGuid();
            var territory2 = new TerritoryBuilder().WithGuid();
            var utcNow = DateTime.UtcNow;

            _unitOfWorkMock.Setup(uow => uow.Get<Models.Property>()).Returns(new List<Models.Property>
            {
                new CorePropertyBuilder().WithTerritory(territory1).WithName("Prop1").WithDateCreated(utcNow).WithDateLastModified(utcNow),
                new CorePropertyBuilder().WithTerritory(territory1).WithName("Prop2").WithDateCreated(utcNow.AddDays(-1)).WithDateLastModified(utcNow.AddDays(-1)),
                new CorePropertyBuilder().WithTerritory(territory1).WithName("Prop3").WithDateCreated(utcNow.AddDays(-2)).WithDateLastModified(utcNow.AddDays(-2)),
                new CorePropertyBuilder().WithTerritory(territory1).WithName("Prop4").WithDateCreated(utcNow).WithDateLastModified(utcNow).AsDeleted(),
                new CorePropertyBuilder().WithTerritory(territory2).WithName("Prop5").WithDateCreated(utcNow).WithDateLastModified(utcNow)
            }.AsQueryable());
            var service = GetService();

            //  act
            var properties = service.GetProperties(new GetPropertiesRequest
            {
                PropertiesRequest = new ListPropertiesRequest
                {
                    DeletedItems = false,
                    ItemsPerPage = 2,
                    PageNumber = 1,
                    LastRetrievedDate = utcNow
                },
                TerritoryID = territory1.Object.Guid
            });

            //  assert
            Assert.IsNotNull(properties);
            Assert.AreEqual(1, properties.Count);

            var resultProperty = properties.ElementAt(0);
            Assert.AreEqual(territory1.Object.Guid, resultProperty.TerritoryID);
            Assert.AreEqual(false, resultProperty.IsGeoProperty);
            Assert.AreEqual("Prop1", resultProperty.Name);
        }

        [Test]
        [Category("GetProperties")]
        public void GetProperties_ValidGeoPropertyRequest_GeoPropertiesReturned()
        {
            //  arrange
            var territory = new TerritoryBuilder().WithGuid();
            _unitOfWorkMock.Setup(uow => uow.Get<Models.Property>()).Returns(new List<Models.Property>
            {
                new CorePropertyBuilder().WithTerritory(territory),
                new CorePropertyBuilder().WithTerritory(territory)
            }.AsQueryable());
            _geographyBridgeMock.Setup(gb => gb.GetProperties(It.IsAny<List<Geo.PropertiesRequest>>()))
                .Returns(new List<Geo.Property>
                {
                    new GeoPropertyBuilder().WithId("id1"),
                    new GeoPropertyBuilder().WithId("id2"),
                    new GeoPropertyBuilder().WithId("id2"),
                });
            var service = GetService();

            //  act
            var properties = service.GetProperties(new GetPropertiesRequest
            {
                GeoPropertiesRequest = new List<Geo.PropertiesRequest> { new Geo.PropertiesRequest() },
                TerritoryID = territory.Object.Guid
            });

            //  assert
            Assert.IsNotNull(properties);
            Assert.AreEqual(3, properties.Count);
            Assert.IsTrue(properties.All(p => p.IsGeoProperty && p.TerritoryID == territory.Object.Guid));
        }

        [Test]
        [Category("GetProperties")]
        public void GetProperties_ValidRequest_CommonPropertiesExcluded()
        {
            //  arrange
            var territory = new TerritoryBuilder().WithGuid();
            _unitOfWorkMock.Setup(uow => uow.Get<Models.Property>()).Returns(new List<Models.Property>
            {
                new CorePropertyBuilder().WithTerritory(territory).WithExternalID("Ext1"),
                new CorePropertyBuilder().WithTerritory(territory).WithExternalID("Ext2"),
                new CorePropertyBuilder().WithTerritory(territory).WithExternalID("ext3")
            }.AsQueryable());
            _geographyBridgeMock.Setup(gb => gb.GetProperties(It.IsAny<List<Geo.PropertiesRequest>>()))
                .Returns(new List<Geo.Property>
                {
                    new GeoPropertyBuilder().WithId("ext2"),
                    new GeoPropertyBuilder().WithId("Ext3"),
                    new GeoPropertyBuilder().WithId("Ext4")
                });
            var service = GetService();

            //  act
            var properties = service.GetProperties(new GetPropertiesRequest
            {
                PropertiesRequest = new ListPropertiesRequest(),
                GeoPropertiesRequest = new List<Geo.PropertiesRequest>(),
                TerritoryID = territory.Object.Guid
            });

            //  assert
            Assert.IsNotNull(properties);
            Assert.AreEqual(4, properties.Count);
            Assert.AreEqual(1, properties.Count(p => !p.IsGeoProperty && p.ExternalID.Equals("Ext1", StringComparison.InvariantCultureIgnoreCase)));
            Assert.AreEqual(1, properties.Count(p => !p.IsGeoProperty && p.ExternalID.Equals("Ext2", StringComparison.InvariantCultureIgnoreCase)));
            Assert.AreEqual(1, properties.Count(p => !p.IsGeoProperty && p.ExternalID.Equals("Ext3", StringComparison.InvariantCultureIgnoreCase)));
            Assert.AreEqual(1, properties.Count(p => p.IsGeoProperty && p.ExternalID.Equals("Ext4", StringComparison.InvariantCultureIgnoreCase) && p.TerritoryID == territory.Object.Guid));
        }

        #endregion

        #region SyncProperty

        [Test]
        [Category("SyncProperty")]
        public void SyncProperty_InvalidRequest_Throws()
        {
            //  arrange
            var service = GetService();

            //  act, assert
            Assert.Throws<ArgumentException>(() => service.SyncProperty(Guid.Empty));
        }

        [Test]
        [Category("SyncProperty")]
        public void SyncProperty_InvalidProperty_Throws()
        {
            //  arrange
            var service = GetService();

            //  act, assert
            Assert.Throws<ApplicationException>(() => service.SyncProperty(Guid.NewGuid()));
        }

        [Test]
        [Category("SyncProperty")]
        public void SyncProperty_GeoPropertyNull_NotSynced()
        {
            //  arrange
            var property = new CorePropertyBuilder().WithGuid();
            _unitOfWorkMock.Setup(uow => uow.Get<Models.Property>()).Returns(new List<Models.Property> { property }.AsQueryable());
            _geographyBridgeMock.Setup(gb => gb.GetProperties(It.IsAny<List<Geo.PropertiesRequest>>())).Returns((ICollection<Geo.Property>)null);

            var service = GetService();

            //  act
            service.SyncProperty(property.Object.Guid);

            //  assert
            _unitOfWorkMock.Verify(uow => uow.SaveChanges(), Times.Never);
        }

        [Test]
        [Category("SyncProperty")]
        public void SyncProperty_GeoPropertyNotFound_NotSynced()
        {
            //  arrange
            var property = new CorePropertyBuilder().WithGuid().WithExternalID("ext1");
            var geoProperty = new GeoPropertyBuilder().WithId("ext2");

            _unitOfWorkMock.Setup(uow => uow.Get<Models.Property>()).Returns(new List<Models.Property> { property }.AsQueryable());
            _geographyBridgeMock.Setup(gb => gb.GetProperties(It.IsAny<List<Geo.PropertiesRequest>>())).Returns(new List<Geo.Property> { geoProperty });

            var service = GetService();

            //  act
            service.SyncProperty(property.Object.Guid);

            //  assert
            _unitOfWorkMock.Verify(uow => uow.SaveChanges(), Times.Never);
        }

        [Test]
        [Category("SyncProperty")]
        public void SyncProperty_GeoPropertyNoOccupants_OccupantsNotSynced()
        {
            //  arrange
            var property = new CorePropertyBuilder().WithGuid().WithExternalID("ext1");
            var geoProperty = new GeoPropertyBuilder().WithId("ext1");

            _unitOfWorkMock.Setup(uow => uow.Get<Models.Property>()).Returns(new List<Models.Property> { property }.AsQueryable());
            _geographyBridgeMock.Setup(gb => gb.GetProperties(It.IsAny<List<Geo.PropertiesRequest>>())).Returns(new List<Geo.Property> { geoProperty });

            var service = GetService();

            //  act
            var result = service.SyncProperty(property.Object.Guid);

            //  assert
            _unitOfWorkMock.Verify(uow => uow.SaveChanges(), Times.Once);
            Assert.IsNull(result.Occupants);
        }

        [Test]
        [Category("SyncProperty")]
        public void SyncProperty_PropertyHasOccupants_OccupantsNotSynced()
        {
            //  arrange
            var property = new CorePropertyBuilder().WithGuid().WithExternalID("ext1").WithOccupants(new List<Occupant> { new OccupantBuilder() });
            var geoProperty = new GeoPropertyBuilder().WithId("ext1").WithOccupants(new List<Geo.Occupant> { new GeoOccupantBuilder() });

            _unitOfWorkMock.Setup(uow => uow.Get<Models.Property>()).Returns(new List<Models.Property> { property }.AsQueryable());
            _geographyBridgeMock.Setup(gb => gb.GetProperties(It.IsAny<List<Geo.PropertiesRequest>>())).Returns(new List<Geo.Property> { geoProperty });

            var service = GetService();

            //  act
            var result = service.SyncProperty(property.Object.Guid);

            //  assert
            _unitOfWorkMock.Verify(uow => uow.SaveChanges(), Times.Once);
            Assert.AreEqual(1, result.Occupants.Count);
        }

        [Test]
        [Category("SyncProperty")]
        public void SyncProperty_PropertyNoOccupants_OccupantsSynced()
        {
            //  arrange
            var property = new CorePropertyBuilder().WithGuid().WithExternalID("ext1");
            var geoProperty = new GeoPropertyBuilder().WithId("ext1").WithOccupants(new List<Geo.Occupant> { new GeoOccupantBuilder(), new GeoOccupantBuilder() });

            _unitOfWorkMock.Setup(uow => uow.Get<Models.Property>()).Returns(new List<Models.Property> { property }.AsQueryable());
            _geographyBridgeMock.Setup(gb => gb.GetProperties(It.IsAny<List<Geo.PropertiesRequest>>())).Returns(new List<Geo.Property> { geoProperty });

            var service = GetService();

            //  act
            var result = service.SyncProperty(property.Object.Guid);

            //  assert
            _unitOfWorkMock.Verify(uow => uow.SaveChanges(), Times.Once);
            Assert.AreEqual(2, result.Occupants.Count);
        }


        [Test]
        [Category("SyncProperty")]
        public void SyncProperty_GeoPropertyNoPropertyBag_PropertyBagNotSynced()
        {
            //  arrange
            var property = new CorePropertyBuilder().WithGuid().WithExternalID("ext1");
            var geoProperty = new GeoPropertyBuilder().WithId("ext1");

            _unitOfWorkMock.Setup(uow => uow.Get<Models.Property>()).Returns(new List<Models.Property> { property }.AsQueryable());
            _geographyBridgeMock.Setup(gb => gb.GetProperties(It.IsAny<List<Geo.PropertiesRequest>>())).Returns(new List<Geo.Property> { geoProperty });

            var service = GetService();

            //  act
            var result = service.SyncProperty(property.Object.Guid);

            //  assert
            _unitOfWorkMock.Verify(uow => uow.SaveChanges(), Times.Once);
            Assert.IsNull(result.PropertyBag);
        }

        [Test]
        [Category("SyncProperty")]
        public void SyncProperty_PropertyHasPropertyBag_PropertyBagNotSynced()
        {
            //  arrange
            var property = new CorePropertyBuilder().WithGuid().WithExternalID("ext1").WithPropertyBag(new List<Field> { new FieldBuilder() });
            var geoProperty = new GeoPropertyBuilder().WithId("ext1").WithPropertyBag(new List<Geo.Field> { new GeoFieldBuilder() });

            _unitOfWorkMock.Setup(uow => uow.Get<Models.Property>()).Returns(new List<Models.Property> { property }.AsQueryable());
            _geographyBridgeMock.Setup(gb => gb.GetProperties(It.IsAny<List<Geo.PropertiesRequest>>())).Returns(new List<Geo.Property> { geoProperty });

            var service = GetService();

            //  act
            var result = service.SyncProperty(property.Object.Guid);

            //  assert
            _unitOfWorkMock.Verify(uow => uow.SaveChanges(), Times.Once);
            Assert.AreEqual(1, result.PropertyBag.Count);
        }

        [Test]
        [Category("SyncProperty")]
        public void SyncProperty_PropertyNoPropertyBag_PropertyBagSynced()
        {
            //  arrange
            var property = new CorePropertyBuilder().WithGuid().WithExternalID("ext1");
            var geoProperty = new GeoPropertyBuilder().WithId("ext1").WithPropertyBag(new List<Geo.Field> { new GeoFieldBuilder(), new GeoFieldBuilder() });

            _unitOfWorkMock.Setup(uow => uow.Get<Models.Property>()).Returns(new List<Models.Property> { property }.AsQueryable());
            _geographyBridgeMock.Setup(gb => gb.GetProperties(It.IsAny<List<Geo.PropertiesRequest>>())).Returns(new List<Geo.Property> { geoProperty });

            var service = GetService();

            //  act
            var result = service.SyncProperty(property.Object.Guid);

            //  assert
            _unitOfWorkMock.Verify(uow => uow.SaveChanges(), Times.Once);
            Assert.AreEqual(2, result.PropertyBag.Count);
        }

        [Test]
        [Category("SyncProperty")]
        public void SyncProperty_GeoPropertyNoAttributes_AttributesNotSynced()
        {
            //  arrange
            var property = new CorePropertyBuilder().WithGuid().WithExternalID("ext1");
            var geoProperty = new GeoPropertyBuilder().WithId("ext1");

            _unitOfWorkMock.Setup(uow => uow.Get<Models.Property>()).Returns(new List<Models.Property> { property }.AsQueryable());
            _geographyBridgeMock.Setup(gb => gb.GetProperties(It.IsAny<List<Geo.PropertiesRequest>>())).Returns(new List<Geo.Property> { geoProperty });

            var service = GetService();

            //  act
            var result = service.SyncProperty(property.Object.Guid);

            //  assert
            _unitOfWorkMock.Verify(uow => uow.SaveChanges(), Times.Once);
            Assert.IsNull(result.Attributes);
        }

        [Test]
        [Category("SyncProperty")]
        public void SyncProperty_PropertyHasAttributes_NewAttributesSynced()
        {
            //  arrange
            var utcNow = DateTime.UtcNow;

            var property = new CorePropertyBuilder().WithGuid().WithExternalID("ext1").WithAttributes(new List<Models.PropertyAttribute> { new PropertyAttributeBuilder().WithDateCreated(utcNow.AddDays(-2)) });
            var geoProperty =
                new GeoPropertyBuilder().WithId("ext1")
                    .WithAttributes(new List<Geo.PropertyAttribute>
                    {
                        new GeoPropertyAttributeBuilder().WithDateCreated(utcNow.AddDays(-1)),
                        new GeoPropertyAttributeBuilder().WithDateCreated(utcNow.AddDays(-3))
                    });

            _unitOfWorkMock.Setup(uow => uow.Get<Models.Property>()).Returns(new List<Models.Property> { property }.AsQueryable());
            _geographyBridgeMock.Setup(gb => gb.GetProperties(It.IsAny<List<Geo.PropertiesRequest>>())).Returns(new List<Geo.Property> { geoProperty });

            var service = GetService();

            //  act
            var result = service.SyncProperty(property.Object.Guid);

            //  assert
            _unitOfWorkMock.Verify(uow => uow.SaveChanges(), Times.Once);
            Assert.AreEqual(2, result.Attributes.Count);
            Assert.AreEqual(1, result.Attributes.Count(a => a.DateCreated == utcNow.AddDays(-1)));
            Assert.AreEqual(1, result.Attributes.Count(a => a.DateCreated == utcNow.AddDays(-2)));
        }

        [Test]
        [Category("SyncProperty")]
        public void SyncProperty_PropertyNoAttributes_AttributesSynced()
        {
            //  arrange
            var property = new CorePropertyBuilder().WithGuid().WithExternalID("ext1");
            var geoProperty = new GeoPropertyBuilder().WithId("ext1").WithAttributes(new List<Geo.PropertyAttribute> { new GeoPropertyAttributeBuilder(), new GeoPropertyAttributeBuilder() });

            _unitOfWorkMock.Setup(uow => uow.Get<Models.Property>()).Returns(new List<Models.Property> { property }.AsQueryable());
            _geographyBridgeMock.Setup(gb => gb.GetProperties(It.IsAny<List<Geo.PropertiesRequest>>())).Returns(new List<Geo.Property> { geoProperty });

            var service = GetService();

            //  act
            var result = service.SyncProperty(property.Object.Guid);

            //  assert
            _unitOfWorkMock.Verify(uow => uow.SaveChanges(), Times.Once);
            Assert.AreEqual(2, result.Attributes.Count);
        }

        #endregion
    }
}
